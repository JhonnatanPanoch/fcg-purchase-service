using Fcg.Games.Purchase.Application.Dtos.Compra;
using Fcg.Games.Purchase.Domain.Entities;

namespace Fcg.Games.Purchase.Application.Interfaces;

public interface ICompraAppService
{
    Task<TransacaoJogosEntity?> ConsultarAquisicaoAsync(Guid compraId);
    Task<CompraIniciadaDto> IniciarCompraAsync(ComprarJogoDto request, Guid idUsuario);
}