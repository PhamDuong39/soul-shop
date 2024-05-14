using Soul.Shop.Module.Minio.Abstractions.CommonModels;

namespace Soul.Shop.Module.Minio.Abstractions.RequestModel;

public class BaseNameFileInBucketInputModel : BaseModel
{
    public string? BucketName { get; set; }
    public string? FileName { get; set; }
    public string? Path { get; set; }
}
