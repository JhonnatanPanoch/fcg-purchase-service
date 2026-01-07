using System.Runtime.Serialization;

namespace Fcg.Games.Purchase.Domain.Exceptions;

/// <summary>
/// Exceção para falha de validação de negócio (400)
/// </summary>
[Serializable]
public class BussinessException : CustomExceptionBase
{
    public BussinessException(string message) : base(message) { }

    public BussinessException(string message, Exception inner) : base(message, inner) { }

    protected BussinessException(SerializationInfo info, StreamingContext context) : base(info, context) { }
}
