using Soul.Shop.Module.Minio.Abstractions.CommonModels;

namespace Soul.Shop.Module.Minio.Abstractions.RequestModel;

public class ListPreviewsInputModel : BaseModel
{
    public string? NameBucket { get; set; }
    public string? PathFiles { get; set; }
}

public class PathFile
{
    public string? Path { get; set; }
    public string? NameFile { get; set; }
}
