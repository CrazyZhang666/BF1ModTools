using BF1MarneTools.Helper;
using RestSharp;

namespace BF1MarneTools.Api;

public static class CoreApi
{
    private static readonly RestClient _client;

    static CoreApi()
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
    /// 获取服务器版本信息
    /// </summary>
    public static async Task<Version> GetWebUpdateVersion()
    {
        try
        {
            var request = new RestRequest("https://api.github.com/repos/CrazyZhang666/BF1MarneTools/releases/latest", Method.Get);

            var response = await _client.ExecuteAsync(request);
            LoggerHelper.Info($"GetWebUpdateVersion 请求结束，状态 {response.ResponseStatus}");
            LoggerHelper.Info($"GetWebUpdateVersion 请求结束，状态码 {response.StatusCode}");

            if (response.ResponseStatus == ResponseStatus.TimedOut)
            {
                LoggerHelper.Info($"GetWebUpdateVersion 请求超时");
                return null;
            }

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var jsonNode = JsonNode.Parse(response.Content);

                var tagName = jsonNode["tag_name"].GetValue<string>();
                if (Version.TryParse(tagName, out Version version))
                {
                    LoggerHelper.Info($"获取服务器更新版本号成功 {version}");
                    return version;
                }
            }

            LoggerHelper.Warn($"获取服务器更新版本号失败 {response.Content}");
            return null;
        }
        catch (Exception ex)
        {
            LoggerHelper.Error("获取服务器更新版本号发生异常", ex);
            return null;
        }
    }
}