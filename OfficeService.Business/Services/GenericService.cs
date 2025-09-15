using OfficeService.Business.IServices;
using OfficeService.DAL.IRepository;

namespace OfficeService.Business.Services
{
    public abstract class GenericService<T> : IGenericService<T> where T : class
    {
        protected readonly IGenericRepository<T> rp;

        public GenericService(IGenericRepository<T> rp)
        {
            this.rp = rp;
        }
    }
}
