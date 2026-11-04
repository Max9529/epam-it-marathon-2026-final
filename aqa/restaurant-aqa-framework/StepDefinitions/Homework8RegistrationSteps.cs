using Restaurant_Aqa_Framework.Api.Dtos.Auth;
using Restaurant_Aqa_Framework.Api.Seeding;
using Restaurant_Aqa_Framework.Screenplay;
using Restaurant_Aqa_Framework.Screenplay.Questions;
using Restaurant_Aqa_Framework.Screenplay.Tasks;
using Restaurant_Aqa_Framework.Support;

namespace Restaurant_Aqa_Framework.StepDefinitions;

[Binding]
public sealed class Homework8RegistrationSteps(Actor actor) : StepDefinitionBase(actor)
{
    [Given("the homework visitor has filled in valid registration details")]
    public Task GivenValidDetails()
    {
        var password = SampleDataFactory.ValidPassword();
        return Actor.AttemptsToAsync(new FillInTheSignUpForm(
            new RegisterRequest("Anna", "Tester", SampleDataFactory.UniqueEmail(), password, password)));
    }

    [When("the homework visitor uses a {string} of length {int}")]
    [When("the homework visitor uses an {string} of length {int}")]
    public Task WhenBoundary(string field, int length) =>
        Actor.AttemptsToAsync(new EnterHomeworkRegistrationDetail(field, Homework8Inputs.AtLength(field, length)));

    [When("the homework visitor uses {string} as {string}")]
    public Task WhenValue(string value, string field) =>
        Actor.AttemptsToAsync(new EnterHomeworkRegistrationDetail(field, value));

    [When("the homework visitor registers with a {string} of length {int}")]
    public Task WhenRegisterBoundary(string field, int length) =>
        Actor.AttemptsToAsync(new HomeworkBoundarySignUp(field, length));

    [Then("the homework registration form is {string}")]
    public async Task ThenAvailability(string availability)
    {
        if (availability == "unavailable")
        {
            await Actor.AttemptsToAsync(new AttemptInvalidHomeworkRegistration());
            var result = await Actor.AsksForAsync(new HomeworkRegistrationRefusal());
            await Assert.That(result.Prevented).IsTrue().Because(result.Explanation);
            await Assert.That(string.IsNullOrWhiteSpace(result.Explanation)).IsFalse();
            return;
        }
        var actual = await Actor.AsksForAsync(new TheSignUpFormCanBeSubmitted());
        var messages = actual == (availability == "available")
            ? Array.Empty<string>()
            : await Actor.AsksForAsync(new TheSignUpFormMessages());
        await Assert.That(actual).IsEqualTo(availability == "available")
            .Because($"Contract expects {availability}; visible validation: {string.Join("; ", messages)}");
    }

    [Then("the homework registration form explains the invalid detail")]
    public async Task ThenExplanation() =>
        await Assert.That((await Actor.AsksForAsync(new TheSignUpFormMessages())).Any()).IsTrue()
            .Because("Invalid registration data must have a visible explanation.");

    [Then("the homework signed-in account has only the customer role")]
    public async Task ThenCustomerRole()
    {
        var roles = await Actor.AsksForAsync(new HomeworkCustomerRoles());
        await Assert.That(roles.Length).IsEqualTo(1).Because(string.Join(", ", roles));
        await Assert.That(roles[0]).IsEqualTo("Customer").Because(string.Join(", ", roles));
    }
}
