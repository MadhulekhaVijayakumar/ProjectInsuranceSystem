namespace InsuranceAPI.Interfaces
{
    public interface IReadOnlyRepository<K, T> where T : class
    {
        Task<IEnumerable<T>> GetAll();
        Task<T> GetById(K key);
    }
}
