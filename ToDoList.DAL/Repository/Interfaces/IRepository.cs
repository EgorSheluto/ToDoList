using System.Linq;
using System.Threading.Tasks;

namespace ToDoList.DAL.Repository.Interfaces
{
    public interface IRepository<T>
    {
        IQueryable<T> Query();

        IQueryable<T> Query(int id);

        IQueryable<T> QueryNoTracking();

        IQueryable<T> QueryNoTracking(int id);

        Task SaveAsync(T entity);

        Task UpdateAsync(T entity);

        Task DeleteAsync(T entity);
    }
}
