using Fcg.Games.Purchase.Domain.Enums;

namespace Fcg.Games.Purchase.Application.ClientsContracts.User;
public class UsuarioDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; }
    public string Email { get; set; }
    public RoleEnum Role { get; set; }
}
