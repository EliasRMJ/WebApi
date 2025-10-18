using Microsoft.EntityFrameworkCore.Storage;
using PersistenceNet.Enuns;
using PersistenceNet.Structs;
using WebApi.ContextDB;

namespace WebApi.Managers
{
    public abstract class Manager
    {
        public WebApiContext _sGSContext;

        protected IDbContextTransaction? _transaction;
        protected int _transationManager;
        protected OperationReturn IsValid { get { return _return; } }
        protected bool IsActiveTransaction { get { return _transaction != null; } }

        private OperationReturn _return;

#pragma warning disable CS8618 
        public Manager(Manager? manager = null)
#pragma warning restore CS8618 
        {
            if (manager != null)
            {
                _transaction = manager._transaction;
                _sGSContext = manager._sGSContext;
                _transationManager = manager._transationManager;
            }

            _return = new OperationReturn { ReturnType = ReturnTypeEnum.Success };
        }

        async protected Task InitNewTransaction()
        {
            try
            {
                if (!IsActiveTransaction)
                    _transaction = await _sGSContext.Database.BeginTransactionAsync();

                Interlocked.Increment(ref _transationManager);
            }
            catch (Exception ex)
            {
                _transationManager = 0;

                throw new Exception("Erro ao iniciar transação! "
                    , ex);
            }
        }

        protected void Commit()
        {
            if (_transationManager.Equals(1))
            {
                _transaction?.Commit();

                _transaction = null;
                _transationManager = 0;
            }
            else
                Interlocked.Decrement(ref _transationManager);
        }

        protected void Rollback()
        {
            if (_transationManager.Equals(1))
            {
                _transaction!.Rollback();

                _transaction = null;
                _transationManager = 0;
            }
            else
                Interlocked.Decrement(ref _transationManager);
        }

        protected void Dispose()
        {
            _transaction?.Dispose();
        }
    }
}