
namespace Fcg.Games.Purchase.Application.Interfaces;

public interface IUsuarioAutenticadoAppService
{
    string? ObterEmail();
    Guid ObterIdUsuario();
}