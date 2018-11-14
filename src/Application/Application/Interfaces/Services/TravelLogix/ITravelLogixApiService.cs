using Concepta.Application.Queries;
using System.Threading.Tasks;

namespace Concepta.Application.Interfaces.Services.TravelLogix
{
    /// <summary>
    /// The TravelLogixApiService, exposes only the necessary features to the application
    /// Authentication, authorization and other relative to the service engine, should not be exposed
    /// </summary>
    public interface ITravelLogixApiService
    {
        /// <summary>
        /// Makes a request on the TravelLogixApiService to get ticket availability
        /// </summary>
        Task<TicketAvailabilityQueryResponse> GetTickets(TicketAvailabilityQuery query);
    }
}
