using Restaurant_Aqa_Framework.Screenplay;
using Restaurant_Aqa_Framework.Screenplay.Questions;
using Restaurant_Aqa_Framework.Screenplay.Tasks;

namespace Restaurant_Aqa_Framework.StepDefinitions;

[Binding]
public sealed class Homework8SignInSteps(Actor actor) : StepDefinitionBase(actor)
{
    [Given("the homework customer has an individual account")]
    public Task GivenCustomer() => Actor.AttemptsToAsync(new CreateHomeworkCustomer());

    [When("the homework customer signs in with {string} credentials")]
    public Task WhenSignIn(string credentials) => Actor.AttemptsToAsync(new HomeworkSignIn(credentials));

    [Then("the homework storefront refuses sign-in clearly")]
    public async Task ThenRefused()
    {
        var result = await Actor.AsksForAsync(new HomeworkSignInRefusal());
        await Assert.That(result.StayedOnLogin).IsTrue().Because("Wrong credentials must not give access.");
        await Assert.That(result.Message).Contains("Incorrect email or password").Because(result.Message);
    }
}
