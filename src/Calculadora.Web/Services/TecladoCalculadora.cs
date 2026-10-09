using System.Globalization;
using Calculadora.Web.Models;

namespace Calculadora.Web.Services;

/// <summary>
/// Estado do teclado da calculadora. Não calcula: quando há uma operação a resolver,
/// devolve o <see cref="CalcularRequest"/> para ser enviado à API e aguarda o resultado
/// em <see cref="AplicarResultado"/>.
/// </summary>
public class TecladoCalculadora
{
    private const int MaximoDeDigitos = 15;

    private decimal _acumulado;
    private Operacao? _pendente;
    private Operacao? _proximaOperacao;
    private bool _iniciarNovaEntrada = true;

    public string Display { get; private set; } = "0";

    public string Expressao => _pendente is null
        ? string.Empty
        : $"{Formatar(_acumulado)} {_pendente.Value.Simbolo()}";

    public void Digitar(int digito)
    {
        if (digito is < 0 or > 9)
            throw new ArgumentOutOfRangeException(nameof(digito));

        if (_iniciarNovaEntrada)
        {
            Display = digito.ToString(CultureInfo.InvariantCulture);
            _iniciarNovaEntrada = false;
            return;
        }

        if (QuantidadeDeDigitos() >= MaximoDeDigitos)
            return;

        Display = Display == "0" ? digito.ToString(CultureInfo.InvariantCulture) : Display + digito;
    }

    public void Virgula()
    {
        if (_iniciarNovaEntrada)
        {
            Display = "0,";
            _iniciarNovaEntrada = false;
            return;
        }

        if (!Display.Contains(','))
            Display += ",";
    }

    public void InverterSinal()
    {
        if (Display == "0")
            return;

        Display = Display.StartsWith('-') ? Display[1..] : "-" + Display;
    }

    public void Apagar()
    {
        if (_iniciarNovaEntrada)
            return;

        Display = Display.Length <= 1 || (Display.Length == 2 && Display.StartsWith('-'))
            ? "0"
            : Display[..^1];
    }

    public void Limpar()
    {
        Display = "0";
        _acumulado = 0;
        _pendente = null;
        _proximaOperacao = null;
        _iniciarNovaEntrada = true;
    }

    public CalcularRequest? PressionarOperacao(Operacao operacao)
    {
        if (_pendente is not null && !_iniciarNovaEntrada)
        {
            _proximaOperacao = operacao;
            return new CalcularRequest(_acumulado, ValorAtual(), _pendente.Value);
        }

        _acumulado = ValorAtual();
        _pendente = operacao;
        _iniciarNovaEntrada = true;
        return null;
    }

    public CalcularRequest? PressionarIgual()
    {
        if (_pendente is null || _iniciarNovaEntrada)
            return null;

        _proximaOperacao = null;
        return new CalcularRequest(_acumulado, ValorAtual(), _pendente.Value);
    }

    public void AplicarResultado(decimal resultado)
    {
        Display = Formatar(resultado);
        _acumulado = resultado;
        _pendente = _proximaOperacao;
        _proximaOperacao = null;
        _iniciarNovaEntrada = true;
    }

    public void AplicarErro() => Limpar();

    private decimal ValorAtual() =>
        decimal.Parse(Display.Replace(',', '.'), NumberStyles.Float, CultureInfo.InvariantCulture);

    private int QuantidadeDeDigitos() => Display.Count(char.IsDigit);

    private static string Formatar(decimal valor) =>
        valor.ToString(CultureInfo.InvariantCulture).Replace('.', ',');
}
