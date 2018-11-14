using Concepta.Application.Interfaces.Services.TravelLogix;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Concepta.Application.Queries
{
    /// <summary>
    /// This is the handler to Query for Room Availability
    /// </summary>
    public class TicketAvailabilityQueryHandler : 
        IRequestHandler<TicketAvailabilityQuery, TicketAvailabilityQueryResponse>,
        IPipelineBehavior<TicketAvailabilityQuery, TicketAvailabilityQueryResponse>
    {
        private ITravelLogixApiService Repository;

        /// <summary>
        /// Constructor with injected dependencies
        /// </summary>
        /// <param name="repository">The Room Repository injected by IOC framework</param>
        public TicketAvailabilityQueryHandler(ITravelLogixApiService repository) => Repository = repository;

        public async Task<TicketAvailabilityQueryResponse> Handle(TicketAvailabilityQuery request, CancellationToken cancellationToken)
        {
            // Business rules should be applied here
            var tickets = Repository.GetTickets(request);

            return await tickets;
        }

        public Task<TicketAvailabilityQueryResponse> Handle(TicketAvailabilityQuery request, CancellationToken cancellationToken, RequestHandlerDelegate<TicketAvailabilityQueryResponse> next)
        {
            System.Diagnostics.Trace.WriteLine("Pipeline behavior executed before Query Handler");

            return next();
        }
    }
}
