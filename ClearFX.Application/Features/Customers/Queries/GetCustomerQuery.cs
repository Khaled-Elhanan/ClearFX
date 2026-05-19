using MediatR;

namespace ClearFX.Application.Features.Customers.Queries;

public record GetCustomerQuery(Guid CustomerId) : IRequest<CustomerDto>;

public record CustomerDto(
    Guid Id,
    string FullName,
    string NationalId,
    string Phone,
    string? Email,
    bool IsActive,
    DateTimeOffset CreatedAt
);