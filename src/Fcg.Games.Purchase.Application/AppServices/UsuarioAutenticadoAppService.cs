using Fcg.Games.Purchase.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Fcg.Games.Purchase.Application.AppServices;
public class UsuarioAutenticadoAppService : IUsuarioAutenticadoAppService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UsuarioAutenticadoAppService(
        IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? ObterEmail()
    {
        var user = _httpContextAccessor.HttpContext?.User;
        if (user == null)
            return null;

        return user.FindFirst(ClaimTypes.Email)?.Value;
    }

    public Guid ObterIdUsuario()
    {
        var user = _httpContextAccessor.HttpContext?.User;
        if (user == null || !Guid.TryParse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value, out Guid userId))
            throw new ApplicationException();

            return userId;
    }
}
