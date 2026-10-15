using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Administradores.Models.Exceptions
{
    public class DataAccessException : Exception
    {
        public DataAccessException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}