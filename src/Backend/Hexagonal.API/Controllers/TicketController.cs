using Concepta.Application.Queries;
using Massena.Infrastructure.Core.Web;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Concepta.API.Controllers
{
    [Produces("application/json")]
    [Route("api/ticket")]
    public class TicketController : ApiController
    {
        public TicketController(IMediator mediator) : base(mediator) { }
        
        [HttpPost]
        [Authorize]
        public Task<TicketAvailabilityQueryResponse> search([FromBody] TicketAvailabilityQuery request)
        {
            return Mediator.Send<TicketAvailabilityQueryResponse>(request);
        }
    }
}