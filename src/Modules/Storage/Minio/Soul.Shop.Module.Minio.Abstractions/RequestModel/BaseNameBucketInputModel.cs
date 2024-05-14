using Soul.Shop.Module.Minio.Abstractions.CommonModels;

namespace Soul.Shop.Module.Minio.Abstractions.RequestModel;

public class BaseNameBucketInputModel : BaseModel
{
    public string? NameBucket { get; set; }
}
