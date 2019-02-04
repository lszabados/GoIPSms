using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace VOXO.GISSM.Todos
{
    public interface ITodoAppService
    {
        Task<PagedResultDto<TodoDto>> GetListAsync();
    }
}