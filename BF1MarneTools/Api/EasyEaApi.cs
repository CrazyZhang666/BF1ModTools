namespace BF1MarneTools.Api;

public static class EasyEaApi
{
    /// <summary>
    /// 获取 LSX 监听服务所需的许可证 License
    /// </summary>
    public static async Task<string> GetLSXLicense(string email, string password, string requestToken, string contentId)
    {
        var result = await EaApi.GetLSXLicense(email, password, requestToken, contentId);
        if (!result.IsSuccess)
            return string.Empty;

        return result.Content;
    }
}