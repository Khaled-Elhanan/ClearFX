using ClearFX.Domain.Interfaces;

namespace ClearFX.Infrastructure.Persistence;

public class UnitOfWork(AppDbContext  context):IUnitOfWork
{
    public  async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => await context.SaveChangesAsync(cancellationToken);

    public void Dispose() => context.Dispose();
}