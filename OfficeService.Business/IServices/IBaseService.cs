namespace OfficeService.Business.IServices
{
    public interface IBaseService<T> : IGenericService<T> where T : class
    {
    }
}
