using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Ploeh.Samples.BookingApi;
using MvcControllerBase = Microsoft.AspNetCore.Mvc.ControllerBase;

namespace BookingApi.WebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : MvcControllerBase
    {
        // POST api/booking/reservation
        [HttpPost]
        public async void Post([FromBody] Reservation reservation)
        {
            var r = await new ReservationsController(10, null).Post(reservation);
        }
    }
}
