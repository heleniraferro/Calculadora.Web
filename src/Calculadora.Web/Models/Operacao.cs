namespace Calculadora.Web.Models;

public enum Operacao
{
    Soma = 1,
    Subtracao = 2,
    Multiplicacao = 3,
    Divisao = 4
}

public static class OperacaoExtensions
{
    public static string Simbolo(this Operacao operacao) => operacao switch
    {
        Operacao.Soma => "+",
        Operacao.Subtracao => "−",
        Operacao.Multiplicacao => "×",
        Operacao.Divisao => "÷",
        _ => "?"
    };
}
