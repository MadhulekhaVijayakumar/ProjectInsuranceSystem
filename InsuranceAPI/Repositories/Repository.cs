using InsuranceAPI.Context;
using InsuranceAPI.Interfaces;

namespace InsuranceAPI.Repositories
{
    public abstract class Repository<K, T> : ReadOnlyRepository<K, T>, IRepository<K, T> where T : class
    {
        protected Repository(InsuranceManagementContext context) : base(context) { }

        public async Task<T> Add(T entity)
        {
            _context.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<T> Update(K key, T entity)
        {
            var data = await GetById(key);
            if (data == null) throw new Exception($"Entity with key {key} not found.");
            _context.Entry(data).CurrentValues.SetValues(entity);
            await _context.SaveChangesAsync();
            return data;
        }

        public async Task<T> Delete(K key)
        {
            var data = await GetById(key);
            if (data == null) throw new Exception($"Entity with key {key} not found.");
            _context.Remove(data);
            await _context.SaveChangesAsync();
            return data;
        }
    }
}