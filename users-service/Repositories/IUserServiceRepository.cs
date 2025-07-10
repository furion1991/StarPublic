namespace UsersService.Repositories;

public interface IUserServiceRepository<T> where T : class
{
    Task<List<T>?> GetAll();
    Task<T?> Get(string id, bool isReadonly = false);
    Task<bool> Add(T entity, bool flush = true);
    Task<bool> Update(T entity);
    Task<bool> Delete(string id);
}