using Fcg.Games.Purchase.Application.Dtos.Transacao;
using Fcg.Games.Purchase.Application.Interfaces;
using Fcg.Games.Purchase.Domain.Entities;
using Fcg.Games.Purchase.Domain.Interfaces;
using MassTransit.Initializers;
using Microsoft.Extensions.Logging;

namespace Fcg.Games.Purchase.Application.AppServices;
public class TransacaoAppService : ITransacaoAppService
{
    private readonly ILogger<CompraAppService> _logger;
    private readonly IRepository<TransacaoJogosEntity> _repository;

    public TransacaoAppService(
        ILogger<CompraAppService> logger, 
        IRepository<TransacaoJogosEntity> repository)
    {
        _logger = logger;
        _repository = repository;
    }

    public async Task<List<TransacaoJogosCompraDto>> ObterListaJogosCompradosAsync(Guid userId)
    {
        _logger.LogInformation("Consultando dados para: {@userId}", userId);

        var dados = await _repository.ObterAsync(x => x.UsuarioId == userId);

        return dados
            .GroupBy(x => x.CompraId)
            .Select(s => new TransacaoJogosCompraDto()
            {
                CompraId = s.Key,
                Jogos = s.Select(j => new TransacaoJogosDto() 
                { 
                    DataAquisicao = j.DataAquisicao,
                    JogoId = j.JogoId,
                    PrecoPago = j.PrecoPago
                })
            }).ToList();
    } 
}
