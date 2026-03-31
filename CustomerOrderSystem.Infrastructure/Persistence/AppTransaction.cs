using CustomerOrderSystem.Domain.Abstractions;
using Microsoft.EntityFrameworkCore.Storage;

namespace CustomerOrderSystem.Infrastructure.Persistence;

public class AppTransaction(IDbContextTransaction transaction) : IAppTransaction
{
    public Task CommitAsync(CancellationToken cancellationToken = default)
    {
        return transaction.CommitAsync(cancellationToken);
    }

    public Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        return transaction.RollbackAsync(cancellationToken);
    }

    public ValueTask DisposeAsync()
    {
        return transaction.DisposeAsync();
    }
}

