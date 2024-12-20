﻿using ApplicationToSellThings.APIs.Models;
using ApplicationToSellThings.APIs.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApplicationToSellThings.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CardController : ControllerBase
    {
        private readonly ICardService _cardService;

        public CardController(ICardService cardService)
        {
            _cardService = cardService;
        }

        [Authorize(Policy = "UserPolicy")]
        [HttpPost]
        public async Task<IActionResult> AddCard([FromBody] CardRequestApiModel cardRequestApiModel)
        {
            var result = await _cardService.AddCardDetails(cardRequestApiModel);
            return Ok(result);
        }

        [Authorize(Policy = "UserPolicy")]
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetCardDetails(string userId)
        {
            var result = await _cardService.GetCardDetailsForUser(userId);
            return Ok(result);
        }
    }
}
