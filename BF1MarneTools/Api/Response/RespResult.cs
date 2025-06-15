using RestSharp;

namespace BF1MarneTools.Api;

public class RespResult(string apiName)
{
    public string ApiName { get; private set; } = apiName;

    public bool IsSuccess { get; set; }
    public ResponseStatus StatusText { get; set; }
    public HttpStatusCode StatusCode { get; set; }
    public string Content { get; set; }
    public string Exception { get; set; }
}