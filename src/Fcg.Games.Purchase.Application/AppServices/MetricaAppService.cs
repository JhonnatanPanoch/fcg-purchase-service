using Fcg.Games.Purchase.Application.Dtos.Metricas;
using Fcg.Games.Purchase.Application.Interfaces;
using Fcg.Games.Purchase.Domain.Entities;
using Fcg.Games.Purchase.Domain.Enums;
using Fcg.Games.Purchase.Domain.Interfaces;

namespace Fcg.Games.Purchase.Application.AppServices;
public class MetricaAppService : IMetricaAppService
{
    private readonly IRepository<TransacaoJogosEntity> _transacaoRepository;

    public MetricaAppService(IRepository<TransacaoJogosEntity> transacaoRepository)
    {
        _transacaoRepository = transacaoRepository;
    }

    public async Task<List<JogoMaisVendidoDto>> ContarVendasPorJogoAsync(int top)
    {
        var dados = await _transacaoRepository.ObterAsync(t => t.Status == EStatusCompra.Finalizado);

        return dados
            .GroupBy(t => t.JogoId)
            .Select(g => new JogoMaisVendidoDto(g.Key, g.Count()))
            .OrderByDescending(x => x.QuantidadeVendas)
            .Take(top)
            .ToList();
    }
}
