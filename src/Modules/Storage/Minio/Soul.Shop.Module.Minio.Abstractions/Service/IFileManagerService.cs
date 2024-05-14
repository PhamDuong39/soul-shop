using Microsoft.AspNetCore.Http;
using Minio.DataModel;
using Minio.DataModel.Result;
using Soul.Shop.Module.Minio.Abstractions.RequestModel;
using Soul.Shop.Module.Minio.Abstractions.ResponseModel;

namespace Soul.Shop.Module.Minio.Abstractions.Service;

public interface IFileManagerService
{
    Task<BaseResponseModel<ListAllMyBucketsResult>> ListBucketsAsync();
    Task<BaseResponseModel<string>> CreateBucketAsync(BaseNameBucketInputModel inputModel);

    Task<BaseResponseModel<string>> CreateOrUploadBucketAndPushFileOrListFileWithLocationAsync(
        CreateOrUploadBucketInputModel inputModel, IFormFileCollection fileCollection);

    Task<BaseResponseModel<bool>> BucketExistsAsync(BaseNameBucketInputModel inputModel);
    Task<BaseResponseModel<string>> RemoveBucketAsync(BaseNameBucketInputModel inputModel);
    Task<BaseResponseModel<List<Item>>> ListObjectsAsync(BaseNameBucketInputModel inputModel);
    Task<BaseResponseModel<string>> RemoveAllObjectInBucketAsync(BaseNameBucketInputModel inputModel);
    Task<BaseResponseModel<string>> RemoveFileInBucket(BaseNameFileInBucketInputModel inputModel);
    Task<BaseResponseModel<ObjectStatusDTO>> CheckObjectStatus(BaseNameFileInBucketInputModel inputModel);

    Task<BaseResponseModel<MemoryStream>> DownloadObjectFromBucket(BaseNameFileInBucketInputModel inputModel);

    //download path
    Task<BaseResponseModel<MemoryStream>> DownloadPathFromBucket(DownLoadZippedInputModel inputModel);

    // check file exist
    Task<BaseResponseModel<bool>> CheckFileExistInBucket(BaseNameFileInBucketInputModel inputModel);
    Task<BaseResponseModel<string>> LinkFileToBucketAsync(ShortLinkInputModel inputModel);
    Task<BaseResponseModel<dynamic>> CheckObjectLifeCycleAsync(BaseNameFileInBucketInputModel inputModel);
    Task<BaseResponseModel<dynamic>> CheckBucketLifeCycleAsync(BaseNameBucketInputModel inputModel);
    Task<BaseResponseModel<dynamic>> SetObjectLifeCycleAsync(SetLifeCycleBucketInputModel inputModel);

    Task<BaseResponseModel<dynamic>> ScanObjectExpiry();

    //preview file
    Task<BaseResponseModel<dynamic>> PreviewFileAsync(PreviewFileInputModel inputModel);
    Task<BaseResponseModel<PathFileReturnResponseModel>> ListPreviewsAsync(ListPreviewsInputModel inputModel);
    Task<BaseResponseModel<dynamic>> GetMacVerify(ListPreviewsInputModel inputModel);
}
