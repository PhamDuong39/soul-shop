using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Soul.Shop.Module.Minio.Abstractions.Attribute;
using Soul.Shop.Module.Minio.Abstractions.CommonModels;
using Soul.Shop.Module.Minio.Abstractions.RequestModel;
using Soul.Shop.Module.Minio.Abstractions.Service;

namespace Soul.Shop.Module.Minio.Controller;

[Route("api/micro-service-file-manager")]
[ApiController]
public class FileManagerController(IFileManagerService fileManagerService) : ControllerBase
{
    [HttpPost("list-buckets")]
    public async Task<IActionResult> ListBucketsAsync(BaseModel baseModel)
    {
        var result = await fileManagerService.ListBucketsAsync();
        if (result.Success)
        {
            return Ok(result);
        }

        return result.StatusCode switch
        {
            400 => BadRequest(result),
            401 => BadRequest(result),
            403 => BadRequest(result),
            404 => NotFound(result),
        };
    }

    [HttpPost("buckets/create-new-bucket")]
    //[VerifyMacAttributes<BaseNameBucketInputModel>]
    public async Task<IActionResult> CreateBucketAsync(BaseNameBucketInputModel inputModel)
    {
        var result = await fileManagerService.CreateBucketAsync(inputModel);
        if (result.Success)
        {
            return Ok(result);
        }

        return result.StatusCode switch
        {
            400 => BadRequest(result),
            401 => BadRequest(result),
            403 => BadRequest(result),
            404 => NotFound(result),
        };
    }

    [HttpPost("buckets/create-or-upload")]
    [MultipartFormData]
    // [DisableFormValueModelBinding]
    [RequestFormLimits(ValueLengthLimit = int.MaxValue, MultipartBodyLengthLimit = int.MaxValue)]
    //[VerifyMacAttributes<CreateOrUploadBucketInputModel>]
    public async Task<IActionResult> CreateBucketAndPushFileOrListFileWithLocationAsync(
        [FromForm] CreateOrUploadBucketInputModel inputModel, [FromForm] IFormFileCollection fileCollection)
    {
        var result =
            await fileManagerService.CreateOrUploadBucketAndPushFileOrListFileWithLocationAsync(inputModel,
                fileCollection);
        if (result.Success)
        {
            return Ok(result);
        }

        return result.StatusCode switch
        {
            400 => BadRequest(result),
            401 => BadRequest(result),
            403 => BadRequest(result),
            404 => NotFound(result),
        };
    }

    [HttpPost("buckets/exist")]
    //[VerifyMacAttributes<BaseNameBucketInputModel>]
    public async Task<IActionResult> BucketExistsAsync(BaseNameBucketInputModel inputModel)
    {
        var result = await fileManagerService.BucketExistsAsync(inputModel);
        if (result.Success)
        {
            return Ok(result);
        }

        return result.StatusCode switch
        {
            400 => BadRequest(result),
            401 => BadRequest(result),
            403 => BadRequest(result),
            404 => NotFound(result),
        };
    }

    [HttpPost("buckets/remove")]
    //[VerifyMacAttributes<BaseNameBucketInputModel>]
    public async Task<IActionResult> RemoveBucketAsync(BaseNameBucketInputModel inputModel)
    {
        var result = await fileManagerService.RemoveBucketAsync(inputModel);
        if (result.Success)
        {
            return Ok(result);
        }

        return result.StatusCode switch
        {
            400 => BadRequest(result),
            401 => BadRequest(result),
            403 => BadRequest(result),
            404 => NotFound(result),
        };
    }

    [HttpPost("buckets/objects")]
    //[VerifyMacAttributes<BaseNameBucketInputModel>]
    public async Task<IActionResult> ListObjectsAsync(BaseNameBucketInputModel inputModel)
    {
        var result = await fileManagerService.ListObjectsAsync(inputModel);
        if (result.Success)
        {
            return Ok(result);
        }

        return result.StatusCode switch
        {
            400 => BadRequest(result),
            401 => BadRequest(result),
            403 => BadRequest(result),
            404 => NotFound(result),
        };
    }

    [HttpPost("buckets/objects/remove")]
    //[VerifyMacAttributes<BaseNameBucketInputModel>]
    public async Task<IActionResult> RemoveFilesInBucket(BaseNameBucketInputModel inputModel)
    {
        var result = await fileManagerService.RemoveAllObjectInBucketAsync(inputModel);
        if (result.Success)
        {
            return Ok(result);
        }

        return result.StatusCode switch
        {
            400 => BadRequest(result),
            401 => BadRequest(result),
            403 => BadRequest(result),
            404 => NotFound(result),
        };
    }

    [HttpPost("buckets/objects/remove-an-object")]
    //[VerifyMacAttributes<BaseNameFileInBucketInputModel>]
    public async Task<IActionResult> RemoveFileInBucket(BaseNameFileInBucketInputModel inputModel)
    {
        var result = await fileManagerService.RemoveFileInBucket(inputModel);
        if (result.Success)
        {
            return Ok(result);
        }

        return result.StatusCode switch
        {
            400 => BadRequest(result),
            401 => BadRequest(result),
            403 => BadRequest(result),
            404 => NotFound(result),
        };
    }

    [HttpPost("buckets/objects/status")]
    // //[VerifyMacAttributes<BaseNameFileInBucketInputModel>]
    public async Task<IActionResult> CheckObjectStatus(BaseNameFileInBucketInputModel inputModel)
    {
        var result = await fileManagerService.CheckObjectStatus(inputModel);
        if (result.Success)
        {
            return Ok(result);
        }

        return result.StatusCode switch
        {
            400 => BadRequest(result),
            401 => BadRequest(result),
            403 => BadRequest(result),
            404 => NotFound(result),
        };
    }

    [HttpPost("buckets/objects/download")]
    //[VerifyMacAttributes<BaseNameFileInBucketInputModel>]
    public async Task<IActionResult> DownloadObjectFromBucket(BaseNameFileInBucketInputModel inputModel)
    {
        var document = await fileManagerService.DownloadObjectFromBucket(inputModel);
        var provider = new FileExtensionContentTypeProvider();

        if (!provider.TryGetContentType(inputModel.FileName, out var contentType))
        {
            contentType = "application/octet-stream";
        }

        if (document.Success)
        {
            return File(document.Data, contentType, inputModel.FileName);
        }

        return document.StatusCode switch
        {
            400 => BadRequest(document),
            401 => BadRequest(document),
            403 => BadRequest(document),
            404 => NotFound(document),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    [HttpPost("buckets/objects/download/zipped")]
    //[VerifyMacAttributes<DownLoadZippedInputModel>]
    public async Task<IActionResult> DownloadPathFromBucket(DownLoadZippedInputModel inputModel)
    {
        var document = await fileManagerService.DownloadPathFromBucket(inputModel);

        var provider = new FileExtensionContentTypeProvider();
        if (!provider.TryGetContentType("zip", out var contentType))
        {
            contentType = "application/octet-stream";
        }

        return File(document.Data, contentType, $"{inputModel.Path}.zip");
    }

    [HttpPost("buckets/objects/check-file-exist")]
    ////[VerifyMacAttributes<BaseNameFileInBucketInputModel>]
    public async Task<IActionResult> CheckFileExistInBucket(BaseNameFileInBucketInputModel inputModel)
    {
        var result = await fileManagerService.CheckFileExistInBucket(inputModel);
        if (result.Success)
        {
            return Ok(result);
        }

        return result.StatusCode switch
        {
            400 => BadRequest(result),
            401 => BadRequest(result),
            403 => BadRequest(result),
        };
    }

    [HttpPost("buckets/objects/share-link")]
    //[VerifyMacAttributes<ShortLinkInputModel>]
    public async Task<IActionResult> ShareLink(ShortLinkInputModel inputModel)
    {
        var result = await fileManagerService.LinkFileToBucketAsync(inputModel);
        if (result.Success)
        {
            return Ok(result);
        }

        return result.StatusCode switch
        {
            400 => BadRequest(result),
        };
    }

    [HttpPost("buckets/objects/check-object-lifecycle")]
    //[VerifyMacAttributes<BaseNameFileInBucketInputModel>]
    public async Task<IActionResult> CheckObjectLifeCycle(BaseNameFileInBucketInputModel inputModel)
    {
        var result = await fileManagerService.CheckObjectLifeCycleAsync(inputModel);
        if (result.Success)
        {
            return Ok(result);
        }

        return result.StatusCode switch
        {
            400 => BadRequest(result),
        };
    }

    [HttpPost("buckets/objects/check-bucket-lifecycle")]
    //[VerifyMacAttributes<BaseNameFileInBucketInputModel>]
    public async Task<IActionResult> CheckBucketLifeCycle(BaseNameBucketInputModel inputModel)
    {
        var result = await fileManagerService.CheckBucketLifeCycleAsync(inputModel);
        if (result.Success)
        {
            return Ok(result);
        }

        return result.StatusCode switch
        {
            400 => BadRequest(result),
        };
    }

    [HttpPost("buckets/objects/set-object-lifecycle")]
    //[VerifyMacAttributes<BaseNameFileInBucketInputModel>]
    public async Task<IActionResult> SetObjectLifeCycle(SetLifeCycleBucketInputModel inputModel)
    {
        var result = await fileManagerService.SetObjectLifeCycleAsync(inputModel);
        if (result.Success)
        {
            return Ok(result);
        }

        return result.StatusCode switch
        {
            400 => BadRequest(result),
        };
    }

    [HttpPost("buckets/objects/previews")]
    //[VerifyMacAttributes<PreviewFileInputModel>]
    public async Task<IActionResult> PreviewFile(PreviewFileInputModel inputModel)
    {
        var result = await fileManagerService.PreviewFileAsync(inputModel);
        if (!result.Success)
            return result.StatusCode switch
            {
                400 => BadRequest(result),
            };
        if (result is not null)
        {
            return Ok(result);
        }

        return result.StatusCode switch
        {
            400 => BadRequest(result),
        };
    }

    [HttpPost("buckets/objects/list-previews")]
    //[VerifyMacAttributes<ListPreviewsInputModel>]
    public async Task<IActionResult> ListPreviews(ListPreviewsInputModel inputModel)
    {
        var result = await fileManagerService.ListPreviewsAsync(inputModel);
        if (!result.Success)
            return result.StatusCode switch
            {
                400 => BadRequest(result),
            };
        if (result is not null)
        {
            return Ok(result);
        }

        return result.StatusCode switch
        {
            400 => BadRequest(result),
        };
    }

    // [HttpPost]
    // public async Task<IActionResult> GetMacVerify(ListPreviewsInputModel inputModel)
    // {
    //
    //     var result = await fileManagerService.GetMacVerify(inputModel);
    //     if (!result.Success)
    //         return result.StatusCode switch
    //         {
    //             400 => BadRequest(result),
    //
    //         };
    //     if (result is not null)
    //     {
    //         return Ok(result);
    //     }
    //
    //     return result.StatusCode switch
    //     {
    //         400 => BadRequest(result),
    //
    //     };
    // }
}
