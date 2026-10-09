namespace Calculadora.Web.Services;

public class ApiException : Exception
{
    public ApiException(string mensagem) : base(mensagem)
    {
    }
}
