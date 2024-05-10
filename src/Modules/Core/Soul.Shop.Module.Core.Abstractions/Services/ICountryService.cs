using Soul.Shop.Module.Core.Abstractions.ViewModels;
using Soul.Shop.Module.Core.Abstractions.Models;

namespace Soul.Shop.Module.Core.Abstractions.Services;

public interface ICountryService
{
    Task<IList<StateOrProvinceDto>> GetProvinceByCache(int countryId);

    Task ClearProvinceCache(int countryId);

    IList<ProvinceTreeResult> ProvinceTree(IList<StateOrProvinceDto> list, int? parentId = null);


    void ProvincesTransformToStringArray(IList<StateOrProvinceDto> provinces, int stateOrProvinceId,
        ref IList<string> list, int loop = 0);
}