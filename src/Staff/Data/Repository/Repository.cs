using Microsoft.EntityFrameworkCore;
using Staff.Core;
using System.Linq.Expressions;

namespace Staff.Data
{
    class Repository<T, TKey> : IRepository<T, TKey>
        where TKey : IEquatable<TKey>
        where T : BaseEntity<TKey>
    {
        private readonly DataContext _dataContext;
        private readonly DbSet<T> _dbSet;

        public Repository(DataContext dataContext)
        {
            _dataContext = dataContext;
            _dbSet=dataContext.Set<T>();
        }

        public async Task Add(T item, CancellationToken cancellationToken = default)
        {
            _dbSet.Add(item);

            int resutl = await _dataContext.SaveChangesAsync(cancellationToken);
            if (resutl == 0)
                await Task.FromException(new Exception("002"));
        }

        public Task<List<T>> All(CancellationToken cancellationToken = default)
        {
            return _dbSet.ToListAsync(cancellationToken);
        }

        public Task<int> Count(CancellationToken cancellationToken = default)
        {
            return _dbSet.CountAsync(cancellationToken);
        }

        public async Task Delete(TKey id, CancellationToken cancellationToken = default)
        {
            _dbSet.Remove(await FindById(id));

            int resutl = await _dataContext.SaveChangesAsync(cancellationToken);
            if (resutl == 0)
                await Task.FromException(new Exception("004"));
        }

        public Task<T> FindById(TKey id, CancellationToken cancellationToken = default)
        {
            return _dbSet.SingleAsync((p)=>p.Id.Equals(id), cancellationToken);
        }

        public Task<T> Singal(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return _dbSet.SingleOrDefaultAsync(predicate, cancellationToken);
        }

        public async Task Update(T item, CancellationToken cancellationToken = default)
        {
            _dbSet.Update(item);

            int resutl = await _dataContext.SaveChangesAsync(cancellationToken);
            if (resutl == 0) 
                await Task.FromException(new Exception("003"));
        }

        public Task<List<T>> Where(Expression<Func<T,bool>> predicate, CancellationToken cancellationToken = default)
        {
            return _dbSet.Where(predicate).ToListAsync(cancellationToken);
        }
    }
}
