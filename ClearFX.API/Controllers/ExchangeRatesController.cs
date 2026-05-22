using ClearFX.API.Authorization;
using ClearFX.Application.Features.ExchangeRates;
using ClearFX.Application.Features.ExchangeRates.Commands;
using ClearFX.Application.Features.ExchangeRates.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClearFX.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ExchangeRatesController(IMediator mediator, IRateSyncService rateSyncService) : ControllerBase
{
   
    [HttpPost("manual")]
    [Authorize(Policy = Policies.CanManageRates)]
    public async Task<IActionResult> SetManualRate(SetExchangeRateCommand command)
    {
        var result = await mediator.Send(command);
        return CreatedAtAction(nameof(GetActiveRate),
            new { from = result.FromCurrency, to = result.ToCurrency }, result);
    }

    
    [HttpPost("sync")]
    [Authorize(Policy = Policies.CanManageRates)]
    public async Task<IActionResult> SyncRate([FromQuery] string from, [FromQuery] string to)
    {
        await rateSyncService.SyncRateAsync(from.ToUpper(), to.ToUpper());
        return Ok(new { message = $"Rate synced successfully for {from.ToUpper()}/{to.ToUpper()}" });
    }

    
    [HttpPost("sync/all")]
    [Authorize(Policy = Policies.CanManageRates)]
    public async Task<IActionResult> SyncAllRates()
    {
        await rateSyncService.SyncAllRatesAsync();
        return Ok(new { message = "All rates synced successfully." });
    }

    
    [HttpGet("active")]
    public async Task<IActionResult> GetActiveRate([FromQuery] string from, [FromQuery] string to)
    {
        var result = await mediator.Send(new GetActiveRateQuery(from, to));
        return Ok(result);
    }

    // Get all rates (active or history)
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] bool activeOnly = true)
    {
        var result = await mediator.Send(new GetAllRatesQuery(activeOnly));
        return Ok(result);
    }
}