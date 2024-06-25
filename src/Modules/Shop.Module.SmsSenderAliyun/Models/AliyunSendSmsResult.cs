namespace Shop.Module.SmsSenderAliyun.Models;

public class AliyunSendSmsResult
{
    /// <summary>
    /// Gửi ID biên nhận
    /// Bạn có thể truy vấn trạng thái gửi cụ thể trong giao diện QuerySendDetails dựa trên ID này.
    /// 900619746936498440^0
    /// </summary>
    public string BizId { get; set; }

    /// <summary>
    /// Yêu cầu mã trạng thái.
    /// OK
    /// </summary>
    public string Code { get; set; }

    /// <summary>
    /// Mô tả mã trạng thái。
    /// OK
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// Yêu cầu ID。
    /// F655A8D5-B967-440B-8683-DAD6FF8DE990
    /// </summary>
    public string RequestId { get; set; }
}
