using Blazor.Analytics;
using BytexDigital.Blazor.Components.CookieConsent;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Heron.MudCalendar.Docs;
using Heron.MudCalendar.Docs.Services;
using MudBlazor.Docs.Pages.Consent.Prompt;
using MudBlazor.Docs.Services;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddMudServices();
//builder.Services.TryAddDocsViewServices();

builder.Services.AddScoped<IDocsJsApiService, DocsJsApiService>();
builder.Services.AddSingleton<IMenuService, MenuService>();
builder.Services.AddScoped<IDocsNavigationService, DocsNavigationService>();
builder.Services.AddSingleton<IRenderQueueService, RenderQueueService>();

builder.Services.AddGoogleAnalytics("G-QMN1W7PK2X");
builder.Services.AddCookieConsent(options =>
{
    options.ImportJsAutomatically = false;
    options.Revision = 1;
    options.PolicyUrl = "/mudcalendar/cookie-policy";

    // Replace default prompt. We don't use the modal.
    options.ConsentPromptVariant = new MudCookieConsentPromptVariant();

    options.Categories.Add(new CookieCategory
    {
        TitleText = new()
        {
            ["en"] = "Google Services",
        },
        DescriptionText = new()
        {
            ["en"] = "Allows the integration and usage of Google services.",
        },
        Identifier = "google",
        IsPreselected = true,
        Services =
        [
            new CookieCategoryService
            {
                Identifier = "google-analytics",
                PolicyUrl = "https://policies.google.com/privacy",
                TitleText = new()
                {
                    ["en"] = "Google Analytics",
                },
                ShowPolicyText = new()
                {
                    ["en"] = "Display policies",
                }
            }
        ]
    });
});

builder.Services.AddScoped<EventService>();

await builder.Build().RunAsync();
