using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dv.autenticacion.auth0
{
    public interface IAuthService
    {
        /// <summary>
        /// Obtains an Auth0 token for API access.
        /// </summary>
        /// <returns></returns>
        Task<string> GetAuth0TokenAsync();
    }
}
