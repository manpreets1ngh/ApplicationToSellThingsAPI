﻿using ApplicationToSellThings.APIs.Areas.Identity.Data;
using ApplicationToSellThings.APIs.Data;
using ApplicationToSellThings.APIs.Models;
using ApplicationToSellThings.APIs.Services.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ApplicationToSellThings.APIs.Services
{
    public class CardService : ICardService
    {
        private readonly ApplicationToSellThingsAPIIdentityContext _dbContext;
        private readonly UserManager<ApplicationToSellThingsAPIsUser> _userManager;
        public CardService(ApplicationToSellThingsAPIIdentityContext dbContext, UserManager<ApplicationToSellThingsAPIsUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        public async Task<ResponseModel<CardResponseApiModel>> AddCardDetails(CardRequestApiModel cardRequestApiModel)
        {
            var user = await _userManager.FindByIdAsync(cardRequestApiModel.UserId);
            if (user != null)
            {
                var cardModel = new CardModel
                {
                    CardId = Guid.NewGuid(),
                    UserId = cardRequestApiModel.UserId,
                    CardHolderName = cardRequestApiModel.CardHolderName,
                    CardNumber = cardRequestApiModel.CardNumber,
                    ExpiryDate = cardRequestApiModel.ExpiryDate,
                    Cvv = cardRequestApiModel.Cvv,
                    AddedOn = DateTime.Now,
                };

                _dbContext.CardDetails.Add(cardModel);
                try
                {
                    await _dbContext.SaveChangesAsync();
                    var resultResponse = new CardResponseApiModel
                    {
                        CardHolderName = cardModel.CardHolderName,
                        CardNumber = cardModel.CardNumber,
                        ExpiryDate = cardModel.ExpiryDate,
                        Cvv = cardModel.Cvv,
                        AddedOn = DateTime.Now
                    };
                    var response = new ResponseModel<CardResponseApiModel>()
                    {
                        StatusCode = 200,
                        Status = "Success",
                        Message = "Card added Successfully",
                        Data = resultResponse,
                    };

                    return response;
                }
                catch (Exception ex)
                {
                    return new ResponseModel<CardResponseApiModel>
                    {
                        StatusCode = 500,
                        Message = ex.Message,
                    };
                }
            }
            return new ResponseModel<CardResponseApiModel>
            {
                StatusCode = 401,
                Status = "Failure",
                Message = "User Not Found"
            };
        }

        public async Task<ResponseModel<CardResponseApiModel>> GetCardDetailsForUser(string userId)
        {
            List<CardResponseApiModel> cardResponseApiModel = new List<CardResponseApiModel>();
            var cardDetails = await _dbContext.CardDetails.Where(a => a.UserId == userId).ToListAsync();

            if (cardDetails != null)
            {
                foreach (var card in cardDetails)
                {
                    var cardResponseData = new CardResponseApiModel()
                    {
                        CardId = card.CardId,
                        CardHolderName = card.CardHolderName,
                        CardNumber = card.CardNumber,
                        ExpiryDate = card.ExpiryDate,
                        Cvv = card.Cvv,
                        AddedOn = card.AddedOn
                    };

                    cardResponseApiModel.Add(cardResponseData);
                }

                var response = new ResponseModel<CardResponseApiModel>()
                {
                    StatusCode = 200,
                    Status = "Success",
                    Message = "Card Founded Successfully",
                    Items = cardResponseApiModel,
                };

                return response;
            }
            return new ResponseModel<CardResponseApiModel>
            {
                StatusCode = 401,
                Status = "Failure",
                Message = "Card Not Found"
            };

        }
    }
}
