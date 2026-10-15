using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Administradores.Models.Formularios.Models;

namespace Administradores.BR.Interfaces.Repositories
{
    public interface IHabilitacionFormularioRepository
    {
        Task<IEnumerable<HabilitacionFormulario>> ListarAsync();

        Task CrearAsync(HabilitacionFormulario formulario);

        Task EditarAsync(HabilitacionFormulario formulario);

        Task EliminarAsync(int id);
    }
}