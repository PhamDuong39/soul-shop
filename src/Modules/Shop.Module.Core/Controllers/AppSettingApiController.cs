using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Shop.Infrastructure;
using Shop.Infrastructure.Data;
using Shop.Module.Core.Entities;
using Shop.Module.Core.Extensions;
using Shop.Module.Core.Models;
using Shop.Module.Core.Services;

namespace Shop.Module.Core.Controllers;

/// <summary>
/// A backend management controller for handling API requests related to application settings.
/// </summary>
[ApiController]
[Route("api/appsettings")]
[Authorize(Roles = "admin")]
public class AppSettingApiController : ControllerBase
{
    private readonly IRepositoryWithTypedId<AppSetting, string> _appSettingRepository;
    private readonly IConfigurationRoot _configurationRoot;
    private readonly IAppSettingService _appSettingService;
    private readonly IWorkContext _workContext;

    public AppSettingApiController(
        IRepositoryWithTypedId<AppSetting, string> appSettingRepository,
        IConfiguration configuration,
        IAppSettingService appSettingService,
        IWorkContext workContext)
    {
        _appSettingRepository = appSettingRepository;
        _configurationRoot = (IConfigurationRoot)configuration;
        _appSettingService = appSettingService;
        _workContext = workContext;
    }

    /// <summary>
    /// Retrieve the list of application settings.
    /// </summary>
    /// <returns> Represents the operation result as <see cref="Result"/> object </returns>
    [HttpGet]
    public async Task<Result> Get()
    {
        var settings = await _appSettingRepository.Query().Where(x => x.IsVisibleInCommonSettingPage).ToListAsync();
        return Result.Ok(settings.OrderBy(c => c.Module).ThenBy(c => c.FormatType).ThenBy(c => c.Id));
    }

    /// <summary>
    /// Update application settings.
    /// </summary>
    /// <param name="model">Application settings to be updated</param>
    /// <returns>Indicating the result of the operation <see cref="Result"/> object </returns>
    [HttpPut]
    public async Task<Result> Put([FromBody] AppSetting model)
    {
        // Due to involving advanced permissions and affecting system operation, currently only system administrators are allowed to make changes.
        // To be optimized
        var user = await _workContext.GetCurrentOrThrowAsync();
        if (user.Id != (int)UserWithId.System)
            return Result.Fail("You are not a system administrator！You do not have permission to perform this operation.");

        var setting = await _appSettingRepository.Query()
            .FirstOrDefaultAsync(x => x.Id == model.Id && x.IsVisibleInCommonSettingPage);
        if (setting != null)
        {
            if (setting.FormatType == AppSettingFormatType.Json)
            {
                var type = Type.GetType(setting.Type);
                if (type == null) return Result.Fail("Setting type error");
                var obj = JsonConvert.DeserializeObject(model.Value, type);
                if (obj == null) return Result.Fail("Setting parameter exception");
            }

            setting.Value = model.Value;
            var count = await _appSettingRepository.SaveChangesAsync();
            if (count > 0)
            {
                await _appSettingService.ClearCache(setting.Id);
                _configurationRoot.Reload();

                //if (setting.FormatType == AppSettingFormatType.Json)
                //{
                //    // Binding method
                //    // Singleton/Option
                //    // Can bindings be reinjected???
                //    var type = Type.GetType(setting.Type);
                //    if (type == null)
                //    {
                //        return Result.Fail("Configuration type exception");
                //    }
                //    //var obj = type.Assembly.CreateInstance(type.FullName);
                //    var obj = Activator.CreateInstance(type);
                //    //var pis = type.GetType().GetProperties();
                //    //foreach (var pi in pis)
                //    //{
                //    //    obj.GetType().GetField(pi.Name).SetValue(obj, objVal[pi.Name]);
                //    //}
                //    return Result.Ok();
                //}
            }
        }

        return Result.Ok();
    }

    //[HttpPut()]
    //public async Task<Result> Puts([FromBody]IList<AppSetting> model)
    //{
    //    var settings = await _appSettingRepository.Query().Where(x => x.IsVisibleInCommonSettingPage).ToListAsync();
    //    foreach (var item in settings)
    //    {
    //        var vm = model.FirstOrDefault(x => x.Id == item.Id);
    //        if (vm != null)
    //        {
    //            item.Value = vm.Value;
    //        }
    //    }
    //    await _appSettingRepository.SaveChangesAsync();
    //    _configurationRoot.Reload();
    //    return Result.Ok();
    //}
}
