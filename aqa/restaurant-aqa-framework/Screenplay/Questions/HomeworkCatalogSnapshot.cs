using System.Globalization;
using Microsoft.Playwright;
using Restaurant_Aqa_Framework.Screenplay.Tasks;
using Restaurant_Aqa_Framework.Ui;

namespace Restaurant_Aqa_Framework.Screenplay.Questions;

public sealed record HomeworkMenuSnapshot(string[] Names, decimal[] Prices, HomeworkCatalogData Returned);

public sealed class HomeworkCatalogSnapshot : IQuestion<HomeworkMenuSnapshot>
{
    public async Task<HomeworkMenuSnapshot> AnsweredByAsync(Actor actor)
    {
        var page = new HomeworkCatalogPage(actor.LookingAtThePage());
        var returned = actor.Recall<HomeworkCatalogData>();
        // Wait for rendering of the actual observed response, rather than an arbitrary delay.
        await Assertions.Expect(page.Names).ToHaveTextAsync(returned.Dishes.Select(dish => dish.Name).ToArray());
        var names = (await page.Names.AllTextContentsAsync()).Select(name => name.Trim()).ToArray();
        var prices = (await page.Prices.AllTextContentsAsync())
            .Select(price => decimal.Parse(price.Replace("$", "", StringComparison.Ordinal).Replace(",", "", StringComparison.Ordinal).Trim(), CultureInfo.InvariantCulture)).ToArray();
        return new HomeworkMenuSnapshot(names, prices, returned);
    }
}

public sealed class HomeworkDishDetails : IQuestion<string>
{
    public async Task<string> AnsweredByAsync(Actor actor)
    {
        var modal = new HomeworkCatalogPage(actor.LookingAtThePage()).Modal;
        await modal.WaitForAsync();
        return await modal.InnerTextAsync();
    }
}

public sealed class HomeworkHomeCatalog : IQuestion<(int Dishes, int Locations)>
{
    public async Task<(int Dishes, int Locations)> AnsweredByAsync(Actor actor)
    {
        var page = new HomeworkCatalogPage(actor.LookingAtThePage());
        await page.PopularDishNames.First.WaitForAsync();
        await page.LocationAddresses.First.WaitForAsync();
        return (await page.PopularDishNames.CountAsync(), await page.LocationAddresses.CountAsync());
    }
}
