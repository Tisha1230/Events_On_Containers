﻿using EventCatalogAPI.Data;
using EventCatalogAPI.Domain;
using EventCatalogAPI.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventCatalogAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly EventContext _context;
            private readonly IConfiguration _config;
        //private readonly _config;
        public EventController (EventContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> Items([FromQuery] int pageIndex = 0, [FromQuery] int pageSize = 6)
        {
            var itemsCount = _context.Events.LongCountAsync();
            var items = await _context.Events
                  //.OrderBy (c=>c.EventName)
                  .Skip(pageIndex * pageSize)
                  .Take(pageSize)
                  .ToListAsync();

            items = ChangePictureUrl(items);
            var model = new PaginatedItemsViewModel
            {
                PageIndex = pageIndex,
                PageSize = items.Count,
                Count = itemsCount.Result,
                Data = items
        };

            return Ok(model);
        }

        private List<EachEvent> ChangePictureUrl(List<EachEvent> items)
        {
            items.ForEach(item=>
            item.PictureUrl=
            item.PictureUrl.Replace(
                "http://externalcatalogbaseurltobereplaced", _config["ExternalCatalogUrl"]));
            return items;
        }
    }
}
