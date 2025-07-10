using dv.autenticacion.auth0.Modelos;
namespace dv.autenticacion.auth0
{
    public  interface IHttpClientBase
    {

        /// <summary>
        /// API Health Check 
        /// </summary>
        /// <returns></returns>
        Task<HttpResponse> APIHealthCheck();
        Task<HttpResponse> PostRequest(string apiEndpoint, string jsonContent);
    }
}
