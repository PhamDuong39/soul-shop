using System.Collections.ObjectModel;
using System.IO.Compression;
using System.Reactive.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel;
using Minio.DataModel.Args;
using Minio.DataModel.ILM;
using Minio.DataModel.Result;
using Minio.Exceptions;
using Newtonsoft.Json;
using Serilog.Events;
using Soul.Shop.Module.Minio.Abstractions;
using Soul.Shop.Module.Minio.Abstractions.Extensions;
using Soul.Shop.Module.Minio.Abstractions.Options;
using Soul.Shop.Module.Minio.Abstractions.RequestModel;
using Soul.Shop.Module.Minio.Abstractions.ResponseModel;
using Soul.Shop.Module.Minio.Abstractions.Service;

namespace Soul.Shop.Module.Minio;

public class FileManagerService(
    IOptions<MinIoConfigOption> minIoConfigOption,
    MinioClient minioClient,
    ILogger<FileManagerService> logger,
    IOptions<SettingCornJob> settingCornJobOptions,
    IOptions<PreviewOption> previewOption)
    : IFileManagerService
{
    private readonly MinIoConfigOption _minIoConfigOption = minIoConfigOption.Value;
    private const string ClassName = nameof(FileManagerService);
    private readonly MinioClient _minioClient = minioClient;
    private readonly ILogger<FileManagerService> _logger = logger;

    private readonly SettingCornJob _settingCornJobOptions = settingCornJobOptions.Value;

    //call option setting corn job
    private readonly PreviewOption _previewOption = previewOption.Value;

    public async Task<BaseResponseModel<ListAllMyBucketsResult>> ListBucketsAsync()
    {
        try
        {
            var progress = new Progress<ProgressReport>(progressReport =>
            {
                _logger.LogInformation(
                    $"Percentage: {progressReport.Percentage}% TotalBytesTransferred: {progressReport.TotalBytesTransferred} bytes"
                        .GeneratedLog(ClassName, LogEventLevel.Information));
                if (progressReport.Percentage != 100)
                    Console.SetCursorPosition(0, Console.CursorTop - 1);
                else Console.WriteLine("Completed");
            });

            _logger.LogInformation("Get list buckets success".GeneratedLog(ClassName, LogEventLevel.Information));
            return new BaseResponseModel<ListAllMyBucketsResult>()
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Get list bucket success",
                Success = true,
                Data = await _minioClient.ListBucketsAsync()
            };
        }
        catch (Exception e)
        {
            _logger.LogCritical(
                $"{MethodBase.GetCurrentMethod()?.Name} Throw exception: {e.Message} || Stack trace: {e.StackTrace}"
                    .GeneratedLog(ClassName, LogEventLevel.Fatal));
            return new BaseResponseModel<ListAllMyBucketsResult>
            {
                StatusCode = 500, Message = e.Message, Success = false
            };
        }
    }

    public async Task<BaseResponseModel<string>> CreateBucketAsync(BaseNameBucketInputModel inputModel)
    {
        try
        {
            var progress = new Progress<ProgressReport>(progressReport =>
            {
                _logger.LogInformation(
                    $"Percentage: {progressReport.Percentage}% TotalBytesTransferred: {progressReport.TotalBytesTransferred} bytes"
                        .GeneratedLog(ClassName, LogEventLevel.Information));
                if (progressReport.Percentage != 100)
                    Console.SetCursorPosition(0, Console.CursorTop - 1);
                else Console.WriteLine("Completed");
            });

            var bucketExistsArgs = new BucketExistsArgs().WithBucket(inputModel.NameBucket);
            var bucketExists = await _minioClient.BucketExistsAsync(bucketExistsArgs);
            if (bucketExists)
                return new BaseResponseModel<string>()
                {
                    StatusCode = StatusCodes.Status400BadRequest, Message = "Bucket name is exists", Success = false
                };
            _logger.LogInformation("Tạo bucket".GeneratedLog(ClassName, LogEventLevel.Information));
            var makeBucketArgs = new MakeBucketArgs().WithBucket(inputModel.NameBucket);
            await _minioClient.MakeBucketAsync(makeBucketArgs);
            return new BaseResponseModel<string>()
            {
                StatusCode = StatusCodes.Status200OK, Message = "Create bucket success", Success = true
            };
        }
        catch (Exception e)
        {
            _logger.LogCritical(
                $"{MethodBase.GetCurrentMethod()?.Name} Throw exception: {e.Message} || Stack trace: {e.StackTrace}"
                    .GeneratedLog(ClassName, LogEventLevel.Fatal));
            return new BaseResponseModel<string> { StatusCode = 500, Message = e.Message, Success = false };
        }
    }

    public async Task<BaseResponseModel<string>> CreateOrUploadBucketAndPushFileOrListFileWithLocationAsync(
        CreateOrUploadBucketInputModel inputModel, IFormFileCollection fileCollection
    )
    {
        try
        {
            var progress = new Progress<ProgressReport>(progressReport =>
            {
                _logger.LogInformation(
                    $"Percentage: {progressReport.Percentage}% TotalBytesTransferred: {progressReport.TotalBytesTransferred} bytes"
                        .GeneratedLog(ClassName, LogEventLevel.Information));
                if (progressReport.Percentage != 100)
                    Console.SetCursorPosition(0, Console.CursorTop - 1);
                else Console.WriteLine("Completed");
            });


            #region check param and set default value

            var nameBucket = inputModel.BucketName;
            var path = inputModel.Path;
            if (string.IsNullOrEmpty(nameBucket) || string.IsNullOrWhiteSpace(nameBucket))
                nameBucket = _minIoConfigOption.DefaultBucket;

            if (string.IsNullOrEmpty(path) || string.IsNullOrWhiteSpace(path))
                path = _minIoConfigOption.DefaultLocation;

            if (fileCollection.Count == 0)
                return new BaseResponseModel<string>()
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "File collection is empty",
                    Success = false
                };

            #endregion

            //Check bucket exists
            var bucketExistsArgs = new BucketExistsArgs().WithBucket(nameBucket);
            var bucketExists = await _minioClient.BucketExistsAsync(bucketExistsArgs);

            if (!bucketExists)
            {
                _logger.LogInformation("Tạo bucket".GeneratedLog(ClassName, LogEventLevel.Information));
                //Create bucket
                var makeBucketArgs = new MakeBucketArgs().WithBucket(nameBucket);
                await _minioClient.MakeBucketAsync(makeBucketArgs);
            }

            foreach (var file in fileCollection)
            {
                byte[] content = await ReadBytesFile(file);
                await UploadAsync(nameBucket, file.FileName, content, path);
            }

            return new BaseResponseModel<string>()
            {
                StatusCode = StatusCodes.Status200OK, Message = "Create bucket success", Success = true
            };
        }
        catch (Exception e)
        {
            _logger.LogCritical(
                $"{MethodBase.GetCurrentMethod()?.Name} Throw exception: {e.Message} || Stack trace: {e.StackTrace}"
                    .GeneratedLog(ClassName, LogEventLevel.Fatal));
            return new BaseResponseModel<string> { StatusCode = 500, Message = e.Message, Success = false };
        }
    }

    private async Task UploadAsync(string? nameBucket, string fileFileName, byte[] content, string? path)
    {
        _logger.LogInformation("Upload file".GeneratedLog(ClassName, LogEventLevel.Information));
        using var fileStream = new MemoryStream(content);
        var objectName = string.IsNullOrEmpty(path) ? fileFileName : $"{path}/{fileFileName}";
        var putObjectArgs = new PutObjectArgs()
            .WithBucket(nameBucket)
            .WithObject(objectName)
            .WithStreamData(fileStream)
            .WithObjectSize(fileStream.Length)
            .WithContentType("application/octet-stream");
        await _minioClient.PutObjectAsync(putObjectArgs);
    }

    private async Task<byte[]> ReadBytesFile(IFormFile file)
    {
        using var ms = new MemoryStream();
        await file.CopyToAsync(ms);
        var fileBytes = ms.ToArray();
        return fileBytes;
    }

    public async Task<BaseResponseModel<bool>> BucketExistsAsync(BaseNameBucketInputModel inputModel)
    {
        try
        {
            var nameBucket = inputModel.NameBucket;
            var progress = new Progress<ProgressReport>(progressReport =>
            {
                _logger.LogInformation(
                    $"Percentage: {progressReport.Percentage}% TotalBytesTransferred: {progressReport.TotalBytesTransferred} bytes"
                        .GeneratedLog(ClassName, LogEventLevel.Information));
                if (progressReport.Percentage != 100)
                    Console.SetCursorPosition(0, Console.CursorTop - 1);
                else Console.WriteLine("Completed");
            });

            _logger.LogInformation("Kiểm tra bucket".GeneratedLog(ClassName, LogEventLevel.Information));
            var bucketExistsArgs = new BucketExistsArgs().WithBucket(nameBucket);
            return new BaseResponseModel<bool>()
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Check bucket success",
                Success = true,
                Data = await _minioClient.BucketExistsAsync(bucketExistsArgs)
            };
        }
        catch (Exception e)
        {
            _logger.LogCritical(
                $"{MethodBase.GetCurrentMethod()?.Name} Throw exception: {e.Message} || Stack trace: {e.StackTrace}"
                    .GeneratedLog(ClassName, LogEventLevel.Fatal));
            return new BaseResponseModel<bool> { StatusCode = 500, Message = e.Message, Success = false };
        }
    }

    public async Task<BaseResponseModel<string>> RemoveBucketAsync(BaseNameBucketInputModel inputModel)
    {
        try
        {
            var nameBucket = inputModel.NameBucket;
            var progress = new Progress<ProgressReport>(progressReport =>
            {
                _logger.LogInformation(
                    $"Percentage: {progressReport.Percentage}% TotalBytesTransferred: {progressReport.TotalBytesTransferred} bytes"
                        .GeneratedLog(ClassName, LogEventLevel.Information));
                if (progressReport.Percentage != 100)
                    Console.SetCursorPosition(0, Console.CursorTop - 1);
                else Console.WriteLine("Completed");
            });

            //Check bucket exists
            var bucketExistsArgs = new BucketExistsArgs().WithBucket(nameBucket);
            var bucketExists = await _minioClient.BucketExistsAsync(bucketExistsArgs);
            if (!bucketExists)
                return new BaseResponseModel<string>()
                {
                    StatusCode = StatusCodes.Status404NotFound, Message = "Bucket not exists", Success = false
                };
            _logger.LogInformation("Xóa bucket".GeneratedLog(ClassName, LogEventLevel.Information));
            if (nameBucket != null)
            {
                await RemoveAllObjectInBucketAsync(inputModel);
                var removeBucketArgs = new RemoveBucketArgs().WithBucket(nameBucket);
                await _minioClient.RemoveBucketAsync(removeBucketArgs).ConfigureAwait(false);
            }

            return new BaseResponseModel<string>()
            {
                StatusCode = StatusCodes.Status200OK, Message = "Remove bucket success", Success = true
            };
        }
        catch (Exception e)
        {
            _logger.LogCritical(
                $"{MethodBase.GetCurrentMethod()?.Name} Throw exception: {e.Message} || Stack trace: {e.StackTrace}"
                    .GeneratedLog(ClassName, LogEventLevel.Fatal));
            return new BaseResponseModel<string> { StatusCode = 500, Message = e.Message, Success = false };
        }
    }

    private async Task<object> RemoveObjectInBucketAsync(string itemKey, string nameBucket)
    {
        try
        {
            _logger.LogInformation("Xóa file".GeneratedLog(ClassName, LogEventLevel.Information));
            var putObjectArgs = new RemoveObjectArgs()
                .WithBucket(nameBucket)
                .WithObject(itemKey);
            await _minioClient.RemoveObjectAsync(putObjectArgs).ConfigureAwait(false);
            Console.WriteLine("Successfully Removed " + itemKey);
            return new BaseResponseModel<string>()
            {
                StatusCode = StatusCodes.Status200OK, Message = "Remove file success", Success = true
            };
        }
        catch (MinioException e)
        {
            _logger.LogError(e, "Xóa file thất bại".GeneratedLog(ClassName, LogEventLevel.Error));
            throw;
        }
    }

    public async Task<BaseResponseModel<List<Item>>> ListObjectsAsync(BaseNameBucketInputModel inputModel)
    {
        try
        {
            var nameBucket = inputModel.NameBucket;
            var progress = new Progress<ProgressReport>(progressReport =>
            {
                _logger.LogInformation(
                    $"Percentage: {progressReport.Percentage}% TotalBytesTransferred: {progressReport.TotalBytesTransferred} bytes"
                        .GeneratedLog(ClassName, LogEventLevel.Information));
                if (progressReport.Percentage != 100)
                    Console.SetCursorPosition(0, Console.CursorTop - 1);
                else Console.WriteLine("Completed");
            });

            //Check bucket exists
            var bucketExistsArgs = new BucketExistsArgs().WithBucket(nameBucket);
            var bucketExists = await _minioClient.BucketExistsAsync(bucketExistsArgs);
            if (!bucketExists)
                return new BaseResponseModel<List<Item>>()
                {
                    StatusCode = StatusCodes.Status404NotFound, Message = "Bucket not exists", Success = false
                };
            _logger.LogInformation(
                $"Lấy danh sách object từ bucket : {nameBucket}".GeneratedLog(ClassName, LogEventLevel.Information));
            return new BaseResponseModel<List<Item>>()
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Get list object success",
                Success = true,
                Data = (List<Item>)await _minioClient.ListObjectsAsync(new ListObjectsArgs().WithBucket(nameBucket))
                    .ToList()
            };
        }
        catch (Exception e)
        {
            _logger.LogCritical(
                $"{MethodBase.GetCurrentMethod()?.Name} Throw exception: {e.Message} || Stack trace: {e.StackTrace}"
                    .GeneratedLog(ClassName, LogEventLevel.Fatal));
            return new BaseResponseModel<List<Item>> { StatusCode = 500, Message = e.Message, Success = false };
        }
    }

    public async Task<BaseResponseModel<string>> RemoveAllObjectInBucketAsync(BaseNameBucketInputModel inputModel)
    {
        try
        {
            var nameBucket = inputModel.NameBucket;
            var progress = new Progress<ProgressReport>(progressReport =>
            {
                _logger.LogInformation(
                    $"Percentage: {progressReport.Percentage}% TotalBytesTransferred: {progressReport.TotalBytesTransferred} bytes"
                        .GeneratedLog(ClassName, LogEventLevel.Information));
                if (progressReport.Percentage != 100)
                    Console.SetCursorPosition(0, Console.CursorTop - 1);
                else Console.WriteLine("Completed");
            });

            //Check bucket exists
            var bucketExistsArgs = new BucketExistsArgs().WithBucket(nameBucket);
            var bucketExists = await _minioClient.BucketExistsAsync(bucketExistsArgs);
            if (!bucketExists)
                return new BaseResponseModel<string>()
                {
                    StatusCode = StatusCodes.Status404NotFound, Message = "Bucket not exists", Success = false
                };
            _logger.LogInformation("Xóa tất cả file".GeneratedLog(ClassName, LogEventLevel.Information));
            var docs = _minioClient.ListObjectsAsync(new ListObjectsArgs().WithBucket(nameBucket)
                .WithRecursive(true));
            docs.Subscribe(
                async item => await RemoveObjectInBucketAsync(item.Key, nameBucket!),
                ex => Console.WriteLine("OnError: {0}", ex.Message),
                () => Console.WriteLine("OnCompleted"));
            return new BaseResponseModel<string>()
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Remove all object in bucket success",
                Success = true
            };
        }
        catch (Exception e)
        {
            _logger.LogCritical(
                $"{MethodBase.GetCurrentMethod()?.Name} Throw exception: {e.Message} || Stack trace: {e.StackTrace}"
                    .GeneratedLog(ClassName, LogEventLevel.Fatal));
            return new BaseResponseModel<string> { StatusCode = 500, Message = e.Message, Success = false };
        }
    }

    public async Task<BaseResponseModel<string>> RemoveFileInBucket(BaseNameFileInBucketInputModel inputModel)
    {
        try
        {
            var bucketName = inputModel.BucketName;
            var fileName = inputModel.FileName;
            var path = inputModel.Path;

            var progress = new Progress<ProgressReport>(progressReport =>
            {
                _logger.LogInformation(
                    $"Percentage: {progressReport.Percentage}% TotalBytesTransferred: {progressReport.TotalBytesTransferred} bytes"
                        .GeneratedLog(ClassName, LogEventLevel.Information));
                if (progressReport.Percentage != 100)
                    Console.SetCursorPosition(0, Console.CursorTop - 1);
                else Console.WriteLine("Completed");
            });

            var objectName = string.IsNullOrEmpty(path) ? fileName : $"{path}/{fileName}";

            var putObjectArgs = new RemoveObjectArgs()
                .WithBucket(bucketName)
                .WithObject(objectName);
            await _minioClient.RemoveObjectAsync(putObjectArgs).ConfigureAwait(false);
            Console.WriteLine("Successfully Removed " + objectName);
            return new BaseResponseModel<string>()
            {
                StatusCode = StatusCodes.Status200OK, Message = "Remove file success", Success = true
            };
        }
        catch (MinioException e)
        {
            _logger.LogError(e, "Xóa file thất bại".GeneratedLog(ClassName, LogEventLevel.Error));
            throw;
        }
    }


    public async Task<BaseResponseModel<ObjectStatusDTO>> CheckObjectStatus(BaseNameFileInBucketInputModel inputModel)
    {
        try
        {
            var bucketName = inputModel.BucketName;
            var fileName = inputModel.FileName;
            var path = inputModel.Path;


            var progress = new Progress<ProgressReport>(progressReport =>
            {
                _logger.LogInformation(
                    $"Percentage: {progressReport.Percentage}% TotalBytesTransferred: {progressReport.TotalBytesTransferred} bytes"
                        .GeneratedLog(ClassName, LogEventLevel.Information));
                if (progressReport.Percentage != 100)
                    Console.SetCursorPosition(0, Console.CursorTop - 1);
                else Console.WriteLine("Completed");
            });

            var objectName = string.IsNullOrEmpty(path) ? fileName : $"{path}/{fileName}";
            var statObjectArgs = new StatObjectArgs()
                .WithBucket(bucketName)
                .WithObject(objectName);
            var objectReturn = await _minioClient.StatObjectAsync(statObjectArgs);
            var objectStatusDto = new ObjectStatusDTO()
            {
                ObjectName = objectReturn.ObjectName,
                Size = objectReturn.Size,
                LastModified = objectReturn.LastModified,
                ETag = objectReturn.ETag,
                ContentType = objectReturn.ContentType,
                MetaData = objectReturn.MetaData,
                VersionId = objectReturn.VersionId,
                DeleteMarker = objectReturn.DeleteMarker,
                TaggingCount = objectReturn.TaggingCount,
                ArchiveStatus = objectReturn.ArchiveStatus,
                Expires = objectReturn.Expires,
                ReplicationStatus = objectReturn.ReplicationStatus,
                ObjectLockMode = objectReturn.ObjectLockMode,
                ObjectLockRetainUntilDate = objectReturn.ObjectLockRetainUntilDate,
                LegalHoldEnabled = objectReturn.LegalHoldEnabled
            };
            return new BaseResponseModel<ObjectStatusDTO>()
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Check object status success",
                Success = true,
                Data = objectStatusDto
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Kiểm tra file thất bại".GeneratedLog(ClassName, LogEventLevel.Error));
            return new BaseResponseModel<ObjectStatusDTO>()
            {
                StatusCode = StatusCodes.Status500InternalServerError, Message = e.Message, Success = false
            };
        }
    }

    public async Task<BaseResponseModel<MemoryStream>> DownloadObjectFromBucket(
        BaseNameFileInBucketInputModel inputModel)
    {
        try
        {
            var bucketName = inputModel.BucketName;
            var fileName = inputModel.FileName;
            var path = inputModel.Path;

            var progress = new Progress<ProgressReport>(progressReport =>
            {
                _logger.LogInformation(
                    $"Percentage: {progressReport.Percentage}% TotalBytesTransferred: {progressReport.TotalBytesTransferred} bytes"
                        .GeneratedLog(ClassName, LogEventLevel.Information));

                if (progressReport.Percentage != 100)
                    Console.SetCursorPosition(0, Console.CursorTop - 1);
                else Console.WriteLine("Completed");
            });

            var objectName = string.IsNullOrEmpty(path) ? fileName : $"{path}/{fileName}";
            var memoryStream = new MemoryStream();
            var args = new GetObjectArgs()
                .WithBucket(bucketName)
                .WithObject(objectName)
                .WithCallbackStream((stream) => { stream.CopyTo(memoryStream); });
            await _minioClient.GetObjectAsync(args);

            memoryStream.Position = 0;

            if (memoryStream.Length == 0)
                return new BaseResponseModel<MemoryStream>()
                {
                    StatusCode = StatusCodes.Status404NotFound, Message = "File not exists", Success = false
                };
            return new BaseResponseModel<MemoryStream>()
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Download file success",
                Success = true,
                Data = memoryStream
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Download file thất bại".GeneratedLog(ClassName, LogEventLevel.Error));
            return new BaseResponseModel<MemoryStream>()
            {
                StatusCode = StatusCodes.Status500InternalServerError, Message = e.Message, Success = false
            };
        }
    }

    public async Task<BaseResponseModel<MemoryStream>> DownloadPathFromBucket(DownLoadZippedInputModel inputModel)
    {
        try
        {
            var bucketName = inputModel.NameBucket;
            var path = inputModel.Path;
            var progress = new Progress<ProgressReport>(progressReport =>
            {
                _logger.LogInformation(
                    $"Percentage: {progressReport.Percentage}% TotalBytesTransferred: {progressReport.TotalBytesTransferred} bytes");
                if (progressReport.Percentage != 100)
                    Console.SetCursorPosition(0, Console.CursorTop - 1);
                else Console.WriteLine("Completed");
            });
            var memoryStream = new MemoryStream();

            ListObjectsArgs listObjectsArgs = new ListObjectsArgs()
                .WithBucket(bucketName)
                .WithPrefix(path).WithRecursive(true);


            // add files to zip
            using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
            {
                foreach (var item in await _minioClient.ListObjectsAsync(listObjectsArgs).ToList())
                {
                    var objectName = item.Key;
                    var args = new GetObjectArgs()
                        .WithBucket(bucketName)
                        .WithObject(objectName)
                        .WithCallbackStream((stream) =>
                        {
                            var zipArchiveEntry = archive.CreateEntry(objectName, CompressionLevel.Fastest);
                            using var zipStream = zipArchiveEntry.Open();
                            stream.CopyTo(zipStream);
                        });
                    await _minioClient.GetObjectAsync(args);
                }
            }

            memoryStream.Position = 0;

            return new BaseResponseModel<MemoryStream>()
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Download and compression success",
                Success = true,
                Data = memoryStream
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Download and compression failed".GeneratedLog(ClassName, LogEventLevel.Error));
            return new BaseResponseModel<MemoryStream>()
            {
                StatusCode = StatusCodes.Status500InternalServerError, Message = e.Message, Success = false
            };
        }
    }

    public async Task<BaseResponseModel<bool>> CheckFileExistInBucket(BaseNameFileInBucketInputModel inputModel)
    {
        try
        {
            var bucketName = inputModel.BucketName;
            var fileName = inputModel.FileName;
            var path = inputModel.Path;

            var progress = new Progress<ProgressReport>(progressReport =>
            {
                _logger.LogInformation(
                    $"Percentage: {progressReport.Percentage}% TotalBytesTransferred: {progressReport.TotalBytesTransferred} bytes");

                if (progressReport.Percentage != 100)
                    Console.SetCursorPosition(0, Console.CursorTop - 1);
                else Console.WriteLine("Completed");
            });

            var objectName = string.IsNullOrEmpty(path) ? fileName : $"{path}/{fileName}";
            var statObjectArgs = new StatObjectArgs()
                .WithBucket(bucketName)
                .WithObject(objectName);
            var statObject = await _minioClient.StatObjectAsync(statObjectArgs);
            if (statObject.Size != 0)
                return new BaseResponseModel<bool>()
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Check file exist success",
                    Success = true,
                    Data = true
                };
            return new BaseResponseModel<bool>()
            {
                StatusCode = StatusCodes.Status404NotFound,
                Message = "File not exists",
                Success = true,
                Data = false
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Check file exist failed".GeneratedLog(ClassName, LogEventLevel.Error));
            return new BaseResponseModel<bool>()
            {
                StatusCode = StatusCodes.Status500InternalServerError, Message = e.Message, Success = false
            };
        }
    }

    public async Task<BaseResponseModel<string>> LinkFileToBucketAsync(ShortLinkInputModel inputModel)
    {
        try
        {
            var lifeTime = inputModel.ActiveFor?.Day * 24 * 60 * 60 + inputModel.ActiveFor?.Hours * 60 * 60 +
                           inputModel.ActiveFor?.Minutes * 60;
            var resigned = new PresignedGetObjectArgs();

            resigned.WithBucket(inputModel.NameBucket);
            resigned.WithObject(inputModel.Path + "/" + inputModel.NameFile);
            resigned.WithExpiry((int)lifeTime!);

            var sharedUrl = await _minioClient.PresignedGetObjectAsync(resigned);
            _logger.LogInformation("Get url success".GeneratedLog(ClassName, LogEventLevel.Information));
            return new BaseResponseModel<string>()
            {
                StatusCode = StatusCodes.Status200OK, Message = "Get url success", Success = true, Data = sharedUrl
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Get url failed".GeneratedLog(ClassName, LogEventLevel.Error));
            return new BaseResponseModel<string>()
            {
                StatusCode = StatusCodes.Status500InternalServerError, Message = e.Message, Success = false
            };
        }
    }

    public async Task<BaseResponseModel<dynamic>> CheckObjectLifeCycleAsync(BaseNameFileInBucketInputModel inputModel)
    {
        try
        {
            var bucketName = inputModel.BucketName;
            var fileName = inputModel.FileName;
            var path = inputModel.Path;

            var progress = new Progress<ProgressReport>(progressReport =>
            {
                _logger.LogInformation(
                    $"Percentage: {progressReport.Percentage}% TotalBytesTransferred: {progressReport.TotalBytesTransferred} bytes");

                if (progressReport.Percentage != 100)
                    Console.SetCursorPosition(0, Console.CursorTop - 1);
                else Console.WriteLine("Completed");
            });

            var objectName = string.IsNullOrEmpty(path) ? fileName : $"{path}/{fileName}";
            var statObjectArgs = new StatObjectArgs()
                .WithBucket(bucketName)
                .WithObject(objectName);
            var statObject = await _minioClient.StatObjectAsync(statObjectArgs);
            if (statObject.Size != 0)
                return new BaseResponseModel<dynamic>()
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Check object life cycle success" +
                              $"Time expire : {statObject.Expires} day",
                    Success = true,
                };
            return new BaseResponseModel<dynamic>()
            {
                StatusCode = StatusCodes.Status404NotFound, Message = "Object not exists", Success = false,
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Check object life cycle failed".GeneratedLog(ClassName, LogEventLevel.Error));
            return new BaseResponseModel<dynamic>()
            {
                StatusCode = StatusCodes.Status500InternalServerError, Message = e.Message, Success = false
            };
        }
    }

    public async Task<BaseResponseModel<dynamic>> CheckBucketLifeCycleAsync(BaseNameBucketInputModel inputModel)
    {
        try
        {
            var bucketName = inputModel.NameBucket;
            var progress = new Progress<ProgressReport>(progressReport =>
            {
                _logger.LogInformation(
                    $"Percentage: {progressReport.Percentage}% TotalBytesTransferred: {progressReport.TotalBytesTransferred} bytes");

                if (progressReport.Percentage != 100)
                    Console.SetCursorPosition(0, Console.CursorTop - 1);
                else Console.WriteLine("Completed");
            });

            var checkBucketLifeCycleArgs = new GetBucketLifecycleArgs();
            checkBucketLifeCycleArgs.WithBucket(bucketName);
            var checkBucketLifeCycle = await _minioClient.GetBucketLifecycleAsync(checkBucketLifeCycleArgs);
            if (checkBucketLifeCycle != null)
                return new BaseResponseModel<dynamic>()
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Check bucket life cycle success",
                    Success = true,
                    Data = checkBucketLifeCycle
                };
            return new BaseResponseModel<dynamic>();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Check bucket life cycle failed".GeneratedLog(ClassName, LogEventLevel.Error));
            return new BaseResponseModel<dynamic>()
            {
                StatusCode = StatusCodes.Status500InternalServerError, Message = e.Message, Success = false
            };
        }
    }

    public async Task<BaseResponseModel<dynamic>> SetObjectLifeCycleAsync(SetLifeCycleBucketInputModel inputModel)
    {
        var bucketName = inputModel.NameBucket;
        var setBucketLifeCycleArgs = new SetBucketLifecycleArgs();
        setBucketLifeCycleArgs.WithBucket(bucketName);
        var rule = new LifecycleRule
        {
            Status = inputModel.Status,
            Expiration = new Expiration
            {
                Days = inputModel.Days, ExpiryDate = null, ExpiredObjectDeleteMarker = null
            },
            Filter = new RuleFilter { Prefix = inputModel.Prefix, }
        };
        var listRule = new Collection<LifecycleRule> { rule };
        var lifecycleConfiguration = new LifecycleConfiguration { Rules = listRule };
        setBucketLifeCycleArgs.WithLifecycleConfiguration(lifecycleConfiguration);
        var result = _minioClient.SetBucketLifecycleAsync(setBucketLifeCycleArgs);
        Task.WaitAll(result);
        var result1 = result.Status;

        return new BaseResponseModel<dynamic>()
        {
            StatusCode = StatusCodes.Status200OK, Message = "Set object life cycle success", Success = true,
        };
    }

    public async Task<BaseResponseModel<dynamic>> ScanObjectExpiry()
    {
        var listBucket = await _minioClient.ListBucketsAsync();
        var buckets = listBucket.Buckets;
        //check any bucket
        if (buckets.Count == 0)
            return new BaseResponseModel<dynamic>()
            {
                StatusCode = StatusCodes.Status404NotFound,
                Message =
                    $"No one bucket in minio {Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")} environment",
                Success = false,
            };

        var listObjectExpiry = new Dictionary<string, List<string>>();
        List<string> bucketScanner = _settingCornJobOptions.BucketScanner ?? new List<string>();


        foreach (var bucket in buckets)
        {
            if (bucketScanner.Count > 0 && !bucketScanner.Contains(bucket.Name)) continue;
            IList<Item>? folders = null;
            //get folder
            try
            {
                folders = await _minioClient.ListObjectsAsync(new ListObjectsArgs().WithBucket(bucket.Name))
                    .ToList();
                if (folders.Count == 0) continue;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Get folder failed".GeneratedLog(ClassName, LogEventLevel.Error));
                return new BaseResponseModel<dynamic>()
                {
                    StatusCode = StatusCodes.Status500InternalServerError, Message = e.Message, Success = false,
                };
            }

            foreach (var folder in folders)
            {
                //if (!folder.Key.EndsWith("/")) continue;
                IList<Item>? listObject = await _minioClient
                    .ListObjectsAsync(new ListObjectsArgs().WithBucket(bucket.Name).WithPrefix(folder.Key)).ToList();
                if (listObject!.Count <= 0) continue;
                var timeBonusExpire = _settingCornJobOptions.TimeBonusExpire!.GetTime();

                if (timeBonusExpire is 0) timeBonusExpire = 7 * 24 * 60 * 60; //set default 7 days

                //lấy các object có thời gian tạo + 7 ngày lớn hơn thời gian hiện tại
                var listObjectExpiryInBucket = listObject.Where(x => x.LastModifiedDateTime
                        .GetValueOrDefault().AddSeconds(timeBonusExpire) < DateTime.Now)
                    .Select(x => x.Key)
                    .ToList();

                if (listObjectExpiryInBucket.Count > 0)
                    listObjectExpiry.Add(bucket.Name, listObjectExpiryInBucket);

                //remove object expiry
                foreach (var removeObjectArgs in listObjectExpiryInBucket.Select(objectExpiry => new RemoveObjectArgs()
                             .WithBucket(bucket.Name)
                             .WithObject(objectExpiry)))
                {
                    _logger.LogInformation(
                        $"Remove object expiry with name : {removeObjectArgs}".GeneratedLog(ClassName,
                            LogEventLevel.Information));
                    await _minioClient.RemoveObjectAsync(removeObjectArgs).ConfigureAwait(false);
                }

                break;
            }
        }

        return new BaseResponseModel<dynamic>()
        {
            StatusCode = StatusCodes.Status200OK,
            Message = "Scan object expiry success",
            Success = true,
            Data = listObjectExpiry
        };
    }

    public async Task<BaseResponseModel<dynamic>> PreviewFileAsync(PreviewFileInputModel inputModel)
    {
        try
        {
            var fileExtension = Path.GetExtension(inputModel.NameFile);
            fileExtension = fileExtension?.Remove(0, 1);
            if (!_previewOption.FileTypes.Contains(fileExtension))
                return new BaseResponseModel<dynamic>()
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "File type not support, Please contact to admin",
                    Success = false,
                };
            var nameBucket = inputModel.NameBucket;
            var path = inputModel.Path;
            var objectName = string.IsNullOrEmpty(path) ? inputModel.NameFile : $"{path}/{inputModel.NameFile}";
            var memoryStream = new MemoryStream();
            var args = new GetObjectArgs()
                .WithBucket(nameBucket)
                .WithObject(objectName)
                .WithCallbackStream((stream) => { stream.CopyTo(memoryStream); });
            await _minioClient.GetObjectAsync(args);
            memoryStream.Position = 0;
            return new BaseResponseModel<dynamic>()
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Get file success",
                Success = true,
                Data = new
                {
                    FileExtension = fileExtension, FileContent = Convert.ToBase64String(memoryStream.ToArray())
                }
            };
        }
        catch (Exception e)
        {
            return new BaseResponseModel<dynamic>()
            {
                StatusCode = StatusCodes.Status500InternalServerError, Message = e.Message, Success = false
            };
        }
    }

    public async Task<BaseResponseModel<PathFileReturnResponseModel>> ListPreviewsAsync(
        ListPreviewsInputModel inputModel)
    {
        //convert string json to object
        List<PathFile>? pathFiles = null;
        try
        {
            if (string.IsNullOrEmpty(inputModel.PathFiles))
                return new BaseResponseModel<PathFileReturnResponseModel>()
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "Please input path files",
                    Success = false,
                };
            pathFiles = JsonConvert.DeserializeObject<List<PathFile>>(inputModel.PathFiles);
            if (pathFiles == null || pathFiles.Count == 0)
                return new BaseResponseModel<PathFileReturnResponseModel>()
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "Path Files is empty, please try again or can't convert to json",
                    Success = false,
                };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Convert string json to object failed".GeneratedLog(ClassName, LogEventLevel.Error));
            return new BaseResponseModel<PathFileReturnResponseModel>()
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = "Input path files is not json",
                Success = false
            };
        }

        //check null
        try
        {
            if (string.IsNullOrEmpty(inputModel.PathFiles))
                return new BaseResponseModel<PathFileReturnResponseModel>()
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "Path Files is empty, please try again",
                    Success = false,
                };
            //check bucket exists
            var bucketExistsArgs = new BucketExistsArgs().WithBucket(inputModel.NameBucket);
            var bucketExists = await _minioClient.BucketExistsAsync(bucketExistsArgs);
            if (!bucketExists)
                return new BaseResponseModel<PathFileReturnResponseModel>()
                {
                    StatusCode = StatusCodes.Status404NotFound, Message = "Bucket not exists", Success = false,
                };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Check bucket exists failed".GeneratedLog(ClassName, LogEventLevel.Error));
            return new BaseResponseModel<PathFileReturnResponseModel>()
            {
                StatusCode = StatusCodes.Status500InternalServerError, Message = e.Message, Success = false
            };
        }

        //get list object
        var lstReturn = new List<ContentFile>();
        var lstError = new List<ErrorPathFileReturnResponseModel>();
        try
        {
            foreach (var pathFile in pathFiles)
            {
                var pathGetFile = string.IsNullOrEmpty(pathFile.Path)
                    ? pathFile.NameFile
                    : $"{pathFile.Path}/{pathFile.NameFile}";
                var statObjectArgs = new StatObjectArgs()
                    .WithBucket(inputModel.NameBucket)
                    .WithObject(pathGetFile);
                try
                {
                    var statObject = await _minioClient.StatObjectAsync(statObjectArgs);
                    if (statObject.Size == 0)
                    {
                        lstError.Add(new ErrorPathFileReturnResponseModel()
                        {
                            Message = "File not exists", Name = pathFile.NameFile, IsExist = false
                        });
                        lstReturn.Add(new ContentFile()
                        {
                            FileNameExtension = null, NameFile = pathFile.NameFile, FileContent = null
                        });
                    }
                    else
                    {
                        var extensionFile = Path.GetExtension(pathFile.NameFile);
                        extensionFile = extensionFile?.Remove(0, 1);
                        if (!_previewOption.FileTypes.Contains(extensionFile))
                        {
                            lstError.Add(new ErrorPathFileReturnResponseModel()
                            {
                                Message = "File type not support, Please contact to admin",
                                Name = pathFile.NameFile,
                            });
                        }
                        else
                        {
                            var memoryStream = new MemoryStream();
                            var args = new GetObjectArgs()
                                .WithBucket(inputModel.NameBucket)
                                .WithObject(pathGetFile)
                                .WithCallbackStream((stream) => { stream.CopyTo(memoryStream); });
                            await _minioClient.GetObjectAsync(args);
                            memoryStream.Position = 0;
                            lstReturn.Add(new ContentFile()
                            {
                                FileNameExtension = extensionFile,
                                NameFile = pathFile.NameFile,
                                FileContent = Convert.ToBase64String(memoryStream.ToArray())
                            });
                        }
                    }
                }
                catch (ObjectNotFoundException e)
                {
                    lstError.Add(new ErrorPathFileReturnResponseModel()
                    {
                        Message = "File not exists", Name = pathFile.NameFile, IsExist = false
                    });
                }
            }

            return new BaseResponseModel<PathFileReturnResponseModel>()
            {
                Message = "Get list object success",
                StatusCode = StatusCodes.Status200OK,
                Success = true,
                Data = new PathFileReturnResponseModel()
                {
                    NameBucket = inputModel.NameBucket, ContentFiles = lstReturn, ErrorPathFiles = lstError,
                }
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Get list object failed".GeneratedLog(ClassName, LogEventLevel.Error));
            return new BaseResponseModel<PathFileReturnResponseModel>()
            {
                StatusCode = StatusCodes.Status500InternalServerError, Message = e.Message, Success = false
            };
        }
    }

    public Task<BaseResponseModel<dynamic>> GetMacVerify(ListPreviewsInputModel inputModel)
    {
        throw new NotImplementedException();
    }
}
