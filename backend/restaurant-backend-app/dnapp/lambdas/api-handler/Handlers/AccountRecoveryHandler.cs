using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using APIHandler.Constants;
using APIHandler.Helpers;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace APIHandler.Handlers;

/// <summary>Cognito verifies ownership of the email/code or access token; no administrator bypass is used.</summary>
public class AccountRecoveryHandler(AmazonCognitoIdentityProviderClient cognito) : IApiRequestHandler
{
    private sealed class RecoveryRequest
    {
        [JsonPropertyName("email")] public string Email { get; set; } = "";
        [JsonPropertyName("code")] public string Code { get; set; } = "";
        [JsonPropertyName("password")] public string Password { get; set; } = "";
        [JsonPropertyName("accessToken")] public string AccessToken { get; set; } = "";
    }

    public async Task<APIGatewayProxyResponse> HandleRequest(APIGatewayProxyRequest request, ILambdaContext context)
    {
        RecoveryRequest? body;
        try { body = JsonSerializer.Deserialize<RecoveryRequest>(request.Body ?? ""); }
        catch (JsonException) { return ResponseBuilder.CreateErrorResponse(400, "Invalid request body.", context.Logger); }
        if (body is null) return ResponseBuilder.CreateErrorResponse(400, "Invalid request body.", context.Logger);
        var action = request.Path.TrimEnd('/');
        var verifyingEmail = action.StartsWith("/email-verification", StringComparison.Ordinal);
        if (verifyingEmail && string.IsNullOrWhiteSpace(body.AccessToken))
            return ResponseBuilder.CreateErrorResponse(401, "Please sign in to verify your email.", context.Logger);
        if (!verifyingEmail && (string.IsNullOrWhiteSpace(body.Email) || body.Email.Length > 254 || !body.Email.Contains('@')))
            return ResponseBuilder.CreateErrorResponse(400, "Enter a valid email address.", context.Logger);
        try
        {
            switch (action)
            {
                case "/password-recovery":
                    await cognito.ForgotPasswordAsync(new ForgotPasswordRequest { ClientId = ResourceNames.ClientId, Username = body.Email.Trim() });
                    return RecoveryNotice();
                case "/password-recovery/confirm":
                    if (string.IsNullOrEmpty(body.Password) || body.Password.Length < 8 || body.Password.Length > 16 || string.IsNullOrWhiteSpace(body.Code))
                        return ResponseBuilder.CreateErrorResponse(400, "Enter the email code and a password of 8–16 characters.", context.Logger);
                    await cognito.ConfirmForgotPasswordAsync(new ConfirmForgotPasswordRequest
                    { ClientId = ResourceNames.ClientId, Username = body.Email.Trim(), ConfirmationCode = body.Code.Trim(), Password = body.Password });
                    return ResponseBuilder.CreateSuccessResponse("Password updated. You can now sign in.");
                case "/email-verification":
                    await cognito.GetUserAttributeVerificationCodeAsync(new GetUserAttributeVerificationCodeRequest
                    { AccessToken = body.AccessToken, AttributeName = "email" });
                    return ResponseBuilder.CreateSuccessResponse("A verification code was sent to your account email.");
                case "/email-verification/confirm":
                    if (string.IsNullOrWhiteSpace(body.Code))
                        return ResponseBuilder.CreateErrorResponse(400, "Enter the verification code.", context.Logger);
                    await cognito.VerifyUserAttributeAsync(new VerifyUserAttributeRequest
                    { AccessToken = body.AccessToken, AttributeName = "email", Code = body.Code.Trim() });
                    return ResponseBuilder.CreateSuccessResponse("Email verified. Password recovery is now available for your account.");
                default:
                    return ResponseBuilder.CreateErrorResponse(404, "Not found.", context.Logger);
            }
        }
        catch (UserNotFoundException) when (action == "/password-recovery") { return RecoveryNotice(); }
        catch (InvalidParameterException) when (action == "/password-recovery") { return RecoveryNotice(); }
        catch (CodeMismatchException) { return ResponseBuilder.CreateErrorResponse(400, "The code is incorrect. Please check the email and try again.", context.Logger); }
        catch (ExpiredCodeException) { return ResponseBuilder.CreateErrorResponse(400, "The code has expired. Request a new code.", context.Logger); }
        catch (InvalidPasswordException) { return ResponseBuilder.CreateErrorResponse(400, "Use 8–16 characters with uppercase, lowercase, a number and a symbol.", context.Logger); }
        catch (NotAuthorizedException) { return ResponseBuilder.CreateErrorResponse(401, "Your session expired. Please sign in again.", context.Logger); }
        catch (LimitExceededException) { return ResponseBuilder.CreateErrorResponse(429, "Too many attempts. Please wait before trying again.", context.Logger); }
        catch (TooManyRequestsException) { return ResponseBuilder.CreateErrorResponse(429, "Too many attempts. Please wait before trying again.", context.Logger); }
        catch (AmazonCognitoIdentityProviderException)
        { return ResponseBuilder.CreateErrorResponse(400, "The request could not be completed. Check your details or try again later.", context.Logger); }
    }

    private static APIGatewayProxyResponse RecoveryNotice() => ResponseBuilder.CreateSuccessResponse(
        "If this account has a verified email, a recovery code has been sent. Check your inbox and spam folder. Email can be verified from My Profile while signed in.");
}
