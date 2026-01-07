using Fcg.Games.Purchase.Domain.Entities;
using Fcg.Games.Purchase.Domain.Events;
using Fcg.Games.Purchase.Domain.Interfaces;
using Fcg.Games.Purchase.Infra.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Fcg.Games.Purchase.Infra.Repositories;
public class EventStoreRepository(AppDbContext context) : Repository<EventoDeCompraEntity>(context), IEventStoreRepository
{
    public async Task<T?> ObterEventoAsync<T>(Guid streamId) where T : EventBase
    {
        var primeiroEventoArmazenado = await _context.EventsStore
            .Where(e => e.StreamId == streamId)
            .OrderBy(e => e.Timestamp)
            .FirstOrDefaultAsync();

        if (primeiroEventoArmazenado is null)
        {
            return null;
        }

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var eventoOriginal = JsonSerializer.Deserialize<T>(primeiroEventoArmazenado.EventData, options);

        return eventoOriginal;
    }

    public async Task SalvarEventoAsync(EventBase evento)
    {
        var storedEvent = new EventoDeCompraEntity
        {
            Id = evento.EventId,
            StreamId = evento.StreamId,
            EventType = evento.GetType().Name,
            EventData = JsonSerializer.Serialize(evento, evento.GetType()),
            Timestamp = evento.Timestamp
        };

        await AdicionarAsync(storedEvent);
    }
}
