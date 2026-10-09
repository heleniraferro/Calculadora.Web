using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Calculadora.Web.Models;

namespace Calculadora.Web.Services;

public class CalculadoraApiClient : ICalculadoraApi
{
    private static readonly JsonSerializerOptions OpcoesJson = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() }
    };

    private readonly HttpClient _http;

    public CalculadoraApiClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<CalculoResponse> CalcularAsync(CalcularRequest request, CancellationToken cancellationToken = default)
    {
        using var resposta = await EnviarAsync(() => _http.PostAsJsonAsync("api/calculos", request, OpcoesJson, cancellationToken));

        if (!resposta.IsSuccessStatusCode)
            throw new ApiException(await LerMensagemDeErroAsync(resposta, cancellationToken));

        return await resposta.Content.ReadFromJsonAsync<CalculoResponse>(OpcoesJson, cancellationToken)
            ?? throw new ApiException("A API retornou uma resposta vazia.");
    }

    public async Task<IReadOnlyList<CalculoResponse>> ListarHistoricoAsync(int quantidade = 10, CancellationToken cancellationToken = default)
    {
        using var resposta = await EnviarAsync(() => _http.GetAsync($"api/calculos?quantidade={quantidade}", cancellationToken));

        if (!resposta.IsSuccessStatusCode)
            throw new ApiException(await LerMensagemDeErroAsync(resposta, cancellationToken));

        return await resposta.Content.ReadFromJsonAsync<List<CalculoResponse>>(OpcoesJson, cancellationToken) ?? [];
    }

    private static async Task<HttpResponseMessage> EnviarAsync(Func<Task<HttpResponseMessage>> envio)
    {
        try
        {
            return await envio();
        }
        catch (HttpRequestException)
        {
            throw new ApiException("Não foi possível conectar à API. Verifique se ela está em execução.");
        }
    }

    private static async Task<string> LerMensagemDeErroAsync(HttpResponseMessage resposta, CancellationToken cancellationToken)
    {
        try
        {
            var problema = await resposta.Content.ReadFromJsonAsync<ProblemaApi>(OpcoesJson, cancellationToken);
            if (!string.IsNullOrWhiteSpace(problema?.Detail))
                return problema.Detail;
        }
        catch (JsonException)
        {
        }

        return $"A API retornou um erro ({(int)resposta.StatusCode}).";
    }

    private sealed record ProblemaApi(string? Title, string? Detail);
}
