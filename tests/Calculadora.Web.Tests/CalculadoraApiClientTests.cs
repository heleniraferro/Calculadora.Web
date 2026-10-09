using System.Net;
using System.Text;
using Calculadora.Web.Models;
using Calculadora.Web.Services;

namespace Calculadora.Web.Tests;

public class CalculadoraApiClientTests
{
    [Fact]
    public async Task Calcular_DeveEnviarOperacaoComoTextoEDesserializarResposta()
    {
        string? corpoEnviado = null;
        var cliente = CriarCliente(async requisicao =>
        {
            corpoEnviado = await requisicao.Content!.ReadAsStringAsync();
            return Json(HttpStatusCode.OK,
                """{"id":"3f2504e0-4f89-41d3-9a0c-0305e82c3301","operandoA":10,"operandoB":4,"operacao":"Divisao","resultado":2.5,"criadoEm":"2026-10-09T12:00:00+00:00"}""");
        });

        var resposta = await cliente.CalcularAsync(new CalcularRequest(10, 4, Operacao.Divisao));

        Assert.Contains("\"operacao\":\"Divisao\"", corpoEnviado);
        Assert.Equal(2.5m, resposta.Resultado);
        Assert.Equal(Operacao.Divisao, resposta.Operacao);
    }

    [Fact]
    public async Task Calcular_ErroDeDominio_DeveLancarApiExceptionComDetalhe()
    {
        var cliente = CriarCliente(_ => Task.FromResult(Json(HttpStatusCode.BadRequest,
            """{"title":"Operação inválida","detail":"Não é possível dividir por zero."}""")));

        var excecao = await Assert.ThrowsAsync<ApiException>(() =>
            cliente.CalcularAsync(new CalcularRequest(1, 0, Operacao.Divisao)));

        Assert.Equal("Não é possível dividir por zero.", excecao.Message);
    }

    [Fact]
    public async Task Calcular_FalhaDeConexao_DeveLancarApiException()
    {
        var cliente = CriarCliente(_ => throw new HttpRequestException());

        await Assert.ThrowsAsync<ApiException>(() =>
            cliente.CalcularAsync(new CalcularRequest(1, 1, Operacao.Soma)));
    }

    [Fact]
    public async Task ListarHistorico_DeveDesserializarLista()
    {
        var cliente = CriarCliente(_ => Task.FromResult(Json(HttpStatusCode.OK,
            """[{"id":"3f2504e0-4f89-41d3-9a0c-0305e82c3301","operandoA":1,"operandoB":2,"operacao":"Soma","resultado":3,"criadoEm":"2026-10-09T12:00:00+00:00"}]""")));

        var historico = await cliente.ListarHistoricoAsync(5);

        var item = Assert.Single(historico);
        Assert.Equal(3, item.Resultado);
    }

    private static CalculadoraApiClient CriarCliente(Func<HttpRequestMessage, Task<HttpResponseMessage>> manipulador) =>
        new(new HttpClient(new ManipuladorFalso(manipulador)) { BaseAddress = new Uri("https://localhost:7100/") });

    private static HttpResponseMessage Json(HttpStatusCode status, string corpo) =>
        new(status) { Content = new StringContent(corpo, Encoding.UTF8, "application/json") };

    private sealed class ManipuladorFalso(Func<HttpRequestMessage, Task<HttpResponseMessage>> manipulador) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) =>
            manipulador(request);
    }
}
