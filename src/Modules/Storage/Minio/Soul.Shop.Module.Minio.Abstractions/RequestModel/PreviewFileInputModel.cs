using Soul.Shop.Module.Minio.Abstractions.CommonModels;

namespace Soul.Shop.Module.Minio.Abstractions.RequestModel;

public class PreviewFileInputModel : BaseModel
{
    //mac
    public string? NameBucket { get; set; }
    public string? Path { get; set; }
    public string? NameFile { get; set; }
}
