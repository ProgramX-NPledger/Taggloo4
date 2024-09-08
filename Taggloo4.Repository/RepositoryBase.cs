using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Taggloo4.Contract;
using Taggloo4.Data.EntityFrameworkCore;

namespace Taggloo4.Repository;

public abstract class RepositoryBase<T> : IRepositoryBase<T> where T : class
{
    protected DataContext DataContext { get; set; } 
    public RepositoryBase(DataContext dataContext) 
    {
        DataContext = dataContext; 
    }
    public IQueryable<T> FindAll() => DataContext.Set<T>().AsNoTracking();
    public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression) => 
        DataContext.Set<T>().Where(expression).AsNoTracking();
    public void Create(T entity) => DataContext.Set<T>().Add(entity);
    public void Update(T entity) => DataContext.Set<T>().Update(entity);
    public void Delete(T entity) => DataContext.Set<T>().Remove(entity);
}