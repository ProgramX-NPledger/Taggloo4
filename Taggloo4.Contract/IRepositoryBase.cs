using System.Linq.Expressions;

namespace Taggloo4.Contract;

public interface IRepositoryBase<T>
{
    IQueryable<T> FindAll();
    IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression);
    
    /// <summary>
    /// Creates the entity, ready for calling <seealso cref="SaveAllAsync"/>.
    /// </summary>
    /// <param name="entity">Entity to create.</param>
    void Create(T entity);
    
    /// <summary>
    /// Marks the entity as having been updated, ready for calling <seealso cref="SaveAllAsync"/>.
    /// </summary>
    /// <param name="entity">Entity to mark as updated.</param>
    void Update(T entity);
    void Delete(T entity);
}