using Soul.Shop.Module.Core.Abstractions.Entities;

namespace Soul.Shop.Module.Core.Abstractions.Services;

public interface IWidgetInstanceService
{
    IQueryable<WidgetInstance> GetPublished();
}