using Shop.Module.SampleData.ViewModels;

namespace Soul.Shop.Module.SampleData.Services;

public interface ISampleDataService
{
    Task ResetToSampleData(SampleDataOption model);
}