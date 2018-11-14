using System;

namespace Massena.Infrastructure.Core.Infrastructure
{
    public interface IUnitOfWork: IDisposable
    {
        void BeginTransaction();
        void Rollback();
        void Commit();
    }
}
