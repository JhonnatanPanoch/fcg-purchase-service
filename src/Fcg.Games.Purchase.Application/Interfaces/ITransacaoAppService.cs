using Fcg.Games.Purchase.Application.Dtos.Transacao;

namespace Fcg.Games.Purchase.Application.Interfaces;
public interface ITransacaoAppService
{
    Task<List<TransacaoJogosCompraDto>> ObterListaJogosCompradosAsync(Guid userId);
}
