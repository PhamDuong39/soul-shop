namespace Shop.Module.SmsSenderAliyun.Models;

public class AliyunSmsOptions
{
    /// <summary>
    /// ID khu vực
    /// default
    /// cn-hangzhou
    /// </summary>
    public string RegionId { get; set; }

    /// <summary>
    /// AccessKey của tài khoản RAM ID
    /// </summary>
    public string AccessKeyId { get; set; }

    /// <summary>
    /// Tài khoản RAM Truy cập Key Secret
    /// </summary>
    public string AccessKeySecret { get; set; }

    /// <summary>
    /// Đây có phải là tin nhắn thử nghiệm không?
    /// </summary>
    public bool IsTest { get; set; }
}
