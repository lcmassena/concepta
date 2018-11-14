using Massena.Infrastructure.Core.Resilience.Http;
using Concepta.Application.Interfaces.Services.TravelLogix;
using Concepta.Application.Queries;
using Concepta.Services.TravelLogix.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Concepta.Services.TravelLogix
{
    /// <summary>
    /// Serviço de comunicação com a API TravelLogix
    /// Responsável por todas as funcionalidades relacionadas à ela
    /// </summary>
    public class TravelLogixApiService : ITravelLogixApiService
    {
        private const string BaseURL = "http://travellogix.api.test.conceptsol.com";
        private IHttpClient HttpClient;
        private string Token;

        public TravelLogixApiService(IHttpClient httpClient)
        {
            HttpClient = httpClient;
            Token = Authorize().Result;
        }

        private async Task<string> Authorize()
        {
            var form = new Dictionary<string, string>(new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("username", "test1@test2.com"),
                new KeyValuePair<string, string>("password", "Aa234567!"),
                new KeyValuePair<string, string>("grant_type", "password"),
            });

            var response = await HttpClient.PostForm<AuthenticationResponse>($"{BaseURL}/token", form);

            return response.access_token;
        }

        public Task<TravelLogixApiResponse> GetTickets(TravelLogixApiRequest request)
        {
            return HttpClient.PostAsync<TravelLogixApiRequest, TravelLogixApiResponse>($"{BaseURL}/api/Ticket/Search", request, Token);
        }

        public async Task<TicketAvailabilityQueryResponse> GetTickets(TicketAvailabilityQuery query)
        {
            // At service implementation, we should convert domain query to destination model
            TravelLogixApiRequest request = TravelLogixApiRequest.ToRequest(query);
            var response = await HttpClient.PostAsync<TravelLogixApiRequest, TravelLogixApiResponse>($"{BaseURL}/api/Ticket/Search", request, Token);

            return TravelLogixApiResponse.ToDomain(response);
        }
    }
}
