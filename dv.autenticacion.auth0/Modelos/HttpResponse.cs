using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace dv.autenticacion.auth0.Modelos
{
    public class HttpResponse
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public HttpStatusCode? StatusCode { get; set; }
    }
}
