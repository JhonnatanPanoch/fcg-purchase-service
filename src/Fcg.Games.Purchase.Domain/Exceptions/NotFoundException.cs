using System.Runtime.Serialization;

namespace Fcg.Games.Purchase.Domain.Exceptions;

[Serializable]
public class NotFoundException : CustomExceptionBase
{
    public NotFoundException() : base("Não foi possível localizar os dados solicitados.") { }

    public NotFoundException(string message) : base(message) { }

    public NotFoundException(string message, Exception inner) : base(message, inner) { }

    protected NotFoundException(SerializationInfo info, StreamingContext context) : base(info, context) { }
}
