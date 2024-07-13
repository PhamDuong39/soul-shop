using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Infrastructure;
using Shop.Infrastructure.Data;
using Shop.Infrastructure.Web.StandardTable;
using Shop.Module.Core.Entities;
using Shop.Module.Core.Models;
using Shop.Module.Core.Services;
using Shop.Module.Core.ViewModels;

namespace Shop.Module.Core.Controllers;

/// <summary>
/// The admin backend controller is used to handle API requests related to countries and provinces
/// </summary>
[ApiController]
[Route("api/countries")]
[Authorize(Roles = "admin")]
public class CountryApiController : ControllerBase
{
    private readonly IRepository<Country> _countryRepository;
    private readonly IRepository<StateOrProvince> _provinceRepository;
    private readonly IRepository<Address> _addressRepository;
    private readonly ICountryService _countryService;

    public CountryApiController(
        IRepository<Country> countryRepository,
        IRepository<StateOrProvince> provinceRepository,
        IRepository<Address> addressRepository,
        ICountryService countryService)
    {
        _countryRepository = countryRepository;
        _provinceRepository = provinceRepository;
        _addressRepository = addressRepository;
        _countryService = countryService;
    }

    /// <summary>
    /// Get paginated results of all countries.
    /// </summary>
    /// <param name="param"> pagination parameters </param>
    /// <returns> Indicating the operation result <see cref="Result{T}"/> object, with its values being. <see cref="StandardTableResult{CountryResult}"/> object </returns>
    [HttpPost("grid")]
    public async Task<Result<StandardTableResult<CountryResult>>> List([FromBody] StandardTableParam param)
    {
        var query = _countryRepository.Query();
        var result = await query.Include(x => x.StatesOrProvinces)
            .ToStandardTableResult(param, c => new CountryResult
            {
                Id = c.Id,
                CreatedOn = c.CreatedOn,
                IsCityEnabled = c.IsCityEnabled,
                DisplayOrder = c.DisplayOrder,
                IsBillingEnabled = c.IsBillingEnabled,
                IsDeleted = c.IsDeleted,
                IsDistrictEnabled = c.IsDistrictEnabled,
                IsPublished = c.IsPublished,
                IsShippingEnabled = c.IsShippingEnabled,
                Name = c.Name,
                NumericIsoCode = c.NumericIsoCode,
                ThreeLetterIsoCode = c.ThreeLetterIsoCode,
                TwoLetterIsoCode = c.TwoLetterIsoCode,
                UpdatedOn = c.UpdatedOn,
                StateOrProvinceCount = c.StatesOrProvinces.Count
            });
        return Result.Ok(result);
    }

    /// <summary>
    /// Retrieve country details based on country ID.
    /// </summary>
    /// <param name="id"> Country ID </param>
    /// <returns> Details of the country specified by ID </returns>
    [HttpGet("{id:int:min(1)}")]
    public async Task<Result> Get(int id)
    {
        var country = await _countryRepository.Query()
            .Include(c => c.StatesOrProvinces)
            .Where(c => c.Id == id)
            .Select(c => new CountryResult
            {
                Id = c.Id,
                CreatedOn = c.CreatedOn,
                IsCityEnabled = c.IsCityEnabled,
                DisplayOrder = c.DisplayOrder,
                IsBillingEnabled = c.IsBillingEnabled,
                IsDeleted = c.IsDeleted,
                IsDistrictEnabled = c.IsDistrictEnabled,
                IsPublished = c.IsPublished,
                IsShippingEnabled = c.IsShippingEnabled,
                Name = c.Name,
                NumericIsoCode = c.NumericIsoCode,
                ThreeLetterIsoCode = c.ThreeLetterIsoCode,
                TwoLetterIsoCode = c.TwoLetterIsoCode,
                UpdatedOn = c.UpdatedOn,
                StateOrProvinceCount = c.StatesOrProvinces.Count
            }).FirstOrDefaultAsync();
        if (country == null)
            throw new Exception("Country does not exist");
        return Result.Ok(country);
    }

    /// <summary>
    /// Retrieve a list of all countries
    /// </summary>
    /// <returns> List of all countries </returns>
    [HttpGet()]
    public async Task<Result> Get()
    {
        var countries = await _countryRepository.Query()
            .Include(c => c.StatesOrProvinces)
            .Select(c => new CountryResult
            {
                Id = c.Id,
                CreatedOn = c.CreatedOn,
                IsCityEnabled = c.IsCityEnabled,
                DisplayOrder = c.DisplayOrder,
                IsBillingEnabled = c.IsBillingEnabled,
                IsDeleted = c.IsDeleted,
                IsDistrictEnabled = c.IsDistrictEnabled,
                IsPublished = c.IsPublished,
                IsShippingEnabled = c.IsShippingEnabled,
                Name = c.Name,
                NumericIsoCode = c.NumericIsoCode,
                ThreeLetterIsoCode = c.ThreeLetterIsoCode,
                TwoLetterIsoCode = c.TwoLetterIsoCode,
                UpdatedOn = c.UpdatedOn,
                StateOrProvinceCount = c.StatesOrProvinces.Count
            }).ToListAsync();
        return Result.Ok(countries);
    }

    /// <summary>
    /// Add a new country
    /// </summary>
    /// <param name="model"> Object containing information of the new country </param>
    /// <returns> Operation result </returns>
    [HttpPost]
    public async Task<Result> Post([FromBody] CountryCreateParam model)
    {
        var any = _countryRepository.Query().Any(c => c.NumericIsoCode == model.NumericIsoCode
                                                      || c.TwoLetterIsoCode == model.TwoLetterIsoCode
                                                      || c.ThreeLetterIsoCode == model.ThreeLetterIsoCode);
        if (any) return Result.Fail("Country code already exists");
        var country = new Country()
        {
            DisplayOrder = model.DisplayOrder,
            NumericIsoCode = model.NumericIsoCode,
            ThreeLetterIsoCode = model.ThreeLetterIsoCode,
            TwoLetterIsoCode = model.TwoLetterIsoCode,
            Name = model.Name,
            IsBillingEnabled = model.IsBillingEnabled,
            IsCityEnabled = model.IsCityEnabled,
            IsDistrictEnabled = model.IsDistrictEnabled,
            IsPublished = model.IsPublished,
            IsShippingEnabled = model.IsShippingEnabled
        };
        _countryRepository.Add(country);
        await _countryRepository.SaveChangesAsync();
        return Result.Ok();
    }

    /// <summary>
    /// Update country information for the specified ID
    /// </summary>
    /// <param name="id">Country ID to be updated </param>
    /// <param name="model"> Object containing update information </param>
    /// <returns> Operation result </returns>
    [HttpPut("{id:int:min(1)}")]
    public async Task<Result> Put(int id, [FromBody] CountryCreateParam model)
    {
        var country = await _countryRepository.FirstOrDefaultAsync(id);
        if (country == null)
            throw new Exception("Country does not exist");
        var any = _countryRepository.Query().Any(c =>
            (c.NumericIsoCode == model.NumericIsoCode
             || c.TwoLetterIsoCode == model.TwoLetterIsoCode
             || c.ThreeLetterIsoCode == model.ThreeLetterIsoCode)
            && c.Id != id);
        if (any) return Result.Fail("Country code already exists");
        country.DisplayOrder = model.DisplayOrder;
        country.NumericIsoCode = model.NumericIsoCode;
        country.ThreeLetterIsoCode = model.ThreeLetterIsoCode;
        country.TwoLetterIsoCode = model.TwoLetterIsoCode;
        country.Name = model.Name;
        country.IsBillingEnabled = model.IsBillingEnabled;
        country.IsCityEnabled = model.IsCityEnabled;
        country.IsDistrictEnabled = model.IsDistrictEnabled;
        country.IsPublished = model.IsPublished;
        country.IsShippingEnabled = model.IsShippingEnabled;
        country.UpdatedOn = DateTime.Now;
        await _countryRepository.SaveChangesAsync();
        return Result.Ok();
    }

    /// <summary>
    /// Delete the country with the specified ID
    /// </summary>
    /// <param name="id"> Country ID to be deleted </param>
    /// <returns> Operation result </returns>
    [HttpDelete("{id:int:min(1)}")]
    public async Task<Result> Delete(int id)
    {
        var country = await _countryRepository.FirstOrDefaultAsync(id);
        if (country == null)
            throw new Exception("Country does not exist");

        var any = _provinceRepository.Query().Any(c => c.CountryId == country.Id);
        if (any)
            throw new Exception("Please ensure the country is not in use");

        var anyUsed = _addressRepository.Query().Any(c => c.CountryId == id);
        if (anyUsed)
            throw new Exception("Please ensure the country is not in use");

        country.IsDeleted = true;
        country.UpdatedOn = DateTime.Now;
        await _countryRepository.SaveChangesAsync();
        return Result.Ok();
    }

    /// <summary>
    /// Retrieve a paginated list of provinces for the specified country ID
    /// </summary>
    /// <param name="countryId"> country ID </param>
    /// <param name="param"> Pagination and query parameters </param>
    /// <returns> Pagination results containing a list of provinces </returns>
    [HttpPost("provinces/grid/{countryId:int:min(1)}")]
    public async Task<Result<StandardTableResult<ProvinceQueryResult>>> ListProvince(int countryId,
        [FromBody] StandardTableParam<ProvinceQueryParam> param)
    {
        var query = _provinceRepository.Query().Where(c => c.CountryId == countryId);
        var search = param.Search;
        if (search != null)
        {
            if (!string.IsNullOrWhiteSpace(search.Name))
                query = query.Where(c => c.Name.Contains(search.Name.Trim()));
            if (!string.IsNullOrWhiteSpace(search.Code))
                query = query.Where(c => c.Code.Contains(search.Code.Trim()));
            if (search.ParentId.HasValue)
                query = query.Where(c => c.ParentId == search.ParentId);
            if (search.Level.Count > 0)
                query = query.Where(c => search.Level.Contains(c.Level));
        }

        var result = await query.Include(x => x.Parent)
            .ToStandardTableResult(param, province => new ProvinceQueryResult
            {
                Code = province.Code,
                CountryId = province.CountryId,
                CreatedOn = province.CreatedOn,
                DisplayOrder = province.DisplayOrder,
                Id = province.Id,
                IsPublished = province.IsPublished,
                Level = province.Level,
                Name = province.Name,
                ParentId = province.ParentId,
                ParentName = province.Parent == null ? "" : province.Parent.Name,
                UpdatedOn = province.UpdatedOn
            });
        return Result.Ok(result);
    }

    /// <summary>
    /// Retrieve the province tree structure for the specified country ID
    /// </summary>
    /// <param name="countryId"> country ID</param>
    /// <returns>Tree structure list of provinces</returns>
    [HttpGet("provinces/tree/{countryId:int:min(1)}")]
    public async Task<Result<IList<ProvinceTreeResult>>> ProvinceTree(int countryId)
    {
        var list = await _countryService.GetProvinceByCache(countryId);
        var result = _countryService.ProvinceTree(list);
        return Result.Ok(result);
    }

    /// <summary>
    /// Retrieve province details based on province ID
    /// </summary>
    /// <param name="id"> province ID </param>
    /// <returns> Details of the province specified by ID </returns>
    [HttpGet("provinces/{id:int:min(1)}")]
    public async Task<Result<ProvinceGetResult>> GetProvince(int id)
    {
        var province = await _provinceRepository.Query().Include(c => c.Parent).Where(c => c.Id == id)
            .FirstOrDefaultAsync();
        if (province == null)
            throw new Exception("Document does not exist");
        var result = new ProvinceGetResult()
        {
            Code = province.Code,
            CountryId = province.CountryId,
            CreatedOn = province.CreatedOn,
            DisplayOrder = province.DisplayOrder,
            Id = province.Id,
            IsPublished = province.IsPublished,
            Level = province.Level,
            Name = province.Name,
            ParentId = province.ParentId,
            ParentName = province.Parent?.Name,
            UpdatedOn = province.UpdatedOn
        };
        return Result.Ok(result);
    }

    /// <summary>
    /// Add a new province in the specified country.
    /// </summary>
    /// <param name="countryId"> country ID </param>
    /// <param name="model"> Object containing province information </param>
    /// <returns>Operation result </returns>
    [HttpPost("provinces/{countryId:int:min(1)}")]
    public async Task<Result> AddProvince(int countryId, [FromBody] ProvinceCreateParam model)
    {
        var anyCountry = _countryRepository.Query().Any(c => c.Id == countryId);
        if (!anyCountry)
            throw new Exception("Country does not exist");
        var level = await GetLevelByParent(model.ParentId);
        var any = _provinceRepository.Query()
            .Any(c => c.Name == model.Name && c.CountryId == countryId && c.ParentId == model.ParentId);
        if (any)
            throw new Exception("Document already exists");
        var province = new StateOrProvince()
        {
            CountryId = countryId,
            ParentId = model.ParentId,
            DisplayOrder = model.DisplayOrder,
            Name = model.Name,
            IsPublished = model.IsPublished,
            Level = level,
            Code = model.Code
        };
        _provinceRepository.Add(province);
        await _provinceRepository.SaveChangesAsync();
        await _countryService.ClearProvinceCache(countryId);
        return Result.Ok();
    }

    /// <summary>
    /// Update province information for the specified ID
    /// </summary>
    /// <param name="id">Province ID to be updated </param>
    /// <param name="model">Object containing update information </param>
    /// <returns>Operation result </returns>
    [HttpPut("provinces/{id:int:min(1)}")]
    public async Task<Result> EditProvince(int id, [FromBody] ProvinceCreateParam model)
    {
        var province = await _provinceRepository.FirstOrDefaultAsync(id);
        if (province == null)
            throw new Exception("Document does not exist");
        var anyCountry = _countryRepository.Query().Any(c => c.Id == province.CountryId);
        if (!anyCountry)
            throw new Exception("Country does not exist");

        var any = _provinceRepository.Query().Any(c =>
            c.Name == model.Name && c.CountryId == province.CountryId && c.ParentId == model.ParentId && c.Id != id);
        if (any)
            throw new Exception("Document already exists");

        if (model.ParentId == province.Id)
            throw new Exception("Cannot set itself as parent");

        //higher or lower level, raising or lowering control
        //if the current document has subsets, adjusting the level is not allowed
        var level = await GetLevelByParent(model.ParentId);
        if (level != province.Level)
        {
            var anyChild = _provinceRepository.Query().Any(c => c.ParentId == province.Id);
            if (anyChild)
                throw new Exception("He current document has child entries，Adjusting the level is not allowed");
        }

        //province.CountryId = model.CountryId;
        province.ParentId = model.ParentId;
        province.DisplayOrder = model.DisplayOrder;
        province.Name = model.Name;
        province.IsPublished = model.IsPublished;
        province.Level = level;
        province.Code = model.Code;
        province.UpdatedOn = DateTime.Now;
        await _provinceRepository.SaveChangesAsync();
        await _countryService.ClearProvinceCache(province.CountryId);
        return Result.Ok();
    }

    /// <summary>
    /// Delete the province with the specified ID.
    /// </summary>
    /// <param name="id">The ID of the province to be deleted </param>
    /// <returns>Operation result </returns>
    [HttpDelete("provinces/{id:int:min(1)}")]
    public async Task<Result> DeleteProvince(int id)
    {
        var province = await _provinceRepository.FirstOrDefaultAsync(id);
        if (province == null)
            throw new Exception("The document does not exist");

        var any = _provinceRepository.Query().Any(c => c.ParentId == id);
        if (any)
            throw new Exception("Please ensure the document is not in use");

        var anyUsed = _addressRepository.Query().Any(c => c.StateOrProvinceId == id);
        if (anyUsed)
            throw new Exception("Please ensure the document is not being used");

        province.IsDeleted = true;
        province.UpdatedOn = DateTime.Now;
        await _provinceRepository.SaveChangesAsync();
        await _countryService.ClearProvinceCache(province.CountryId);
        return Result.Ok();
    }

    private async Task<StateOrProvinceLevel> GetLevelByParent(int? parentId)
    {
        var level = StateOrProvinceLevel.Default;
        if (parentId.HasValue)
        {
            var anyParent = _provinceRepository.Query().Any(c => c.Id == parentId.Value);
            if (!anyParent)
                throw new Exception("Parent level does not exist");

            var parent = await _provinceRepository.FirstOrDefaultAsync(parentId.Value);
            if (parent == null)
                throw new Exception("Parent level does not exist");

            //If a parent exists, then handle the level.
            switch (parent.Level)
            {
                case StateOrProvinceLevel.Default:
                    level = StateOrProvinceLevel.City;
                    break;

                case StateOrProvinceLevel.City:
                    level = StateOrProvinceLevel.District;
                    break;

                case StateOrProvinceLevel.District:
                    level = StateOrProvinceLevel.Street;
                    break;

                case StateOrProvinceLevel.Street:
                    throw new Exception($"Unable to set minimum level[{parent.Level.ToString()}]as a parent");
                default:
                    throw new Exception("Parent type exception");
            }
        }

        return level;
    }
}
