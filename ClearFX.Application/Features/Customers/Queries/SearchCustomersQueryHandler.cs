using ClearFX.Domain.Entities;
using ClearFX.Domain.Interfaces;
using MediatR;

namespace ClearFX.Application.Features.Customers.Queries;

public class SearchCustomersQueryHandler(IRepository<Customer> customerRepo)
    : IRequestHandler<SearchCustomersQuery, IEnumerable<CustomerDto>>
{
    public async Task<IEnumerable<CustomerDto>> Handle(SearchCustomersQuery request, CancellationToken cancellationToken)
    {
        var customers = string.IsNullOrWhiteSpace(request.SearchTerm)
            ? await customerRepo.GetAllAsync(cancellationToken)
            : await customerRepo.FindAsync(
                c => c.FullName.Contains(request.SearchTerm) ||
                     c.NationalId.Contains(request.SearchTerm) ||
                     (c.Phone != null && c.Phone.Contains(request.SearchTerm)),
                cancellationToken);

        return customers.Select(c => new CustomerDto(
            c.Id, c.FullName, c.NationalId, c.Phone, c.Email, c.IsActive, c.CreatedAt
        ));
    }
}