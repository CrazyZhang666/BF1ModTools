using BF1MarneTools.Helper;
using BF1MarneTools.Utils;
using BF1MarneTools.Windows;

namespace BF1MarneTools.Core;

public static class Game
{
    /// <summary>
    /// 获取系统环境变量集合
    /// </summary>
    private static Dictionary<string, string> GetEnvironmentVariables()
    {
        var environmentVariables = new Dictionary<string, string>();
        foreach (DictionaryEntry dirEnity in Environment.GetEnvironmentVariables())
        {
            environmentVariables.Add(dirEnity.Key.ToString(), dirEnity.Value.ToString());
        }
        return environmentVariables;
    }

    /// <summary>
    /// 启动战地1游戏
    /// </summary>
    public static async Task RunBf1Game()
    {
        try
        {
            // 验证邮箱和密码是否为空
            if (string.IsNullOrWhiteSpace(Globals.Email) || string.IsNullOrWhiteSpace(Globals.Password))
            {
                var keyWindow = new KeyWindow
                {
                    Owner = MainWindow.MainWinInstance,
                };
                MainWindow.MainWinInstance.IsShowMaskLayer = true;
                keyWindow.ShowDialog();
                MainWindow.MainWinInstance.IsShowMaskLayer = false;

                return;
            }

            ////////////////////////////////////////////////////

            if (string.IsNullOrWhiteSpace(Globals.BF1AppPath))
            {
                LoggerHelper.Warn("战地1游戏路径为空，启动游戏终止");
                NotifierHelper.Warning("战地1游戏路径为空，启动游戏终止");
                return;
            }

            LoggerHelper.Info("正在启动战地1游戏中...");
            NotifierHelper.Notice("正在启动战地1游戏中...");

            // 获取当前进程所有环境变量名及其值
            var environmentVariables = GetEnvironmentVariables();

            // 自定义战地1文档目录
            var diceDir = Path.Combine(CoreUtil.Dir_CommonAppData, "DICE");
            environmentVariables["USERPROFILE"] = diceDir;

            FileHelper.CreateDirectory(Path.Combine(diceDir, "AppData\\Local"));
            FileHelper.CreateDirectory(Path.Combine(diceDir, "AppData\\LocalLow"));
            FileHelper.CreateDirectory(Path.Combine(diceDir, "AppData\\Roaming"));

            var settingsDir = Path.Combine(diceDir, "Documents\\Battlefield 1\\settings");
            FileHelper.CreateDirectory(settingsDir);
            // 战地1战役存档文件
            var profSave = Path.Combine(settingsDir, "PROFSAVE");
            if (!File.Exists(profSave))
            {
                var profSaveZip = Path.Combine(settingsDir, "PROFSAVE.zip");
                FileHelper.ExtractResFile("Data.PROFSAVE.zip", profSaveZip);
                ZipFile.ExtractToDirectory(profSaveZip, settingsDir, true);
                File.Delete(profSaveZip);
            }

            // 直接赋值给字典，如果键已存在，则更新对应的值，否则则创建
            environmentVariables["EAFreeTrialGame"] = "false";
            environmentVariables["EAAuthCode"] = "OriginPCToken";
            environmentVariables["EALaunchOfflineMode"] = "false";
            environmentVariables["OriginSessionKey"] = "7102090b-ea9a-4531-9598-b2a7e943b544";
            environmentVariables["EAGameLocale"] = "zh_TW";
            environmentVariables["EALaunchEnv"] = "production";
            environmentVariables["EALaunchEAID"] = "battlefield.vip";
            environmentVariables["EALicenseToken"] = "2333";
            environmentVariables["EAEntitlementSource"] = "EA";
            environmentVariables["EAUseIGOAPI"] = "1";
            environmentVariables["EALaunchUserAuthToken"] = "OriginPCToken";
            environmentVariables["EAGenericAuthToken"] = "OriginPCToken";
            environmentVariables["EALaunchCode"] = "unavailable";
            environmentVariables["EARtPLaunchCode"] = EaCrypto.GetRTPHandshakeCode();
            environmentVariables["EALsxPort"] = "3216";
            environmentVariables["EAEgsProxyIpcPort"] = "1705";
            environmentVariables["EASteamProxyIpcPort"] = "1704";
            environmentVariables["EAExternalSource"] = "EA";
            environmentVariables["EASecureLaunchTokenTemp"] = "1001006949032";
            environmentVariables["SteamAppId"] = "1238840";
            environmentVariables["ContentId"] = "1026023";
            environmentVariables["EAConnectionId"] = "1026023";

            var strBuilder = new StringBuilder();

            // 以服务端模式运行
            if (Globals.IsUseServer)
            {
                strBuilder.Append("-mserver -serverInstancePath ./Instance/");
            }
            else
            {
                // 确保离线模式
                strBuilder.Append("-Online.ServiceNameOverride battlefield-1-xone");

                // 是否显示Fps
                if (Globals.IsDrawFps)
                    strBuilder.Append(" -PerfOverlay.DrawFps true");
            }

            // 是否使用Mod
            if (Globals.IsUseMod)
                strBuilder.Append(" -dataPath ./ModData/Marne");

            // 解锁人数限制
            //strBuilder.Append(" -BFServer.RoundMaxPlayerCount 100");
            //strBuilder.Append(" -BFServer.RoundRestartCountdown 100");
            //strBuilder.Append(" -Game.MaxPlayerCount 100");
            //strBuilder.Append(" -Network.MaxClientCount 100");

            var startInfo = new ProcessStartInfo
            {
                UseShellExecute = false,
                FileName = Globals.BF1AppPath,
                WorkingDirectory = Globals.BF1InstallDir,
                Arguments = strBuilder.ToString()
            };

            foreach (var variable in environmentVariables)
            {
                startInfo.EnvironmentVariables[variable.Key] = variable.Value;
            }

            // 最终启动战地1游戏步骤
            await Task.Run(() =>
            {
                Process.Start(startInfo);
            });

            LoggerHelper.Info("启动战地1游戏成功");
            NotifierHelper.Success("启动战地1游戏成功");
        }
        catch (Exception ex)
        {
            LoggerHelper.Error("启动战地1游戏发生异常", ex);
            NotifierHelper.Error("启动战地1游戏发生异常，详情请看日志");
        }
    }
}