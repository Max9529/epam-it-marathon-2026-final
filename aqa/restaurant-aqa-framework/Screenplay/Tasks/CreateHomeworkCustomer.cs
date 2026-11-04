using Restaurant_Aqa_Framework.Api.Seeding;

namespace Restaurant_Aqa_Framework.Screenplay.Tasks;

public sealed class CreateHomeworkCustomer : ITask
{
    public async Task PerformAsAsync(Actor actor)
    {
        await actor.AttemptsToAsync(new RegisterThroughTheService(SampleDataFactory.RegisterRequest()));
        var result = actor.TheLastResponse();
        if (!result.Succeeded)
            throw new InvalidOperationException($"Customer setup failed: {result.Status}: {result.Body}");
    }
}
