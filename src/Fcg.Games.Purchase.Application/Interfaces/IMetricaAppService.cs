using Fcg.Games.Purchase.Application.Dtos.Metricas;

namespace Fcg.Games.Purchase.Application.Interfaces;
public interface IMetricaAppService
{
    Task<List<JogoMaisVendidoDto>> ContarVendasPorJogoAsync(int top);
}
