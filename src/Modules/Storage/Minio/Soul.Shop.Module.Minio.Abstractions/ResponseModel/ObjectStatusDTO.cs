
namespace Soul.Shop.Module.Minio.Abstractions.ResponseModel;

public class ObjectStatusDTO
{
    public string? ObjectName { get; set; }
    public long Size { get; set; }
    public DateTime LastModified { get; set; }
    public string? ETag { get; set; }
    public string? ContentType { get; set; }
    public Dictionary<string, string>? MetaData { get; set; }
    public string? VersionId { get; set; }
    public bool DeleteMarker { get; set; }
    public uint? TaggingCount { get; set; }
    public string? ArchiveStatus { get; set; }
    public DateTime? Expires { get; set; }
    public string? ReplicationStatus { get; set; }
    public dynamic? ObjectLockMode { get; set; }
    public DateTime? ObjectLockRetainUntilDate { get; set; }
    public bool? LegalHoldEnabled { get; set; }
}
