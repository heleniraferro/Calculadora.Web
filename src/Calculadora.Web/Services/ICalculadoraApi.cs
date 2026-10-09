using Calculadora.Web.Models;

namespace Calculadora.Web.Services;

public interface ICalculadoraApi
{
    Task<CalculoResponse> CalcularAsync(CalcularRequest request, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<CalculoResponse>> ListarHistoricoAsync(int quantidade = 10, CancellationToken cancellationToken = default);
}
