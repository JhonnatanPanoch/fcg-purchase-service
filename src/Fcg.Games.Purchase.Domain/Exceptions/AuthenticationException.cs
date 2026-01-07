using System.Runtime.Serialization;

namespace Fcg.Games.Purchase.Domain.Exceptions;

[Serializable]
public class AuthenticationException : CustomExceptionBase
{
    public AuthenticationException() : base("Não foi possível realizar a autenticação.") { }

    public AuthenticationException(string message) : base(message) { }

    public AuthenticationException(string message, Exception inner) : base(message, inner) { }

    protected AuthenticationException(SerializationInfo info, StreamingContext context) : base(info, context) { }
}
