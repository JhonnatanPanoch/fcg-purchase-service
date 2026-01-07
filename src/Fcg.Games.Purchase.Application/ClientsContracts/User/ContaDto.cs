using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fcg.Games.Purchase.Application.ClientsContracts.User;
public class ContaDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; }
    public string Email { get; set; }
    public List<JogosContaDto> Jogos { get; set; }
}
