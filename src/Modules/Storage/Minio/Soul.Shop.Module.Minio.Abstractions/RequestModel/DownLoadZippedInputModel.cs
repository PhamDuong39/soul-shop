﻿using Soul.Shop.Module.Minio.Abstractions.CommonModels;

namespace Soul.Shop.Module.Minio.Abstractions.RequestModel;

public class DownLoadZippedInputModel : BaseModel
{
    public string? NameBucket { get; set; }
    public string? Path { get; set; }
}
