﻿using BF1MarneTools.Api;
using BF1MarneTools.Helper;

namespace BF1MarneTools.Core;

public static class LSXTcpServer
{
    private static TcpListener _tcpServer = null;

    private static readonly List<string> ScoketMsgBFV = [];

    static LSXTcpServer()
    {
        // 加载XML字符串
        for (int i = 0; i <= 27; i++)
        {
            var text = FileHelper.GetEmbeddedResourceText($"LSX.{i:D2}.xml");

            // 头像 \AppData\Local\Origin\AvatarsCache（不清楚为啥不显示）
            text = text.Replace("##AvatarId##", "Avatars40.jpg");

            ScoketMsgBFV.Add(text);
        }

        // 这里结束符必须要加
        ScoketMsgBFV[0] = string.Concat(ScoketMsgBFV[0], "\0");
        ScoketMsgBFV[1] = string.Concat(ScoketMsgBFV[1], "\0");

        ScoketMsgBFV[26] = string.Concat(ScoketMsgBFV[26], "\0");
    }

    /// <summary>
    /// 启动 TCP 监听服务
    /// </summary>
    public static void Run()
    {
        if (_tcpServer is not null)
        {
            LoggerHelper.Warn("LSX 监听服务已经在运行，请勿重复启动");
            return;
        }

        _tcpServer = new TcpListener(IPAddress.Parse("127.0.0.1"), 3216);
        _tcpServer.Start();

        LoggerHelper.Info("启动 LSX 监听服务成功");
        LoggerHelper.Debug("LSX 服务监听端口为 3216");

        _tcpServer.BeginAcceptTcpClient(Result, null);
    }

    /// <summary>
    /// 停止 TCP 监听服务
    /// </summary>
    public static void Stop()
    {
        _tcpServer?.Stop();
        _tcpServer = null;
        LoggerHelper.Info("停止 LSX 监听服务成功");
    }

    /// <summary>
    /// 获取玩家列表Xml字符串
    /// </summary>
    private static string GetFriendsXmlString()
    {
        return ScoketMsgBFV[11];
    }

    /// <summary>
    /// 处理战地1客户端关闭异常
    /// </summary>
    private static bool MakeGameClientCloseEx(Exception ex)
    {
        if (ex.InnerException is not SocketException)
            return false;

        LoggerHelper.Warn("警告: 战地1游戏客户端已被关闭。");
        return true;
    }

    /// <summary>
    /// 处理TCP客户端连接
    /// </summary>
    private static async void Result(IAsyncResult asyncResult)
    {
        // 避免服务关闭时抛出异常
        if (_tcpServer is null)
            return;

        // 完成检索传入的客户端请求的异步操作
        var client = _tcpServer?.EndAcceptTcpClient(asyncResult);
        // 开始异步检索传入的请求（下一个请求）
        _tcpServer?.BeginAcceptTcpClient(Result, null);

        // 保存客户端连接Ip和地址
        var clientIp = string.Empty;

        try
        {
            // 如果连接断开，则结束
            if (!client.Connected)
                return;

            clientIp = client.Client.RemoteEndPoint.ToString();
            LoggerHelper.Debug($"发现 TCP 客户端连接 {clientIp}");

            /////////////////////////////////////////////////

            // 建立和连接的客户端的数据流（传输数据）
            var networkStream = client.GetStream();
            // 设置读写超时时间为 3600 秒
            networkStream.ReadTimeout = 3600000;
            networkStream.WriteTimeout = 3600000;

            var startKey = "cacf897a20b6d612ad0c05e011df52bb";
            var buffer = Encoding.UTF8.GetBytes(ScoketMsgBFV[0].Replace("##KEY##", startKey));

            // 异步写入网络流
            await networkStream.WriteAsync(buffer);

            var tcpString = await ReadTcpString(client, networkStream);
            var partArray = tcpString.Split('\"');

            // 适配FC24
            var doc = XDocument.Parse(tcpString.Replace("version=\"\"", "version1=\"\""));
            var request = doc.Element("LSX").Element("Request");
            var contentId = request.Element("ChallengeResponse").Element("ContentId").Value;

            // 重要！！！
            var response = partArray[5];
            var key = partArray[7];

            LoggerHelper.Debug($"本次启动 Challenge Response 为 {response}");
            LoggerHelper.Info($"本次启动 ContentId 为 {contentId}");
            LoggerHelper.Info("正在处理启动游戏请求...");

            // 检查 Challenge 响应
            if (!EaCrypto.CheckChallengeResponse(response, startKey))
            {
                LoggerHelper.Fatal("Challenge Response 致命错误!");
                return;
            }

            // 处理解密 Challenge 响应
            var newResponse = EaCrypto.MakeChallengeResponse(key);
            LoggerHelper.Debug($"处理解密 Challenge 响应 NewResponse {newResponse}");

            var seed = (ushort)((newResponse[0] << 8) | newResponse[1]);
            LoggerHelper.Debug($"处理解密 Challenge 响应 Seed {newResponse}");

            // 处理请求
            buffer = Encoding.UTF8.GetBytes(ScoketMsgBFV[26].Replace("##RESPONSE##", newResponse).Replace("##ID##", partArray[3]));

            // 异步写入网络流
            await networkStream.WriteAsync(buffer);

            // 这里死循环要注意
            // 仅客户端已连接时运行
            while (client.Connected)
            {
                try
                {
                    var data = await ReadTcpString(client, networkStream);
                    data = EaCrypto.LSXDecryptBF4(data, seed);

                    data = await LSXRequestHandleForBFV(data, contentId);
                    LoggerHelper.Debug($"当前 LSX 回复 {data}");

                    data = EaCrypto.LSXEncryptBF4(data, seed);
                    await WriteTcpString(client, networkStream, $"{data}\0");
                }
                catch (TimeoutException ex)
                {
                    LoggerHelper.Error("处理 TCP Battlelog 客户端连接发生超时异常", ex);
                }
            }
        }
        catch (Exception ex)
        {
            // 处理异常
            if (!MakeGameClientCloseEx(ex))
                LoggerHelper.Error("处理 TCP 客户端连接发生异常", ex);
        }
        finally
        {
            client?.Close();
            LoggerHelper.Debug($"TCP 客户端连接处理结束 {clientIp}");
        }
    }

    /// <summary>
    /// 异步读取 TCP 网络流字符串
    /// </summary>
    private static async Task<string> ReadTcpString(TcpClient client, NetworkStream stream)
    {
        // 如果客户端连接断开，则返回空字符串
        if (!client.Connected)
            return string.Empty;

        /**
         * 以异步的方式模拟 NetworkStream.ReadByte()
         * 为了修复傻逼重生家游戏只能这样干，重生你妈死了
         */

        var strBuilder = new StringBuilder();
        var buffer = new byte[1];       // 单字节缓冲区
        int readLength;                 // 实际读取的长度

        try
        {
            // 读取长度大于0时才执行
            // 当游戏关闭时，这个会发生异常（远程主机强迫关闭了一个现有的连接）
            while ((readLength = await stream.ReadAsync(buffer)) > 0)
            {
                var b = buffer[0];
                if (b == 0)             // 结束符
                    break;

                strBuilder.Append((char)b);
            }
        }
        catch (Exception ex)
        {
            // 处理异常
            if (!MakeGameClientCloseEx(ex))
                LoggerHelper.Error("异步读取 TCP 字符串时发生异常", ex);
        }

        return strBuilder.ToString();
    }

    /// <summary>
    /// 异步写入 TCP 网络流字符串
    /// </summary>
    private static async Task WriteTcpString(TcpClient client, NetworkStream stream, string tcpStr)
    {
        // 如果客户端连接断开，则结束
        if (!client.Connected)
            return;

        // 这个不要用 try catch 捕获异常
        // 主要是为了避免死循环无限执行（使用异常来中断死循环）
        var buffer = Encoding.UTF8.GetBytes(tcpStr);
        await stream.WriteAsync(buffer);
    }

    /// <summary>
    /// 处理 BFV LSX 请求
    /// </summary>
    private static async Task<string> LSXRequestHandleForBFV(string request, string contentId)
    {
        if (string.IsNullOrWhiteSpace(request))
            return string.Empty;

        LoggerHelper.Debug($"BFV LSX 请求 Request {request}");

        var partArray = request.Split('\"');
        LoggerHelper.Debug($"BFV LSX 请求 partArray 长度 {partArray.Length}");

        var id = partArray[3];
        var requestType = partArray[4];

        LoggerHelper.Debug($"BFV LSX 请求 Id {id}");
        LoggerHelper.Debug($"BFV LSX 请求 RequestType {requestType}");

        return requestType switch
        {
            "><GetConfig version=" => ScoketMsgBFV[2].Replace("##ID##", id),
            "><GetAuthCode ClientId=" => ScoketMsgBFV[3].Replace("##ID##", id).Replace("##AuthCode##", "AuthCode"),
            "><GetAuthCode UserId=" => ScoketMsgBFV[3].Replace("##ID##", id).Replace("##AuthCode##", "AuthCode"),
            "><GetBlockList version=" => ScoketMsgBFV[4].Replace("##ID##", id),
            "><GetGameInfo GameInfoId=" => partArray[5] switch
            {
                "FREETRIAL" => ScoketMsgBFV[19].Replace("##ID##", id),
                "UPTODATE" => ScoketMsgBFV[20].Replace("##ID##", id).Replace("##Locale##", "true"),
                "INSTALLED_LANGUAGE" => ScoketMsgBFV[20].Replace("##ID##", id).Replace("##Locale##", "zh_TW"),
                _ => ScoketMsgBFV[5].Replace("##ID##", id),
            },
            "><GetInternetConnectedState version=" => ScoketMsgBFV[6].Replace("##ID##", id),
            "><GetPresence UserId=" => ScoketMsgBFV[7].Replace("##ID##", id),
            "><GetProfile index=" => ScoketMsgBFV[8].Replace("##ID##", id).Replace("##PID##", "123456789").Replace("##DSNM##", "PlayerName").Replace("##UID##", "123456"),
            "><RequestLicense UserId=" => ScoketMsgBFV[15].Replace("##ID##", id).Replace("##License##", await EasyEaApi.GetLSXLicense(Globals.Email, Globals.Password, partArray[7], contentId)),
            "><GetSetting SettingId=" => partArray[5] switch
            {
                "ENVIRONMENT" => ScoketMsgBFV[9].Replace("##ID##", id),
                "IS_IGO_AVAILABLE" => ScoketMsgBFV[10].Replace("##ID##", id),
                "IS_IGO_ENABLED" => ScoketMsgBFV[10].Replace("##ID##", id),
                _ => string.Empty,
            },
            "><QueryFriends UserId=" => GetFriendsXmlString().Replace("##ID##", id),
            "><QueryImage ImageId=" => ScoketMsgBFV[12].Replace("##ID##", id).Replace("##ImageId##", partArray[5]).Replace("##Width##", partArray[7]),
            "><QueryPresence UserId=" => ScoketMsgBFV[13].Replace("##ID##", id),
            "><SetPresence UserId=" => ScoketMsgBFV[14].Replace("##ID##", id),
            "><GetAllGameInfo version=" => contentId switch
            {
                _ => ScoketMsgBFV[27].Replace("##ID##", id).Replace("##SystemTime##", $"{DateTime.Now:s}").Replace("##Locale##", "zh_TW").Replace("##Version##", "1.0.108.2038")
            },
            "><IsProgressiveInstallationAvailable ItemId=" => ScoketMsgBFV[17].Replace("##ID##", id).Replace("Origin.OFR.50.0004342", "Origin.OFR.50.0001455"),
            "><QueryContent UserId=" => ScoketMsgBFV[18].Replace("##ID##", id),
            "><QueryEntitlements UserId=" => ScoketMsgBFV[21].Replace("##ID##", id),
            "><QueryOffers UserId=" => ScoketMsgBFV[22].Replace("##ID##", id),
            "><SetDownloaderUtilization Utilization=" => ScoketMsgBFV[23].Replace("##ID##", id),
            "><QueryChunkStatus ItemId=" => ScoketMsgBFV[24].Replace("##ID##", id),
            "><GetPresenceVisibility UserId=" => ScoketMsgBFV[25].Replace("##ID##", id),
            _ => string.Empty,
        };
    }
}