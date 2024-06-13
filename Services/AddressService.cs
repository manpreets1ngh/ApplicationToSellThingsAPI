using ApplicationToSellThings.APIs.Areas.Identity.Data;
using ApplicationToSellThings.APIs.Data;
using ApplicationToSellThings.APIs.Models;
using ApplicationToSellThings.APIs.Services.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ApplicationToSellThings.APIs.Services
{
    public class AddressService : IAddressService
    {
        private readonly ApplicationToSellThingsAPIIdentityContext _dbContext;
        private readonly UserManager<ApplicationToSellThingsAPIsUser> _userManager;

        public AddressService(ApplicationToSellThingsAPIIdentityContext dbContext, UserManager<ApplicationToSellThingsAPIsUser> userManager)
        { 
            _dbContext = dbContext;
            _userManager = userManager;
        }

        public async Task<ResponseModel<AddressResponseViewModel>> AddAddress(AddressRequestApiModel addressRequestApiModel)
        {
            var user = await _userManager.FindByIdAsync(addressRequestApiModel.UserId);
            if(user != null)
            {
                var address = new AddressModel
                {
                    Id = Guid.NewGuid(),
                    UserId = addressRequestApiModel.UserId,
                    Street = addressRequestApiModel.Street,
                    City = addressRequestApiModel.City,
                    State = addressRequestApiModel.State,
                    PostCode = addressRequestApiModel.PostCode,
                    Country = addressRequestApiModel.Country,
                };

                _dbContext.Addresses.Add(address);
                try
                {
                    await _dbContext.SaveChangesAsync();
                    var response = new ResponseModel<AddressResponseViewModel>()
                    {
                        StatusCode = 200,
                        Status = "Success",
                        Message = "Address added Successfully",
                        Data = new AddressResponseViewModel
                        {
                            Id = address.Id,
                            UserId = address.UserId,
                            Street = address.Street,
                            City = address.City,
                            State = address.State,
                            PostCode = address.PostCode,
                            Country = address.Country
                        },
                    };

                    return response;
                }
                catch(Exception ex)
                {
                    return new ResponseModel<AddressResponseViewModel>
                    {
                        StatusCode = 500,
                        Message = ex.Message,
                    };
                }
            }
            return new ResponseModel<AddressResponseViewModel>
            {
                StatusCode = 401,
                Status = "Failure",
                Message = "User Not Found"
            };
        }

        public async Task<ResponseModel<AddressResponseViewModel>> GetAddressByUser(Guid userId)
        {
            List<AddressResponseViewModel> addressResponseViewModel = new List<AddressResponseViewModel>();
            string id = userId.ToString();
            var addresses = await _dbContext.Addresses.Where(a => a.UserId == id).ToListAsync();

            if(addresses != null)
            {
                foreach (var address in addresses)
                {
                    var addressResponseData = new AddressResponseViewModel()
                    {
                        Id = address.Id,
                        UserId = address.UserId,
                        Street = address.Street,
                        City = address.City,
                        State = address.State,
                        PostCode = address.PostCode,
                        Country = address.Country
                    };

                    addressResponseViewModel.Add(addressResponseData);
                }

                var response = new ResponseModel<AddressResponseViewModel>()
                {
                    StatusCode = 200,
                    Status = "Success",
                    Message = "Address Founded Successfully",
                    Items = addressResponseViewModel,
                };

                return response;
            }
            return new ResponseModel<AddressResponseViewModel>
            {
                StatusCode = 401,
                Status = "Failure",
                Message = "Address Not Found"
            };

        }

    }
}
