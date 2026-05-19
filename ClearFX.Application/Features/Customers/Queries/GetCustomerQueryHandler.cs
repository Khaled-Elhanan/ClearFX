using ClearFX.Domain.Entities;
using ClearFX.Domain.Interfaces;
using MediatR;

namespace ClearFX.Application.Features.Customers.Queries;

public class GetCustomerQueryHandler(IRepository<Customer> customerRepo)
    : IRequestHandler<GetCustomerQuery, CustomerDto>
{
    public async Task<CustomerDto> Handle(GetCustomerQuery request, CancellationToken cancellationToken)
    {
        var customer = await customerRepo.GetByIdAsync(request.CustomerId, cancellationToken)
                       ?? throw new KeyNotFoundException($"Customer '{request.CustomerId}' not found.");

        return new CustomerDto(
            customer.Id,
            customer.FullName,
            customer.NationalId,
            customer.Phone,
            customer.Email,
            customer.IsActive,
            customer.CreatedAt
        );
    }
}