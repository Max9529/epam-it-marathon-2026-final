using System.Text.Json;
using Restaurant_Aqa_Framework.Api.Dtos.Dishes;
using Restaurant_Aqa_Framework.Ui;

namespace Restaurant_Aqa_Framework.Screenplay.Tasks;

public sealed record HomeworkCatalogData(DishListResponse[] Dishes, string? Category, string? Sorting);

public sealed class HomeworkCatalog(string action, string? value = null) : ITask
{
    public async Task PerformAsAsync(Actor actor)
    {
        var catalog = new HomeworkCatalogPage(actor.LookingAtThePage());
        if (action == "open dish")
        {
            await catalog.OpenMenuAsync();
            var dish = actor.Recall<DishResponse>();
            actor.Remember("selected homework dish", dish);
            await catalog.OpenDishAsync(dish.Name);
            return;
        }
        if (action == "home")
        {
            await catalog.OpenHomeAsync();
            return;
        }

        var response = action switch
        {
            "category" => await catalog.SelectCategoryAsync(value!),
            "sort" => await catalog.SortAsync(value!),
            "clear" => await catalog.SelectCategoryAsync(actor.Recall<HomeworkCatalogData>().Category!),
            _ => await catalog.OpenMenuAsync(),
        };
        if (!response.Ok) throw new InvalidOperationException($"Menu returned {response.Status}: {await response.TextAsync()}");
        var dishes = JsonSerializer.Deserialize<DishListResponse[]>(await response.TextAsync(),
            JsonSerializerOptions.Web) ?? [];
        actor.Remember(new HomeworkCatalogData(dishes, action == "category" ? value : null, action == "sort" ? value : null));
    }
}
