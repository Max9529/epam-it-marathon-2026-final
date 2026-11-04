using Restaurant_Aqa_Framework.Api.Dtos.Dishes;
using Restaurant_Aqa_Framework.Screenplay;
using Restaurant_Aqa_Framework.Screenplay.Questions;
using Restaurant_Aqa_Framework.Screenplay.Tasks;

namespace Restaurant_Aqa_Framework.StepDefinitions;

[Binding]
public sealed class Homework8CatalogSteps(Actor actor) : StepDefinitionBase(actor)
{
    [Given("the homework restaurant has an individual location")]
    public Task GivenIndividualLocation() => Actor.AttemptsToAsync(new SetUpLocation());

    [Given("the homework restaurant has dishes in every menu category")]
    public async Task GivenCatalog()
    {
        await Actor.AttemptsToAsync(new SetUpLocation(), new AddDish(DishType.Appetizer),
            new AddDish(DishType.MainCourse), new AddDish(DishType.Dessert), new HomeworkCatalog("open"));
    }

    [When("the homework guest selects {string} on the menu")]
    public Task WhenCategory(string category) => Actor.AttemptsToAsync(new HomeworkCatalog("category", category));

    [When("the homework guest clears that category filter")]
    public Task WhenClear() => Actor.AttemptsToAsync(new HomeworkCatalog("clear"));

    [When("the homework guest sorts the menu by {string}")]
    public Task WhenSort(string sorting) => Actor.AttemptsToAsync(new HomeworkCatalog("sort", sorting));

    [Then("the homework menu shows only {string} dishes")]
    public async Task ThenCategory(string category)
    {
        var shown = await Actor.AsksForAsync(new HomeworkCatalogSnapshot());
        var expectedType = category switch { "Appetizers" => "Appetizer", "Main Courses" => "Main course", "Desserts" => "Dessert", _ => category };
        await Assert.That(shown.Names.Length > 0).IsTrue().Because("A category must contain the scenario's seeded dish.");
        await Assert.That(shown.Returned.Dishes.All(dish => dish.DishType.Equals(expectedType, StringComparison.OrdinalIgnoreCase))).IsTrue()
            .Because(string.Join(", ", shown.Returned.Dishes.Select(dish => dish.DishType)));
    }

    [Then("the homework menu shows the complete catalog")]
    public async Task ThenComplete()
    {
        var shown = await Actor.AsksForAsync(new HomeworkCatalogSnapshot());
        await Assert.That(shown.Returned.Dishes.Select(dish => dish.DishType).Distinct().Count()).IsEqualTo(3);
    }

    [Then("the homework menu prices are {string}")]
    public async Task ThenPrices(string direction)
    {
        var shown = await Actor.AsksForAsync(new HomeworkCatalogSnapshot());
        var ordered = direction == "ascending" ? shown.Prices.Order().ToArray() : shown.Prices.OrderDescending().ToArray();
        await Assert.That(shown.Prices.SequenceEqual(ordered)).IsTrue().Because(string.Join(", ", shown.Prices));
        await Assert.That(shown.Prices.SequenceEqual(shown.Returned.Dishes.Select(dish => decimal.Round(dish.Price, 2, MidpointRounding.AwayFromZero)))).IsTrue();
    }

    [Then("the homework menu preserves the returned popularity order")]
    public async Task ThenPopularity()
    {
        var shown = await Actor.AsksForAsync(new HomeworkCatalogSnapshot());
        await Assert.That(shown.Names.Length > 0).IsTrue();
        await Assert.That(shown.Names.SequenceEqual(shown.Returned.Dishes.Select(dish => dish.Name))).IsTrue();
    }

    [When("the homework guest opens a sample dish on the menu")]
    public Task WhenOpenDish() => Actor.AttemptsToAsync(new HomeworkCatalog("open dish"));

    [Then("the homework dish details match the selected dish")]
    public async Task ThenDishDetails()
    {
        var details = await Actor.AsksForAsync(new HomeworkDishDetails());
        var dish = Actor.Recall<DishResponse>("selected homework dish");
        await Assert.That(details).Contains(dish.Name);
        await Assert.That(details).Contains(dish.Description);
        await Assert.That(details).Contains(dish.Weight);
    }

    [When("the homework guest opens the restaurant home page")]
    public Task WhenHome() => Actor.AttemptsToAsync(new HomeworkCatalog("home"));

    [Then("the homework home page shows popular dishes and locations")]
    public async Task ThenHome()
    {
        var shown = await Actor.AsksForAsync(new HomeworkHomeCatalog());
        await Assert.That(shown.Dishes > 0).IsTrue();
        await Assert.That(shown.Locations > 0).IsTrue();
    }
}
