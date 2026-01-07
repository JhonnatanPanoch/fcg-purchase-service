using Fcg.Games.Purchase.Application.AppServices;
using Fcg.Games.Purchase.Application.Dtos.Metricas;
using Fcg.Games.Purchase.Application.Interfaces;
using Fcg.Games.Purchase.Domain.Exceptions.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace Fcg.Games.Purchase.Api.Controllers;

/// <summary>
/// Responsável pelas métricas de compras de jogos.
/// </summary>
[Authorize]
[ApiController]
[Produces("application/json")]
[Route("api/v{version:apiVersion}/[controller]")]
public class MetricasController : MainController
{
    private readonly IMetricaAppService _service;

    public MetricasController(IMetricaAppService service)
    {
        _service = service;
    }

    /// <summary>
    /// Consulta uma lista dos jogos mais vendidos na plataforma.
    /// </summary>
    /// <param name="top">Quantidade de resultados.</param>
    /// <returns>Consulta uma lista dos jogos mais vendidos na plataforma..</returns>
    [SwaggerOperation(
        Summary = "Lista de Jogos mais vendidos.",
        Description = "Consulta uma lista de jogos adquiridos por usuário."
    )]
    [ProducesResponseType(typeof(List<JogoMaisVendidoDto>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.InternalServerError)]
    [HttpGet("mais-vendidos")]
    public async Task<IActionResult> ObterMaisVendidos([FromQuery] int top = 5)
    {
        var resultado = await _service.ContarVendasPorJogoAsync(top);
        return Ok(resultado);
    }
}
