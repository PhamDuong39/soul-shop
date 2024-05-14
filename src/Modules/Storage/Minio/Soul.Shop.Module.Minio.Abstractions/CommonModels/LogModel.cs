namespace Soul.Shop.Module.Minio.Abstractions.CommonModels;

public class LogModel
{
    //Thời gian thực hiện log, GMT 0 
    public string? Timestamp { get; set; }
    // Full data mà mình log
    public string? FullData { get; set; }
    //Ipv4 của server
    public string? SourceIp { get; set; }
    //Tên của service
    public string? ServiceName { get; set; }
    // Level log
    public string? Level { get; set; }
    public ContextMap? ContextMap { get; set; }
    //log time theo GMT + 7
    public string? CustomTimestamp { get; set; }
}

public class ContextMap
{
    //ID của process
    public string? ClientMessageId { get; set; }
    //httprequest hoặc httpresponse
    public string? LogType { get; set; }
    //tổng thời gian xử lý khi logType = httpresponse, đơn vị millisecond làm tròn 3 chữ số sau dấu chấm
    public string? Duration { get; set; }
}
