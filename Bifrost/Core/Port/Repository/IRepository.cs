namespace Bifrost.Core.Port.Repository;

public interface IRepository<T>
{
    public Task<List<T>> GetAll();
    public Task<T> Add(T domain);
    public Task<T> Update(T entity);
    public Task<T?> FindById(Guid id);
    public Task<bool> IdExists(Guid id);
    public Task DeleteById(Guid id);
}