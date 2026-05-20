using ClearFX.Application.Features.Wallets.Commands;
using ClearFX.Application.Features.Wallets.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClearFX.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WalletsController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [Authorize(Policy = "CanManageCustomers")]
    public async Task<IActionResult> Create(CreateWalletCommand command)
    {
        var result = await mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = result.WalletId }, result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await mediator.Send(new GetWalletQuery(id));
        return Ok(result);
    }

    [HttpGet("customer/{customerId:guid}")]
    public async Task<IActionResult> GetByCustomer(Guid customerId)
    {
        var result = await mediator.Send(new GetCustomerWalletsQuery(customerId));
        return Ok(result);
    }
}