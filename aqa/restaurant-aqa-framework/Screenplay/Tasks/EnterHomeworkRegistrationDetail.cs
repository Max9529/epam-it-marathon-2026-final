using Restaurant_Aqa_Framework.Support;
using Restaurant_Aqa_Framework.Ui;

namespace Restaurant_Aqa_Framework.Screenplay.Tasks;

public sealed class EnterHomeworkRegistrationDetail(string field, string value) : ITask
{
    public async Task PerformAsAsync(Actor actor)
    {
        var form = new RegisterPage(actor.LookingAtThePage());
        var input = Homework8Inputs.Field(field);
        await form.FillAsync(input, value);
        if (input == InputField.Password)
            await form.FillAsync(InputField.PasswordConfirmation, value);
        await form.Input(input).PressAsync("Tab");
    }
}
