namespace DataTransferLib.DataTransferObjects.Common.Interfaces;

public interface IConverter<T, G>
    where T : class
    where G : IDefaultDto
{
    G? Convert();
    G Convert(T entity);
    List<G> Convert(List<T> entities);
}