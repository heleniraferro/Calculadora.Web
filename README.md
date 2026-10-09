# Calculadora.Web

Front-end de uma calculadora básica em **Blazor WebAssembly** (.NET 10). Todas as contas são feitas pela API, e o histórico vem do banco.

API: [Calculadora.Api](https://github.com/heleniraferro/Calculadora.Api)

## Estrutura

```
src/Calculadora.Web
├─ Models/      Operacao, CalcularRequest, CalculoResponse
├─ Services/    ICalculadoraApi + CalculadoraApiClient (HttpClient tipado)
│               TecladoCalculadora (estado do teclado, sem UI e testável)
├─ Components/  Tecla.razor
├─ Pages/       Principal.razor (visor, teclado e histórico)
└─ wwwroot/appsettings.json   URL da API
tests/Calculadora.Web.Tests   xUnit
```

## Como executar

Requisitos: .NET 10 SDK e a [Calculadora.Api](https://github.com/heleniraferro/Calculadora.Api) em execução.

```bash
dotnet run --project src/Calculadora.Web --launch-profile https
```

O front sobe em `https://localhost:7200`. A URL da API fica em `src/Calculadora.Web/wwwroot/appsettings.json` (`ApiBaseUrl`, padrão `https://localhost:7100/`). A API precisa liberar a origem do front em `Cors:Origens`.

## Testes

```bash
dotnet test
```
