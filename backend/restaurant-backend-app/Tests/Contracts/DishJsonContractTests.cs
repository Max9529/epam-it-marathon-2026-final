using APIHandler.Entities;
using APIHandler.Mappers;
using AutoFixture.Xunit2;
using System.Text.Json;

namespace APIHandler.Tests.Contracts;

public class DishJsonContractTests
{
    [Theory, AutoData]
    public void DishListIncludesFatsUnderTheFrontendContractName(Dish dish)
    {
        var response = JsonSerializer.SerializeToElement(DishMapper.ToDishListResponse(dish));

        Assert.True(response.TryGetProperty("fats", out var fats));
        Assert.Equal(dish.Fats, fats.GetString());
        Assert.False(response.TryGetProperty("Fats", out _));
    }
}
