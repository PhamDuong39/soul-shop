namespace Soul.Shop.Module.Minio.Abstractions.ResponseModel;

public class PathFileReturnResponseModel
{
    public string? NameBucket { get; set; }
    public List<ContentFile>? ContentFiles { get; set; }
    public List<ErrorPathFileReturnResponseModel>? ErrorPathFiles { get; set; }
}

public class ErrorPathFileReturnResponseModel
{
    public string? Name { get; set; }
    public bool? IsExist { get; set; }

    public string? Message { get; set; }
}

public class ContentFile
{
    public string? NameFile { get; set; }
    public string? FileNameExtension { get; set; }
    public string FileContent { get; set; }
}
