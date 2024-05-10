﻿namespace Soul.Shop.Module.Shipping.Abstractions.ViewModels;

public class FreightTemplateQueryResult
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string Note { get; set; }

    public DateTime CreatedOn { get; set; }

    public DateTime UpdatedOn { get; set; }
}