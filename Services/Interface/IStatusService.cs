using ApplicationToSellThings.APIs.Models;

namespace ApplicationToSellThings.APIs.Services.Interface
{
    public interface IStatusService
    {
        Task<ResponseModel<StatusModel>> GetStatusById(int id);
        Task<ResponseModel<StatusModel>> GetAllStatuses();
    }
}
