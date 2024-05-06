using Soul.Shop.Module.Core.Abstractions.ViewModels;

namespace Soul.Shop.Module.Core.Abstractions.Services;

public interface IUserAddressService
{
    Task<IList<UserAddressShippingResult>> GetList(int? userAddressId = null);
}