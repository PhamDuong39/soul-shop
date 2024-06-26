﻿using Soul.Shop.Module.Core.Abstractions.Models;

namespace Soul.Shop.Module.Core.Abstractions.ViewModels;

public class StateOrProvinceGetResult
{
    public int Id { get; set; }

    public int? ParentId { get; set; }

    public string Name { get; set; }

    public int Level { get; set; }

    public static StateOrProvinceGetResult FromStateOrProvince(StateOrProvinceDto model)
    {
        if (model == null)
            return null;
        return new StateOrProvinceGetResult()
        {
            Id = model.Id,
            Level = (int)model.Level,
            Name = model.Name,
            ParentId = model.ParentId ?? 0
        };
    }
}