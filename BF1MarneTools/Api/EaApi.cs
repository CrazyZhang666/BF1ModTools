using BF1MarneTools.Core;
using BF1MarneTools.Helper;
using RestSharp;

namespace BF1MarneTools.Api;

public static class EaApi
{
    private static readonly RestClient _client;

    static EaApi()
    {
        var options = new RestClientOptions()
        {
            Timeout = TimeSpan.FromSeconds(20),
            FollowRedirects = false,
            ThrowOnAnyError = false,
            ThrowOnDeserializationError = false
        };

        _client = new RestClient(options);
    }

    /// <summary>
    /// 获取LSX游戏许可证
    /// </summary>
    public static async Task<RespResult> GetLSXLicense(string email, string password, string requestToken, string contentId)
    {
        var respResult = new RespResult("GetLSXLicense Api");

        try
        {
            var request = new RestRequest("https://proxy.novafusion.ea.com/licenses", Method.Get);

            request.AddParameter("ea_email", email);
            request.AddParameter("ea_password", password);
            request.AddParameter("requestToken", requestToken);
            request.AddParameter("contentId", contentId);
            request.AddParameter("machineHash", "1");
            request.AddParameter("requestType", "0");

            request.AddHeader("User-Agent", "EACTransaction");
            request.AddHeader("X-Requester-Id", "Origin Online Activation");

            var response = await _client.ExecuteAsync(request);
            LoggerHelper.Info($"{respResult.ApiName} 请求结束，状态 {response.ResponseStatus}");
            LoggerHelper.Info($"{respResult.ApiName} 请求结束，状态码 {response.StatusCode}");

            respResult.StatusText = response.ResponseStatus;
            respResult.StatusCode = response.StatusCode;
            respResult.Content = response.Content;

            if (response.ResponseStatus == ResponseStatus.TimedOut)
            {
                LoggerHelper.Info($"{respResult.ApiName} 请求超时");
                return respResult;
            }

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var decryptStr = EaCrypto.Decrypt(response.RawBytes).Replace("", "");
                var decryptArray = decryptStr.Split(EaCrypto.TokenSeparator, StringSplitOptions.RemoveEmptyEntries);

                if (!string.IsNullOrWhiteSpace(decryptArray[1]))
                {
                    respResult.Content = decryptArray[1];
                    LoggerHelper.Debug($"{respResult.ApiName} 获取 License 成功 {respResult.Content}");

                    respResult.IsSuccess = true;
                }
                else
                {
                    LoggerHelper.Warn($"{respResult.ApiName} 获取 License 失败");
                }
            }
            else
            {
                LoggerHelper.Info($"{respResult.ApiName} 请求失败，返回结果 {response.Content}");
            }
        }
        catch (Exception ex)
        {
            respResult.Exception = ex.Message;
            LoggerHelper.Error($"{respResult.ApiName} 请求异常", ex);
        }

        return respResult;
    }
}