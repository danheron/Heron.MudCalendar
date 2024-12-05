using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Heron.MudCalendar.Docs;
using Heron.MudCalendar.Docs.Services;
using MudBlazor.Docs.Extensions;
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

builder.Services.AddScoped<EventService>();

await builder.Build().RunAsync();
