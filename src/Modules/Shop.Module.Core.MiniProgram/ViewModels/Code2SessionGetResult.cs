using Newtonsoft.Json;

namespace Shop.Module.Core.MiniProgram.ViewModels;

public class Code2SessionGetResult
{
    /// <summary>
    /// User unique identifier
    /// </summary>
    [JsonProperty("openid")]
    public string OpenId { get; set; }

    /// <summary>
    /// Session key
    /// </summary>
    [JsonProperty("session_key")]
    public string SessionKey { get; set; }

    /// <summary>
    /// User's unique identifier on the open platform, which will be returned if the conditions for issuing UnionID are met. Refer to the UnionID mechanism documentation for details.
    /// </summary>
    [JsonProperty("unionid")]
    public string UnionId { get; set; }

    /// <summary>
    /// Error code
    /// </summary>
    [JsonProperty("errcode")]
    public int ErrCode { get; set; }

    /// <summary>
    /// Error message
    /// </summary>
    [JsonProperty("errmsg")]
    public string ErrMessage { get; set; }
}
