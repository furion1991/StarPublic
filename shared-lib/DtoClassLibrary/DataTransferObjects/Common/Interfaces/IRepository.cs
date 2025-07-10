namespace DataTransferLib.DataTransferObjects.Common.Interfaces;

/// <summary>Интерфейс для обслуживания всех запросов к БД сущности требуемого класса</summary>
public interface IRepository<T>
    where T : class
{
    /// <summary>Метод фильтрации списка предметов по определённому условию</summary>
    IQueryable<T>? Filter(IQueryable<T> objs, DefaultRequest defaultRequest);
    /// <summary>Метод пагинации, фильтрации и сортировки</summary>
    Task<List<T>> GetOrderBy(DefaultRequest defaultRequest);
    Task<int> GetCount(DefaultRequest defaultRequest);

    Task Add(T entity, bool flush = false);
    Task Remove(T entity, bool flush = false);
    Task Save();
    Task Update(T entity, bool flush = false);

    Task<List<T>> GetAll();
    Task<T?> Get(string id);
}