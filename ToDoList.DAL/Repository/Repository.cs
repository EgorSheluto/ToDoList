using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using ToDoList.DAL.Contexts;
using ToDoList.DAL.Models.Base;
using ToDoList.DAL.Repository.Interfaces;

namespace ToDoList.DAL.Repository
{
    public class Repository<T> : IRepository<T> where T : BaseModel
    {
        private Context _repositoryContext { get; set; }

        public Repository(Context repositoryContext)
        {
            _repositoryContext = repositoryContext;
        }

        public async Task DeleteAsync(T entity)
        {
            _repositoryContext.Set<T>().Remove(entity);
            await _repositoryContext.SaveChangesAsync();
        }

        public IQueryable<T> Query()
        {
            return _repositoryContext.Set<T>()
                                     .AsQueryable();
        }

        public IQueryable<T> Query(int id)
        {
            return _repositoryContext.Set<T>()
                                     .Where(s => s.Id == id)
                                     .AsQueryable();
        }

        public IQueryable<T> QueryNoTracking()
        {
            return Query().AsNoTracking();
        }

        public IQueryable<T> QueryNoTracking(int id)
        {
            return Query(id).AsNoTracking();
        }

        public async Task SaveAsync(T entity)
        {
            _repositoryContext.Set<T>().Add(entity);
            await _repositoryContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(T entity)
        {
            _repositoryContext.Set<T>().Update(entity);
            await _repositoryContext.SaveChangesAsync();
        }
    }
}
