using Restaurant_Aqa_Framework.Api.Dtos.Auth;
using Restaurant_Aqa_Framework.Api.Seeding;
using Restaurant_Aqa_Framework.Ui;

namespace Restaurant_Aqa_Framework.Screenplay.Tasks;

public sealed class HomeworkSignIn(string credentials) : ITask
{
    public async Task PerformAsAsync(Actor actor)
    {
        var details = actor.Recall<RegisterRequest>();
        var form = new LoginPage(actor.LookingAtThePage());
        await form.NavigateAsync();
        var email = credentials == "unknown email" ? SampleDataFactory.UniqueEmail() : details.Email;
        var password = credentials == "wrong password" ? "Incorrect9!" : details.Password;
        await actor.LookingAtThePage().RunAndWaitForResponseAsync(() => form.SignInAsync(email, password),
            response => new Uri(response.Url).AbsolutePath.EndsWith("/signin", StringComparison.Ordinal));
    }
}
