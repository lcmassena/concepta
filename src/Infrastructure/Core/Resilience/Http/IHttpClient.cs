using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Massena.Infrastructure.Core.Resilience.Http
{
    public interface IHttpClient
    {
        Task<TResponse> PostForm<TResponse>(string uri, Dictionary<string, string> form, string authorizationToken = null, string requestId = null, string authorizationMethod = "Bearer");

        Task<string> GetStringAsync(string uri, string authorizationToken = null, string authorizationMethod = "Bearer");

        Task<TResponse> PostAsync<TRequest, TResponse>(string uri, TRequest item, string authorizationToken = null, string requestId = null, string authorizationMethod = "Bearer");

        Task<TResponse> DeleteAsync<TResponse>(string uri, string authorizationToken = null, string requestId = null, string authorizationMethod = "Bearer");

        Task<TResponse> PutAsync<TRequest, TResponse>(string uri, TRequest item, string authorizationToken = null, string requestId = null, string authorizationMethod = "Bearer");
    }
}
