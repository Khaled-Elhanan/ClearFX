using MediatR;

namespace ClearFX.Application.Features.Customers.Commands;

public record CreateCustomerCommand(
    string FullName,
    string NationalId,
    string Phone,
    string? Email
) : IRequest<CreateCustomerResult>;

public record CreateCustomerResult(Guid CustomerId, string FullName, string NationalId);