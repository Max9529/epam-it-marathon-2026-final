using Restaurant_Aqa_Framework.Ui;

namespace Restaurant_Aqa_Framework.Screenplay.Questions;

public sealed class HomeworkSignInRefusal : IQuestion<(string Message, bool StayedOnLogin)>
{
    public async Task<(string Message, bool StayedOnLogin)> AnsweredByAsync(Actor actor)
    {
        var page = actor.LookingAtThePage();
        var form = new LoginPage(page);
        await form.ErrorMessage.WaitForAsync();
        return ((await form.ErrorMessage.InnerTextAsync()).Trim(), new Uri(page.Url).AbsolutePath == "/login");
    }
}
