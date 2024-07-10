using Microsoft.EntityFrameworkCore;


namespace Gameball_Elevate.Ops
{
    public abstract class BasicOps<T> : IBasicOps<T> where T : class
    {
        public BasicOps(DbContext dbContext)
        {
            DbContext = dbContext;
            DbSet = dbContext.Set<T>();
        }
        public virtual T? GetbyId(string id)
        {
            return DbSet.Find(id);
        }
        public virtual async Task<T?> GetbyIdAsync(string id)
        {
            var entity = await DbSet.FindAsync(id);
            return entity;
        }

        public virtual IEnumerable<T> GetAll()
        {
            return DbSet.ToList();
        }
        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            var entities = await DbSet.ToListAsync();
            return entities;
        }
        public virtual IQueryable<T> GetAllQueryable()
        {
            return DbSet.AsQueryable();
        }
        public virtual void Create(T entity)
        {
            DbSet.Add(entity);
        }
        public virtual void Edit(T entity)
        {
            EditRecursive(entity);
        }
        public virtual void EditRecursive(T entity)
        {
            var attach = DbSet.Update(entity);
            attach.State = EntityState.Modified;
        }
        public virtual void EditNonRecursive(T entity)
        {
            var entry = DbSet.Entry(entity);
            entry.State = EntityState.Modified;
        }
        public virtual void Delete(string id)
        {
            T? entity = GetbyId(id);
            if (entity != null)
            {
                DbSet.Remove(entity);
            }
        }
        public virtual bool Exists(string id)
        {
            return DbSet.Find(id) != null;
        }
        public virtual async Task<bool> ExistsAsync(string id)
        {
            var entity = await DbSet.FindAsync(id);
            return entity != null;
        }
        public virtual void SaveChanges()
        {
            DbContext.SaveChanges();
        }
        public virtual async Task SaveChangesAsync()
        {
            await DbContext.SaveChangesAsync();
        }
        public virtual void Detach(T Entity)
        {
            DbContext.Entry(Entity).State = EntityState.Detached;
        }

        protected DbContext DbContext { get; set; }
        protected DbSet<T> DbSet { get; set; }
    }
}
