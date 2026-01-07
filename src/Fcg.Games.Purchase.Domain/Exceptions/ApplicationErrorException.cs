using System.Runtime.Serialization;

namespace Fcg.Games.Purchase.Domain.Exceptions;

/// <summary>
/// Exceção para erros inesperados na aplicação (500)
/// </summary>
[Serializable]
public class ApplicationErrorException : CustomExceptionBase
{
    public ApplicationErrorException() : base("Erro inesperado na aplicação.") { }

    public ApplicationErrorException(string message) : base(message) { }

    public ApplicationErrorException(string message, Exception inner) : base(message, inner) { }

    protected ApplicationErrorException(SerializationInfo info, StreamingContext context) : base(info, context) { }
}
