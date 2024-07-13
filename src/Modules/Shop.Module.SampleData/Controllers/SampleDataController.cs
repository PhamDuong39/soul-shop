using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shop.Infrastructure;
using Shop.Module.SampleData.Services;
using Shop.Module.SampleData.ViewModels;

namespace Shop.Module.SampleData.Controllers;

/// <summary>
/// Example data controller used to manage and manipulate the reset and generation of sample data.
/// </summary>
[Authorize(Roles = "admin")]
[Route("api/sample-data")]
public class SampleDataController : ControllerBase
{
    private readonly ISampleDataService _sampleDataService;
    private readonly IStateOrProvinceService _stateOrProvinceService;

    public SampleDataController(
        ISampleDataService sampleDataService,
        IStateOrProvinceService stateOrProvinceService)
    {
        _sampleDataService = sampleDataService;
        _stateOrProvinceService = stateOrProvinceService;
    }

    /// <summary>
    /// Reset application data to example data. This operation clears the current data and imports predefined example data.
    /// </summary>
    /// <param name="model"> A model containing options for resetting example data. </param>
    /// <returns> Return operation result. On success, return success status code; on failure, return error message. </returns>
    [HttpPost]
    public async Task<Result> ResetToSample([FromBody] SampleDataOption model)
    {
        await _sampleDataService.ResetToSampleData(model);
        return Result.Ok();
    }

    /// <summary>
    /// Generate province/city/district data. This operation is typically used to initialize tables related to addresses.
    /// </summary>
    /// <returns> Return operation result. On success, return success status code; on failure, return error message. </returns>
    [HttpPost("provinces")]
    public async Task<Result> GenPcas()
    {
        await _stateOrProvinceService.GenPcas();
        return Result.Ok();
    }
}
