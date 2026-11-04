using Restaurant_Aqa_Framework.Ui;

namespace Restaurant_Aqa_Framework.Screenplay.Tasks;

public sealed record HomeworkRegistrationAttempt(int? Status);

public sealed class AttemptInvalidHomeworkRegistration : ITask
{
    public async Task PerformAsAsync(Actor actor)
    {
        var page = actor.LookingAtThePage();
        var form = new RegisterPage(page);
        if (!await form.SubmitButton.IsEnabledAsync())
        {
            actor.Remember(new HomeworkRegistrationAttempt(null));
            return;
        }
        // The specification permits client-side OR server-side rejection. An
        // enabled button alone must not be reported as a registration defect.
        var response = await page.RunAndWaitForResponseAsync(() => form.SubmitButton.ClickAsync(),
            response => new Uri(response.Url).AbsolutePath.EndsWith("/register", StringComparison.Ordinal));
        actor.Remember(new HomeworkRegistrationAttempt(response.Status));
    }
}
