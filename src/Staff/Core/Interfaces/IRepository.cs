using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Staff.Core
{
    public interface IRepository<T,TKey> 
        where TKey :IEquatable<TKey>
        where T : BaseEntity<TKey>
    {
        Task<int> Count(CancellationToken cancellationToken = default);
        Task<List<T>> All(CancellationToken cancellationToken = default);
        Task<T> FindById(TKey id, CancellationToken cancellationToken = default);
        Task<T> Singal(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
        Task<List<T>> Where(Expression<Func<T,bool>> predicate,CancellationToken cancellationToken = default);
        Task Add(T item, CancellationToken cancellationToken = default);
        Task Update(T item, CancellationToken cancellationToken = default);
        Task Delete(TKey id, CancellationToken cancellationToken = default);
    }
}
