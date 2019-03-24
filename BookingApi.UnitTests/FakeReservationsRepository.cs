using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Ploeh.Samples.BookingApi.UnitTests
{
    public class FakeReservationsRepository : IReservationsRepository
    {
        Exception _readException;
        Exception _createException;
        private readonly List<Reservation> reservations;

        public FakeReservationsRepository()
        {
            reservations = new List<Reservation>();
        }

        public FakeReservationsRepository WithReadException(Exception e)
        {
            _readException = e;
            return this;
        }

        public FakeReservationsRepository WithCreateException(Exception e)
        {
            _createException = e;
            return this;
        }

        public Task<int> Create(Reservation reservation)
        {
            if (_readException != null)
                throw _readException;

            reservations.Add(reservation);
            // Hardly a robut implementation, since indices will be reused,
            // but should be good enough for the purpose of a pair of
            // integration tests
            return Task.FromResult(reservations.IndexOf(reservation));
        }

        public Task<Reservation[]> ReadReservations(DateTimeOffset date)
        {
            try
            {
                if (_readException != null)
                    throw _readException;

                var firstTick = date.Date;
                var lastTick = firstTick.AddDays(1).AddTicks(-1);
                var filteredReservations = reservations
                    .Where(r => firstTick <= r.Date && r.Date <= lastTick)
                    .ToArray();
                return Task.FromResult(filteredReservations);
            }
            catch (Exception e)
            {
                return Task.FromResult(Enumerable.Empty<Reservation>().ToArray());
            }
        }

        public bool Contains(Reservation reservation)
        {
            return reservations.Contains(reservation);
        }

        public int GetId(Reservation reservation)
        {
            return reservations.IndexOf(reservation);
        }
    }
}
