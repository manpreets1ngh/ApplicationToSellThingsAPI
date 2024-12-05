﻿using ApplicationToSellThings.APIs.Models;
using ApplicationToSellThings.APIs.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApplicationToSellThings.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatusController : ControllerBase
    {
        private readonly IStatusService _statusService;

        public StatusController(IStatusService statusService)
        {
            _statusService = statusService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetStatusByStatusId(int id)
        {
            var result = await _statusService.GetStatusById(id);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllStatuses()
        {
            var result = await _statusService.GetAllStatuses();
            return Ok(result);
        }
    }
}
