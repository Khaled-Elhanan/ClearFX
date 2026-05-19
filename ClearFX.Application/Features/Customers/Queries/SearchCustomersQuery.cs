using MediatR;

namespace ClearFX.Application.Features.Customers.Queries;

public record SearchCustomersQuery(string? SearchTerm) : IRequest<IEnumerable<CustomerDto>>;