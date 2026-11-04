using Restaurant_Aqa_Framework.Api.Dtos.Auth;
using Restaurant_Aqa_Framework.Api.Seeding;
using Restaurant_Aqa_Framework.Support;

namespace Restaurant_Aqa_Framework.Screenplay.Tasks;

public sealed class HomeworkBoundarySignUp(string field, int length) : ITask
{
    public async Task PerformAsAsync(Actor actor)
    {
        var password = SampleDataFactory.ValidPassword();
        var details = new RegisterRequest("Anna", "Tester", SampleDataFactory.UniqueEmail(), password, password);
        var value = Homework8Inputs.AtLength(field, length);
        details = field switch
        {
            "first name" => details with { FirstName = value },
            "last name" => details with { LastName = value },
            "password" => details with { Password = value, ConfirmPassword = value },
            // Keep each valid long address unique, including when tests run in parallel.
            "email" => details with { Email = Guid.NewGuid().ToString("N") + value[32..] },
            _ => throw new InvalidOperationException($"Unknown registration field: {field}"),
        };
        await actor.AttemptsToAsync(new SignUp(details));
    }
}
