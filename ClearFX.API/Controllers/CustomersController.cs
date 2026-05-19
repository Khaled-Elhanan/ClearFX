using ClearFX.Application.Features.Customers.Commands;
using ClearFX.Application.Features.Customers.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClearFX.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CustomersController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [Authorize(Policy = "CanManageCustomers")] 
    public async Task<IActionResult> Create(CreateCustomerCommand command)
    {
        var result = await mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = result.CustomerId }, result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await mediator.Send(new GetCustomerQuery(id));
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> Search([FromQuery] string? search)
    {
        var result = await mediator.Send(new SearchCustomersQuery(search));
        return Ok(result);
    }
}