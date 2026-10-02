using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace ScheduleViewer.Web.Authentication;

/// <summary>
/// Attaches a Microsoft Entra access token only to requests sent to the configured API.
/// </summary>
public sealed class ApiAuthorizationMessageHandler : AuthorizationMessageHandler
{
    public ApiAuthorizationMessageHandler(
        IAccessTokenProvider provider,
        NavigationManager navigation,
        IConfiguration configuration)
        : base(provider, navigation)
    {
        var apiBaseUrl = RequireConfiguration(configuration, "ApiBaseUrl");
        var apiScope = RequireConfiguration(configuration, "AzureAd:ApiScope");

        ConfigureHandler(
            authorizedUrls: [apiBaseUrl],
            scopes: [apiScope]);
    }

    private static string RequireConfiguration(IConfiguration configuration, string key)
    {
        var value = configuration[key];
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException($"Configuration '{key}' is missing.");
        }

        return value;
    }
}
