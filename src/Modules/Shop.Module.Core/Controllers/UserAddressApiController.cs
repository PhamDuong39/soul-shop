using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Infrastructure;
using Shop.Infrastructure.Data;
using Shop.Module.Core.Entities;
using Shop.Module.Core.Extensions;
using Shop.Module.Core.Models;
using Shop.Module.Core.Services;
using Shop.Module.Core.ViewModels;

namespace Shop.Module.Core.Controllers;

/// <summary>
/// User shipping address related API
/// </summary>
[ApiController]
[Authorize]
[Route("api/user-addresses")]
public class UserAddressApiController : ControllerBase
{
    private readonly IRepository<UserAddress> _userAddressRepository;
    private readonly IRepository<Country> _countryRepository;
    private readonly IRepository<StateOrProvince> _stateOrProvinceRepository;
    private readonly IRepository<User> _userRepository;
    private readonly IWorkContext _workContext;
    private readonly ICountryService _countryService;
    private readonly IUserAddressService _userAddressService;

    public UserAddressApiController(
        IRepository<UserAddress> userAddressRepository,
        IRepository<Country> countryRepository,
        IRepository<StateOrProvince> stateOrProvinceRepository,
        IRepository<User> userRepository,
        IWorkContext workContext,
        ICountryService countryService,
        IUserAddressService userAddressService)
    {
        _userAddressRepository = userAddressRepository;
        _countryRepository = countryRepository;
        _stateOrProvinceRepository = stateOrProvinceRepository;
        _userRepository = userRepository;
        _workContext = workContext;
        _countryService = countryService;
        _userAddressService = userAddressService;
    }

    /// <summary>
    /// Retrieve all shipping addresses of the current user
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<Result> List()
    {
        var result = await _userAddressService.GetList();
        return Result.Ok(result);
    }

    /// <summary>
    /// Retrieve shipping address details
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id:int:min(1)}")]
    public async Task<Result> Get(int id)
    {
        var list = await _userAddressService.GetList(id);
        var result = list.FirstOrDefault();
        return Result.Ok(result);
    }

    /// <summary>
    /// Add user shipping address
    /// </summary>
    /// <param name="param"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    [HttpPost]
    public async Task<Result> Post([FromBody] UserAddressCreateParam param)
    {
        var user = await _workContext.GetCurrentUserAsync();
        var countryId = (int)CountryWithId.China;
        var provinces = await _countryService.GetProvinceByCache(countryId);
        if (provinces == null || provinces.Count <= 0)
            throw new Exception("Province, city, and district data anomaly，Please contact the administrator");

        if (!provinces.Any(c => c.Id == param.StateOrProvinceId && c.Level == StateOrProvinceLevel.Default))
            throw new Exception("The selected province information does not exist");

        if (!provinces.Any(c =>
                c.Id == param.CityId && c.Level == StateOrProvinceLevel.City && c.ParentId == param.StateOrProvinceId))
            throw new Exception("The selected city information does not exist");

        if (param.DistrictId.HasValue)
            if (!provinces.Any(c =>
                    c.Id == param.DistrictId && c.Level == StateOrProvinceLevel.District && c.ParentId == param.CityId))
                throw new Exception("The selected district information does not exist");

        var address = new Address()
        {
            AddressLine1 = param.AddressLine1,
            ContactName = param.ContactName,
            Phone = param.Phone,
            CountryId = countryId,
            StateOrProvinceId = param.DistrictId ?? param.CityId // tore minimal structured data
        };
        var userAddress = new UserAddress()
        {
            Address = address,
            UserId = user.Id,
            AddressType = AddressType.Shipping
        };
        _userAddressRepository.Add(userAddress);

        var tran = _userAddressRepository.BeginTransaction();
        await _userAddressRepository.SaveChangesAsync();
        if (param.IsDefault)
        {
            var dbUser = await _userRepository.FirstOrDefaultAsync(user.Id);
            dbUser.DefaultShippingAddress = userAddress;
            await _userRepository.SaveChangesAsync();
        }

        tran.Commit();
        return Result.Ok();
    }

    /// <summary>
    /// Update user shipping address
    /// </summary>
    /// <param name="id"></param>
    /// <param name="param"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    [HttpPut("{id:int:min(1)}")]
    public async Task<Result> Put(int id, [FromBody] UserAddressCreateParam param)
    {
        var user = await _workContext.GetCurrentUserAsync();
        var userAddress = await _userAddressRepository.Query().Include(c => c.Address)
            .FirstOrDefaultAsync(c => c.Id == id && c.UserId == user.Id);
        if (userAddress?.Address == null) throw new Exception("Address does not exist");

        var countryId = (int)CountryWithId.China;
        var provinces = await _countryService.GetProvinceByCache(countryId);
        if (provinces == null || provinces.Count <= 0)
            throw new Exception("Province, city, and district data anomaly, please contact the administrator");

        if (!provinces.Any(c => c.Id == param.StateOrProvinceId && c.Level == StateOrProvinceLevel.Default))
            throw new Exception("The selected province information does not exist");

        if (!provinces.Any(c =>
                c.Id == param.CityId && c.Level == StateOrProvinceLevel.City && c.ParentId == param.StateOrProvinceId))
            throw new Exception("The selected city information does not exist");

        if (param.DistrictId.HasValue)
            if (!provinces.Any(c =>
                    c.Id == param.DistrictId && c.Level == StateOrProvinceLevel.District && c.ParentId == param.CityId))
                throw new Exception("The selected district information does not exist");

        userAddress.Address.AddressLine1 = param.AddressLine1;
        userAddress.Address.ContactName = param.ContactName;
        userAddress.Address.Phone = param.Phone;
        userAddress.Address.CountryId = countryId;
        userAddress.Address.StateOrProvinceId = param.DistrictId ?? param.CityId;
        userAddress.Address.UpdatedOn = DateTime.Now;

        await _userAddressRepository.SaveChangesAsync();
        if (param.IsDefault)
        {
            var dbUser = await _userRepository.Query()
                .FirstOrDefaultAsync(c => c.Id == user.Id);
            dbUser.DefaultShippingAddressId = userAddress.Id;
            await _userRepository.SaveChangesAsync();
        }
        else
        {
            var dbUser = await _userRepository.Query()
                .FirstOrDefaultAsync(c => c.Id == user.Id && c.DefaultShippingAddressId == id);
            if (dbUser?.DefaultShippingAddress != null)
            {
                dbUser.DefaultShippingAddressId = null;
                await _userRepository.SaveChangesAsync();
            }
        }

        return Result.Ok();
    }

    /// <summary>
    /// Delete user shipping address
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    [HttpDelete("{id:int:min(1)}")]
    public async Task<Result> Delete(int id)
    {
        var user = await _workContext.GetCurrentUserAsync();
        var userAddress = await _userAddressRepository
            .Query()
            .Include(c => c.Address)
            .FirstOrDefaultAsync(c => c.Id == id && c.UserId == user.Id);
        if (userAddress?.Address == null) throw new Exception("Address does not exist");

        userAddress.IsDeleted = true;
        userAddress.Address.IsDeleted = true;
        userAddress.Address.UpdatedOn = DateTime.Now;
        await _userAddressRepository.SaveChangesAsync();

        var dbUser = await _userRepository
            .Query()
            .Include(c => c.DefaultShippingAddress)
            .FirstOrDefaultAsync(c => c.Id == user.Id && c.DefaultShippingAddressId == id);
        if (dbUser?.DefaultShippingAddress != null)
        {
            dbUser.DefaultShippingAddressId = null;
            await _userRepository.SaveChangesAsync();
        }

        return Result.Ok();
    }

    /// <summary>
    /// Province, city, district list
    /// </summary>
    /// <returns></returns>
    [HttpGet("provinces")]
    public async Task<Result> Get(CountryWithId countryId = CountryWithId.China)
    {
        var list = await _countryService.GetProvinceByCache((int)countryId);
        var result = new
        {
            Provinces = list.Where(c => c.Level == StateOrProvinceLevel.Default)
                .Select(c => StateOrProvinceGetResult.FromStateOrProvince(c)),

            Citys = list.Where(c => c.Level == StateOrProvinceLevel.City)
                .Select(c => StateOrProvinceGetResult.FromStateOrProvince(c)),

            Districts = list.Where(c => c.Level == StateOrProvinceLevel.District)
                .Select(c => StateOrProvinceGetResult.FromStateOrProvince(c))
        };
        return Result.Ok(result);
    }
}
