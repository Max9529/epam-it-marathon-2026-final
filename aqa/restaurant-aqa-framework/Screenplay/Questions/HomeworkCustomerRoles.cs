using System.Text;
using System.Text.Json;
using Restaurant_Aqa_Framework.Api;
using Restaurant_Aqa_Framework.Api.Dtos.Auth;

namespace Restaurant_Aqa_Framework.Screenplay.Questions;

public sealed class HomeworkCustomerRoles : IQuestion<string[]>
{
    public async Task<string[]> AnsweredByAsync(Actor actor)
    {
        // Supplemental service verification of the account created by the UI journey.
        var details = actor.Recall<RegisterRequest>();
        await using var response = await new AuthApi(actor.UsingTheApi())
            .SignInAsync(new SignInRequest(details.Email, details.Password));
        if (!response.Ok)
            throw new InvalidOperationException($"Customer verification failed: {response.Status}");
        var signedIn = await response.ReadAsAsync<SignInResponse>();
        var payload = signedIn.IdToken.Split('.')[1].Replace('-', '+').Replace('_', '/');
        payload = payload.PadRight((payload.Length + 3) / 4 * 4, '=');
        using var json = JsonDocument.Parse(Encoding.UTF8.GetString(Convert.FromBase64String(payload)));
        return json.RootElement.GetProperty("cognito:groups").EnumerateArray()
            .Select(group => group.GetString() ?? "").ToArray();
    }
}
