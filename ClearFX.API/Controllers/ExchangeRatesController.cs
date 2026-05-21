using ClearFX.API.Authorization;
using ClearFX.Application.Features.ExchangeRates.Commands;
using ClearFX.Application.Features.ExchangeRates.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClearFX.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ExchangeRatesController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [Authorize(Policy = Policies.CanManageRates)]
    public async Task<IActionResult> SetRate(SetExchangeRateCommand command)
    {
        var result = await mediator.Send(command);
        return CreatedAtAction(nameof(GetActiveRate), 
            new { from = result.FromCurrency, to = result.ToCurrency }, result);
    }

    [HttpGet("active")]
    public async Task<IActionResult> GetActiveRate([FromQuery] string from, [FromQuery] string to)
    {
        var result = await mediator.Send(new GetActiveRateQuery(from, to));
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] bool activeOnly = true)
    {
        var result = await mediator.Send(new GetAllRatesQuery(activeOnly));
        return Ok(result);
    }
}