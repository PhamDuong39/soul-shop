using Newtonsoft.Json;
using Shop.Module.Core.Cache;
using Soul.Shop.Infrastructure.Data;
using Soul.Shop.Module.Core.Abstractions.Entities;
using Soul.Shop.Module.Core.Abstractions.Models;
using Soul.Shop.Module.SampleData.Data;
using Soul.Shop.Module.SampleData.Models;

namespace Soul.Shop.Module.SampleData.Services;

public class StateOrProvinceService(
    ISqlRepository sqlRepository,
    IStaticCacheManager cache,
    IRepository<StateOrProvince> provinceRepository)
    : IStateOrProvinceService
{
    private readonly ISqlRepository _sqlRepository = sqlRepository;

    public async Task GenPcas()
    {
        throw new NotImplementedException();
    }

    private void Gen(List<StateOrProvince> list, IList<SampleDataPcasDto> pcas, StateOrProvinceLevel level,
        int countryId, StateOrProvince parent = null)
    {
        var i = 0;
        foreach (var item in pcas)
        {
            var model = new StateOrProvince()
            {
                CountryId = countryId,
                Parent = parent,
                DisplayOrder = i,
                Name = item.Name,
                IsPublished = true,
                Level = level,
                Code = item.Code
            };
            list.Add(model);

            if (item.Childrens != null && item.Childrens.Count > 0)
                Gen(list, item.Childrens, (StateOrProvinceLevel)((int)level + 1), countryId, model);

            i++;
        }
    }
}