using Fcg.Games.Purchase.Application.Dtos.Compra;
using Fcg.Games.Purchase.Application.Interfaces;
using Fcg.Games.Purchase.Domain.Exceptions.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace Fcg.Games.Purchase.Api.Controllers;

/// <summary>
/// Responsável pelos endpoints de compras de jogos.
/// </summary>
[Authorize]
[ApiController]
[Produces("application/json")]
[Route("api/v{version:apiVersion}/[controller]")]
public class ComprasController : MainController
{
    private readonly ICompraAppService _service;
    private readonly IUsuarioAutenticadoAppService _userAppService;

    public ComprasController(
        ICompraAppService service,
        IUsuarioAutenticadoAppService userAppService)
    {
        _service = service;
        _userAppService = userAppService;
    }

    /// <summary>
    /// Inicia o processo de compra de jogos para o usuário autenticado.
    /// </summary>
    /// <param name="request">Dados da compra do jogo.</param>
    /// <returns>Informações do jogo adquirido.</returns>
    [SwaggerOperation(
        Summary = "Inicia a compra de uma lista de jogos.",
        Description = "Inicia o processo de compra de jogos para o usuário autenticado e retorna os detalhes da aquisição."
    )]
    [ProducesResponseType(typeof(CompraIniciadaDto), (int)HttpStatusCode.Accepted)]
    [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.Conflict)]
    [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.InternalServerError)]
    [HttpPost]
    public async Task<IActionResult> ComprarJogos([FromBody] ComprarJogoDto request)
    {
        return Accepted(await _service.IniciarCompraAsync(
            request, 
            _userAppService.ObterIdUsuario()));
    }

    [HttpGet("{compraId}")]
    public async Task<IActionResult> ConsultarAquisicao(Guid compraId)
    {
        return Ok(await _service.ConsultarAquisicaoAsync(compraId));
    }
}
