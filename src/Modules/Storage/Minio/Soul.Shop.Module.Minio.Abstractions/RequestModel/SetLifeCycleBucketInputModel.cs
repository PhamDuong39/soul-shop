using System.ComponentModel.DataAnnotations;

namespace Soul.Shop.Module.Minio.Abstractions.RequestModel;

public class SetLifeCycleBucketInputModel : BaseNameBucketInputModel
{
    public string Status { get; set; } = "Enabled";
    [Range(1,Int32.MaxValue)]
    public int? Days { get; set; }
    public string? Prefix { get; set; }
    public string? Tag { get; set; }
}
