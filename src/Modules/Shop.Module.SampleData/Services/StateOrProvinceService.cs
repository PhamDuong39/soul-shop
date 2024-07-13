using Newtonsoft.Json;
using Shop.Infrastructure.Data;
using Shop.Module.Core.Cache;
using Shop.Module.Core.Entities;
using Shop.Module.Core.Models;
using Shop.Module.Core.Services;
using Shop.Module.SampleData.Data;
using Shop.Module.SampleData.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Shop.Module.SampleData.Services;

public class StateOrProvinceService : IStateOrProvinceService
{
    private readonly ISqlRepository _sqlRepository;
    private readonly IStaticCacheManager _cache;
    private readonly IRepository<StateOrProvince> _provinceRepository;

    public StateOrProvinceService(
        ISqlRepository sqlRepository,
        IStaticCacheManager cache,
        IRepository<StateOrProvince> provinceRepository)
    {
        _sqlRepository = sqlRepository;
        _cache = cache;
        _provinceRepository = provinceRepository;
    }

    public async Task GenPcas()
    {
        throw new NotImplementedException();

        // This interface is not open. Province, city, and district data are imported into the database using scripts for provinces, cities, and districts.

        //“Three-level cascading data for provinces, cities, and districts pca-code.json
        //“Four-level cascading data for provinces, cities, districts, and towns pcas-code.json
        var countryId = (int)CountryWithId.China;
        var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SampleContent", "pca-code.json");
        if (!File.Exists(path))
            return;

        var content = File.ReadAllText(path);
        var pcas = JsonConvert.DeserializeObject<List<SampleDataPcasDto>>(content);

        var list = new List<StateOrProvince>();
        Gen(list, pcas, StateOrProvinceLevel.Default, countryId);

        _provinceRepository.AddRange(list);
        await _provinceRepository.SaveChangesAsync();

        _cache.Clear();
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
