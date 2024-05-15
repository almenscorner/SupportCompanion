using SupportCompanion.Models;
using SupportCompanion.Services;

namespace SupportCompanion.ViewModels;

public class EvergreenWidgetViewModel : ViewModelBase
{
    private readonly CatalogsService _catalogsService;
    private List<string> _catalogs = new();

    public EvergreenWidgetViewModel(CatalogsService catalogs)
    {
        _catalogsService = catalogs;
        EvergreenInfo = new EvergreenInfoModel();
        InitializeAsync();
    }

    public EvergreenInfoModel EvergreenInfo { get; }

    private async void InitializeAsync()
    {
        await DeviceCatalogs();
    }

    private async Task DeviceCatalogs()
    {
        _catalogs = await _catalogsService.GetCatalogs();
        EvergreenInfo.Catalogs = _catalogs;
    }
}