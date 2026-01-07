using Fcg.Games.Purchase.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fcg.Games.Purchase.Infra.Mappings;
public class AquisicaoMapping : IEntityTypeConfiguration<TransacaoJogosEntity>
{
    public void Configure(EntityTypeBuilder<TransacaoJogosEntity> builder)
    {
        builder.ToTable("TransacoesJogos");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.UsuarioId)
            .IsRequired();

        builder.Property(a => a.JogoId)
            .IsRequired();

        builder.Property(a => a.DataAquisicao)
            .IsRequired();

        builder.Property(a => a.PrecoPago)
            .IsRequired()
            .HasColumnType("decimal(10,2)");

        builder.Property(a => a.Status)
            .IsRequired();
    }
}
