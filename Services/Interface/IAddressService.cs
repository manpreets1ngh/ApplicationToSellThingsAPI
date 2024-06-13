using ApplicationToSellThings.APIs.Models;

namespace ApplicationToSellThings.APIs.Services.Interface
{
    public interface IAddressService
    {
        Task<ResponseModel<AddressResponseViewModel>> AddAddress(AddressRequestApiModel addressRequestApiModel);
        Task<ResponseModel<AddressResponseViewModel>> GetAddressByUser(Guid userId);
    }
}
