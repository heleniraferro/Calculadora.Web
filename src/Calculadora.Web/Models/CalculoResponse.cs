namespace Calculadora.Web.Models;

public record CalculoResponse(
    Guid Id,
    decimal OperandoA,
    decimal OperandoB,
    Operacao Operacao,
    decimal Resultado,
    DateTimeOffset CriadoEm);
