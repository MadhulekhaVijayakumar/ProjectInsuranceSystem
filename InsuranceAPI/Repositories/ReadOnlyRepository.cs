using InsuranceAPI.Context;
using InsuranceAPI.Interfaces;

namespace InsuranceAPI.Repositories
{
    public abstract class ReadOnlyRepository<K, T> : IReadOnlyRepository<K, T> where T : class
    {
        protected readonly InsuranceManagementContext _context;

        protected ReadOnlyRepository(InsuranceManagementContext context)
        {
            _context = context;
        }

        public abstract Task<IEnumerable<T>> GetAll();
        public abstract Task<T> GetById(K key);
    }
}