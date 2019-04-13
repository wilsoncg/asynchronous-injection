﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Ploeh.Samples.BookingApi;
using MvcControllerBase = Microsoft.AspNetCore.Mvc.ControllerBase;
using IMvcActionResult = Microsoft.AspNetCore.Mvc.IActionResult;
using System.Net;

namespace BookingApi.WebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : MvcControllerBase
    {
        private readonly ActorService _actorService;

        public BookingController(ActorService actorService)
        {
            _actorService = actorService;
        }

        /// <summary>
        /// Creates a reservation
        /// </summary>
        /// <param name="reservation">reservation</param>
        /// <returns>Status indicating whether the reservation request was sucessful or not</returns>
        /// <response code="200">Performs the reservation</response>
        /// <response code="409">The reservation cannot be fulfilled</response>
        [ProducesResponseType(200)]
        [ProducesResponseType(409)]
        [HttpPost]
        public async Task<IMvcActionResult> Post([FromBody] Reservation reservation)
        {
            var r = await 
                new ReservationsController(10, new ReservationsRepository())
                .Post(reservation);
            if (r is InternalServerErrorActionResult)
                return
                    StatusCode(
                        (int)HttpStatusCode.Conflict,
                        (r as InternalServerErrorActionResult).Msg);
            return Ok((r as OkActionResult).Value);
        }
    }
}
