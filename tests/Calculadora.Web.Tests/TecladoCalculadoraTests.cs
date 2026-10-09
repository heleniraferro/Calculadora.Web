using Calculadora.Web.Models;
using Calculadora.Web.Services;

namespace Calculadora.Web.Tests;

public class TecladoCalculadoraTests
{
    private readonly TecladoCalculadora _teclado = new();

    [Fact]
    public void Digitar_DeveSubstituirZeroInicialEConcatenar()
    {
        _teclado.Digitar(0);
        _teclado.Digitar(4);
        _teclado.Digitar(2);

        Assert.Equal("42", _teclado.Display);
    }

    [Fact]
    public void Virgula_DeveAceitarApenasUma()
    {
        _teclado.Digitar(1);
        _teclado.Virgula();
        _teclado.Virgula();
        _teclado.Digitar(5);

        Assert.Equal("1,5", _teclado.Display);
    }

    [Fact]
    public void Virgula_NoInicio_DeveComecarComZero()
    {
        _teclado.Virgula();

        Assert.Equal("0,", _teclado.Display);
    }

    [Fact]
    public void Apagar_DeveRemoverUltimoDigitoEVoltarAZero()
    {
        _teclado.Digitar(1);
        _teclado.Digitar(2);

        _teclado.Apagar();
        Assert.Equal("1", _teclado.Display);

        _teclado.Apagar();
        Assert.Equal("0", _teclado.Display);
    }

    [Fact]
    public void InverterSinal_DeveAlternarSinal()
    {
        _teclado.Digitar(7);

        _teclado.InverterSinal();
        Assert.Equal("-7", _teclado.Display);

        _teclado.InverterSinal();
        Assert.Equal("7", _teclado.Display);
    }

    [Fact]
    public void Igual_SemOperacaoPendente_NaoDeveGerarRequisicao()
    {
        _teclado.Digitar(5);

        Assert.Null(_teclado.PressionarIgual());
    }

    [Fact]
    public void Igual_ComOperacaoPendente_DeveGerarRequisicaoComOsDoisOperandos()
    {
        _teclado.Digitar(1);
        _teclado.Digitar(2);
        Assert.Null(_teclado.PressionarOperacao(Operacao.Divisao));
        _teclado.Digitar(4);

        var requisicao = _teclado.PressionarIgual();

        Assert.Equal(new CalcularRequest(12, 4, Operacao.Divisao), requisicao);
    }

    [Fact]
    public void Igual_ComOperandoDecimal_DeveInterpretarVirgula()
    {
        _teclado.Digitar(1);
        _teclado.Virgula();
        _teclado.Digitar(5);
        _teclado.PressionarOperacao(Operacao.Soma);
        _teclado.Digitar(2);
        _teclado.Virgula();
        _teclado.Digitar(2);
        _teclado.Digitar(5);

        var requisicao = _teclado.PressionarIgual();

        Assert.Equal(new CalcularRequest(1.5m, 2.25m, Operacao.Soma), requisicao);
    }

    [Fact]
    public void AplicarResultado_DeveMostrarResultadoComVirgula()
    {
        _teclado.Digitar(1);
        _teclado.PressionarOperacao(Operacao.Divisao);
        _teclado.Digitar(4);
        _teclado.PressionarIgual();

        _teclado.AplicarResultado(0.25m);

        Assert.Equal("0,25", _teclado.Display);
        Assert.Equal(string.Empty, _teclado.Expressao);
    }

    [Fact]
    public void Encadear_NovaOperacaoComPendente_ResolveAnteriorEContinuaComANova()
    {
        _teclado.Digitar(2);
        _teclado.PressionarOperacao(Operacao.Soma);
        _teclado.Digitar(3);

        var requisicao = _teclado.PressionarOperacao(Operacao.Multiplicacao);
        Assert.Equal(new CalcularRequest(2, 3, Operacao.Soma), requisicao);

        _teclado.AplicarResultado(5);
        Assert.Equal("5 ×", _teclado.Expressao);

        _teclado.Digitar(4);
        Assert.Equal(new CalcularRequest(5, 4, Operacao.Multiplicacao), _teclado.PressionarIgual());
    }

    [Fact]
    public void TrocarOperacao_SemNovoOperando_NaoDeveGerarRequisicao()
    {
        _teclado.Digitar(2);
        _teclado.PressionarOperacao(Operacao.Soma);

        Assert.Null(_teclado.PressionarOperacao(Operacao.Subtracao));
        Assert.Equal("2 −", _teclado.Expressao);
    }

    [Fact]
    public void AplicarErro_DeveReiniciarEstado()
    {
        _teclado.Digitar(9);
        _teclado.PressionarOperacao(Operacao.Divisao);
        _teclado.Digitar(0);
        _teclado.PressionarIgual();

        _teclado.AplicarErro();

        Assert.Equal("0", _teclado.Display);
        Assert.Equal(string.Empty, _teclado.Expressao);
    }

    [Fact]
    public void Digitar_DeveRespeitarLimiteDeDigitos()
    {
        for (var i = 0; i < 30; i++)
            _teclado.Digitar(1);

        Assert.Equal(15, _teclado.Display.Length);
    }
}
