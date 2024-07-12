namespace Shop.Module.Core.MiniProgram.Models;

public class MiniProgramOptions
{
    /// <summary>
    /// Website assigned mini-program ID. Please remember this change.
    /// </summary>
    public string AppId { get; set; }

    public string AppSecret { get; set; }

    /// <summary>
    /// The merchant number assigned.
    /// </summary>
    public string MchId { get; set; }

    /// <summary>
    /// Merchant API key
    /// </summary>
    public string Key { get; set; }
}
