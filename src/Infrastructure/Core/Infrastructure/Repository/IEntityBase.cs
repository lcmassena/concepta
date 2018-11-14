using System;

namespace Massena.Infrastructure.Core.Infrastructure.Repository
{
    public interface IEntityBase
    {
        string UsuarioInclusao { get; set; }
        DateTime DataInclusao { get; set; }
        string UsuarioAlteracao { get; set; }
        DateTime? DataAlteracao { get; set; }
    }
}
