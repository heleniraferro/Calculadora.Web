using Calculadora.Web;
using Calculadora.Web.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var urlDaApi = builder.Configuration["ApiBaseUrl"]
    ?? throw new InvalidOperationException("ApiBaseUrl não configurada em wwwroot/appsettings.json.");

builder.Services.AddHttpClient<ICalculadoraApi, CalculadoraApiClient>(http => http.BaseAddress = new Uri(urlDaApi));

await builder.Build().RunAsync();
