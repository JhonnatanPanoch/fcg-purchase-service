using Fcg.Games.Purchase.Application.ClientsContracts.Jogo;
using Fcg.Games.Purchase.Application.ClientsContracts.User;
using Fcg.Games.Purchase.Application.Dtos.Compra;
using Fcg.Games.Purchase.Application.Interfaces;
using Fcg.Games.Purchase.Domain.Entities;
using Fcg.Games.Purchase.Domain.Events;
using Fcg.Games.Purchase.Domain.Exceptions;
using Fcg.Games.Purchase.Domain.Interfaces;
using Fcg.Games.Purchase.Domain.ValueObjects;
using MassTransit;
using Microsoft.Extensions.Logging;
using System.Threading;

namespace Fcg.Games.Purchase.Application.AppServices;
public class CompraAppService : ICompraAppService
{
    private readonly ILogger<CompraAppService> _logger;
    private readonly IRepository<TransacaoJogosEntity> _aquisicaoRepository;
    private readonly IEventStoreRepository _eventStore;
    private readonly IBus _bus;
    private readonly IGamesServiceClient _jogosServiceClient;
    private readonly IUserServiceClient _userClient;

    public CompraAppService(
        ILogger<CompraAppService> logger,
        IEventStoreRepository eventStore,
        IBus bus,
        IRepository<TransacaoJogosEntity> aquisicaoRepository,
        IGamesServiceClient jogosServiceClient,
        IUserServiceClient userClient)
    {
        _logger = logger;
        _eventStore = eventStore;
        _bus = bus;
        _aquisicaoRepository = aquisicaoRepository;
        _jogosServiceClient = jogosServiceClient;
        _userClient = userClient;
    }

    public async Task<CompraIniciadaDto> IniciarCompraAsync(ComprarJogoDto request, Guid idUsuario)
    {
        _logger.LogInformation("Tentativa de compra: {@request}", request);

        var precoDetalhado = await _jogosServiceClient.ConsultarPrecosAsync(request.IdJogos);

        var infoJogoCompra = precoDetalhado.Jogos.Select(j => new JogoCompradoInfo(j.Id, j.Preco)).ToList();

        var precoTotal = precoDetalhado.Jogos.Sum(j => j.Preco);

        var conta = await _userClient.ConsultarDadosContaAsync() ?? 
            throw new NotFoundException($"Usuário não encontrado na base de dados. id: {idUsuario}");
        
        var compraId = Guid.NewGuid();

        var evento = new CompraIniciadaEvent(
            compraId,
            idUsuario,
            conta.Email ?? string.Empty,
            infoJogoCompra,
            precoTotal
        );

        await _eventStore.SalvarEventoAsync(evento);

        await _bus.Publish(evento);

        return new CompraIniciadaDto(compraId, precoTotal);
    }

    public async Task<TransacaoJogosEntity?> ConsultarAquisicaoAsync(Guid compraId)
    {
        var aquisicao = await _aquisicaoRepository.ObterPorIdAsync(compraId);
        
        return aquisicao is null ? 
            throw new NotFoundException() :
            aquisicao;
    }
}