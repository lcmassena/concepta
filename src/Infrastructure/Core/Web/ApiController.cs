using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Massena.Infrastructure.Core.Web
{

    public abstract class ApiController : ControllerBase
    {
        protected readonly IMediator Mediator;

        protected ApiController(IMediator mediator)
        {
            Mediator = mediator;
        }

        protected IActionResult ResponseQuery(object commandResult)
        {
            if (commandResult == null)
                return NotFound(commandResult);

            return Ok(commandResult);
        }

        protected async Task<IActionResult> ResponseQueryAsync(Task<object> commandResult)
        {
            if (commandResult == null)
                return NotFound(await commandResult);

            return Ok(await commandResult);
        }
    }
}
