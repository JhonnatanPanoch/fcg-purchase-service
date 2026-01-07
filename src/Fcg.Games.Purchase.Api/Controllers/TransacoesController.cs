using Fcg.Games.Purchase.Application.Dtos.Transacao;
using Fcg.Games.Purchase.Application.Interfaces;
using Fcg.Games.Purchase.Domain.Exceptions.Responses;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace Fcg.Games.Purchase.Api.Controllers;
public class TransacoesController : MainController
{
    private readonly ITransacaoAppService _service;

    public TransacoesController(ITransacaoAppService service)
    {
        _service = service;
    }

    /// <summary>
    /// Consulta uma lista de jogos adquiridos por usuário.
    /// </summary>
    /// <param name="userId">Id de um usuário.</param>
    /// <returns>Informações do jogo adquirido.</returns>
    [SwaggerOperation(
        Summary = "Consulta dados de transação.",
        Description = "Consulta uma lista de jogos adquiridos por usuário."
    )]
    [ProducesResponseType(typeof(List<TransacaoJogosCompraDto>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.InternalServerError)]
    [HttpGet("{userId:guid}")]
    public async Task<IActionResult> ConsultarTransacao([FromRoute] Guid userId)
    {
        return Ok(await _service.ObterListaJogosCompradosAsync(userId));
    }
}
