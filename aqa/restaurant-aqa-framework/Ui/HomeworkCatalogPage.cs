using Microsoft.Playwright;

namespace Restaurant_Aqa_Framework.Ui;

public sealed class HomeworkCatalogPage(IPage page) : BasePage(page)
{
    public ILocator Names => Page.Locator("app-dish-card .dish-name");
    public ILocator Prices => Page.Locator("app-dish-card .dish-price");
    public ILocator Modal => Page.Locator("app-dish-modal .modal-content");
    public ILocator Search => Page.GetByRole(AriaRole.Searchbox);

    public Task<IResponse> OpenMenuAsync() => CaptureCatalogAsync(() => Page.GotoAsync("/menu"));
    public Task<IResponse> SelectCategoryAsync(string category) => CaptureCatalogAsync(
        () => Page.GetByRole(AriaRole.Button, new() { Name = category, Exact = true }).ClickAsync());
    public Task<IResponse> SortAsync(string sorting) => CaptureCatalogAsync(
        () => Page.GetByLabel("Sort by:", new() { Exact = true }).SelectOptionAsync(new SelectOptionValue { Label = sorting }));
    public Task OpenDishAsync(string name) =>
        Page.Locator("app-dish-card").Filter(new() { Has = Page.Locator(".dish-name").GetByText(name, new() { Exact = true }) })
            .GetByRole(AriaRole.Button).ClickAsync();
    public Task OpenHomeAsync() => Page.GotoAsync("/main");
    public ILocator PopularDishNames => Page.Locator(".popular-dishes-section .dish-name");
    public ILocator LocationAddresses => Page.Locator("app-location-card");

    private Task<IResponse> CaptureCatalogAsync(Func<Task> action) => Page.RunAndWaitForResponseAsync(action,
        response => new Uri(response.Url).AbsolutePath.EndsWith("/dishes", StringComparison.Ordinal)
            && response.Request.Method == "GET");
}
