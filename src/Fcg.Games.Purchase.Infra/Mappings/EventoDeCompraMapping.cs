using Fcg.Games.Purchase.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fcg.Games.Purchase.Infra.Mappings;
public class EventoDeCompraMapping : IEntityTypeConfiguration<EventoDeCompraEntity>
{
    public void Configure(EntityTypeBuilder<EventoDeCompraEntity> builder)
    {
        builder.ToTable("EventosDeCompra");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.StreamId)
            .IsRequired();

        builder.Property(e => e.EventType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.EventData)
            .IsRequired()
            .HasColumnType("json");

        builder.Property(e => e.Timestamp)
            .IsRequired();
    }
}
