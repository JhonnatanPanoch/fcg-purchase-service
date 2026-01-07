namespace Fcg.Games.Purchase.Application.ClientsContracts.User;
public interface IUserServiceClient
{
    /// <summary>
    /// Obtém os dados da conta do usuário autenticado.
    /// </summary>
    /// <returns>Dados da conta.</returns>
    Task<ContaDto> ConsultarDadosContaAsync();

    /// <summary>
    /// Obtém os detalhes de um usuário específico pelo seu ID.
    /// </summary>
    /// <param name="id">ID do usuário.</param>
    Task<UsuarioDto> ConsultarPorIdAsync(Guid id);
}
