using Fcg.Games.Purchase.Domain.Entities;
using Fcg.Games.Purchase.Domain.Events;

namespace Fcg.Games.Purchase.Domain.Interfaces;
public interface IEventStoreRepository
{
    Task SalvarEventoAsync(EventBase evento);
    Task<T?> ObterEventoAsync<T>(Guid streamId) where T : EventBase;
}