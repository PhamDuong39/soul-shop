using Soul.Shop.Module.Minio.Abstractions.CommonModels;

namespace Soul.Shop.Module.Minio.Abstractions.RequestModel;

public class ShortLinkInputModel : BaseModel
{
    public ActiveFor? ActiveFor { get; set; }
    public string? NameBucket { get; set; }
    public string? Path { get; set; }
    public string? NameFile { get; set; }
}

public class ActiveFor
{
    public int? Day { get; set; } = 1;
    public int? Hours { get; set; }
    public int? Minutes { get; set; }
}
