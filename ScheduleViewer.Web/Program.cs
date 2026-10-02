using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Authentication.WebAssembly.Msal;
using ScheduleViewer.Web;
using ScheduleViewer.Web.Authentication;
using ScheduleViewer.Web.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? "http://localhost:9080/";
var authenticationEnabled = builder.Configuration.GetValue<bool>("AzureAd:Enabled");

if (authenticationEnabled)
{
    var authority = RequireConfiguration(builder.Configuration, "AzureAd:Authority");
    var clientId = RequireConfiguration(builder.Configuration, "AzureAd:ClientId");
    var apiScope = RequireConfiguration(builder.Configuration, "AzureAd:ApiScope");

    builder.Services.AddMsalAuthentication(options =>
    {
        options.ProviderOptions.Authentication.Authority = authority;
        options.ProviderOptions.Authentication.ClientId = clientId;
        options.ProviderOptions.Authentication.ValidateAuthority = true;
        options.ProviderOptions.LoginMode = "redirect";
        options.ProviderOptions.DefaultAccessTokenScopes.Add(apiScope);
    });

    builder.Services.AddScoped<ApiAuthorizationMessageHandler>();
    builder.Services
        .AddHttpClient("ScheduleViewerApi", client =>
        {
            client.BaseAddress = new Uri(apiBaseUrl);
            client.Timeout = TimeSpan.FromSeconds(20);
        })
        .AddHttpMessageHandler<ApiAuthorizationMessageHandler>();
    builder.Services.AddScoped(sp =>
        sp.GetRequiredService<IHttpClientFactory>().CreateClient("ScheduleViewerApi"));
}
else
{
    builder.Services.AddScoped(_ => new HttpClient
    {
        BaseAddress = new Uri(apiBaseUrl),
        Timeout = TimeSpan.FromSeconds(20)
    });
}

builder.Services.AddScoped<ScheduleViewerApiClient>();

await builder.Build().RunAsync();

static string RequireConfiguration(IConfiguration configuration, string key)
{
    var value = configuration[key];
    if (string.IsNullOrWhiteSpace(value))
    {
        throw new InvalidOperationException(
            $"Authentication is enabled, but configuration '{key}' is missing.");
    }

    return value;
}
