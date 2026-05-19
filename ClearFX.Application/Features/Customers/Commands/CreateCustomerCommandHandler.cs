using ClearFX.Domain.Entities;
using ClearFX.Domain.Interfaces;
using MediatR;
using ClearFX.Application.Common;

namespace ClearFX.Application.Features.Customers.Commands;

public class CreateCustomerCommandHandler(
    IRepository<Customer> customerRepo,
    IUnitOfWork uow,
    ICurrentUserService currentUser)
    : IRequestHandler<CreateCustomerCommand, CreateCustomerResult>
{
    public async Task<CreateCustomerResult> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        var existing = await customerRepo.FindAsync(c => c.NationalId == request.NationalId, cancellationToken);
        if (existing.Any())
            throw new InvalidOperationException($"Customer with NationalId '{request.NationalId}' already exists.");

        var customer = new Customer
        {
            FullName   = request.FullName,
            NationalId = request.NationalId,
            Phone      = request.Phone,
            Email      = request.Email,
            CreatedBy  = currentUser.UserId
        };

        await customerRepo.AddAsync(customer, cancellationToken);
        await uow.SaveChangesAsync(cancellationToken);

        return new CreateCustomerResult(customer.Id, customer.FullName, customer.NationalId);
    }
}
