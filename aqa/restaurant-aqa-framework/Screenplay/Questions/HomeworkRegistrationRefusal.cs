using Restaurant_Aqa_Framework.Screenplay.Tasks;
using Restaurant_Aqa_Framework.Ui;

namespace Restaurant_Aqa_Framework.Screenplay.Questions;

public sealed record HomeworkRegistrationResult(bool Prevented, string Explanation);

public sealed class HomeworkRegistrationRefusal : IQuestion<HomeworkRegistrationResult>
{
    public async Task<HomeworkRegistrationResult> AnsweredByAsync(Actor actor)
    {
        var page = actor.LookingAtThePage();
        var form = new RegisterPage(page);
        await form.ValidationFeedback.First.WaitForAsync();
        var explanation = string.Join("; ", await form.ValidationFeedback.AllTextContentsAsync());
        var status = actor.Recall<HomeworkRegistrationAttempt>().Status;
        var refused = status is 400 or 422 || (status is null && !await form.SubmitButton.IsEnabledAsync());
        return new HomeworkRegistrationResult(refused && new Uri(page.Url).AbsolutePath == "/signup", explanation);
    }
}
