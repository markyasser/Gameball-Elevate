namespace Gameball_Elevate.Ops
{
    public interface IBasicOps<T> where T : class
    {
        public T? GetbyId(string id);
        public Task<T?> GetbyIdAsync(string id);
        public IEnumerable<T> GetAll();
        public Task<IEnumerable<T>> GetAllAsync();
        public IQueryable<T> GetAllQueryable();
        public void Create(T entity);
        public void Edit(T entity);
        public void EditRecursive(T entity);
        public void EditNonRecursive(T entity);
        public void Delete(string id);
        public void SaveChanges();
        public Task SaveChangesAsync();
        public bool Exists(string id);
    }
}
