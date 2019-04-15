using Akka.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ploeh.Samples.BookingApi
{
    public class ReservationsRepository : IReservationsRepository
    {
        readonly List<Reservation> reservations;
        readonly ActorService _actors;

        public ReservationsRepository(ActorService actors)
        {
            reservations = new List<Reservation>();
            _actors = actors;
        }

        public Task<int> Create(Reservation reservation)
        {
            var result = CreateInternal(reservation);
            return result;
        }

        Task<int> CreateInternal(Reservation reservation)
        {
            _actors.CreateReservationActor.Tell(new ReservationForCustomer(reservation), null);
            //var r = await 
            return Task.FromResult(1);
        }

        public Task<Reservation[]> ReadReservations(DateTimeOffset date)
        {
            try
            {
                var firstTick = date.Date;
                var lastTick = firstTick.AddDays(1).AddTicks(-1);
                var filteredReservations = 
                    reservations
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

    // https://petabridge.com/blog/intro-to-persistent-actors/
    // https://petabridge.com/blog/akkadotnet-aspnet/
    // https://getakka.net/articles/actors/fault-tolerance.html
    // https://getakka.net/api/Akka.Event.DeadLetterListener.html
    public class CreateReservationActor : ReceivePersistentActor
    {
        public class GetMessages { }
        public override string PersistenceId => $"{new ActorHash().Hash}";

        // internal state
        List<ReservationForCustomer> _msgs = new List<ReservationForCustomer>();
        int _msgsSinceLastSnapshot = 0;

        public CreateReservationActor()
        {
            Recover<ReservationForCustomer>(m => _msgs.Add(m));
            Recover<SnapshotOffer>(offer =>
            {
                var messages = offer.Snapshot as List<ReservationForCustomer>;
                if (messages != null)
                    _msgs = _msgs.Concat(messages).ToList();
            });

            // commands
            Command<ReservationForCustomer>(r => Persist(r, s => {
                _msgs.Add(r); //add msg to in-memory event store after persisting
                if (++_msgsSinceLastSnapshot % 100 == 0)
                {
                    //time to save a snapshot
                    SaveSnapshot(_msgs);
                }
            }));
            Command<SaveSnapshotSuccess>(success => {
                // soft-delete the journal up until the sequence # at
                // which the snapshot was taken
                DeleteMessages(success.Metadata.SequenceNr);
            });
            Command<SaveSnapshotFailure>(failure => {
                // handle snapshot save failure...
            });

            //Command<GetMessages>(_ => Sender.Tell(new List<ReservationForCustomer>(_msgs)));
        }
    }

    public class ActorHash
    {
        public string Hash => $"{Environment.MachineName}";        
    }

    public class ReservationForCustomer : IComparable<ReservationForCustomer>
    {
        public Reservation Reservation;
        public int CustomerId;

        public ReservationForCustomer(Reservation reservation)
        {
            Reservation = reservation;
        }

        public int CompareTo(ReservationForCustomer other)
        {
            var equal =
                (this.CustomerId == other.CustomerId &&
                this.Reservation.Date == other.Reservation.Date &&
                this.Reservation.Email == other.Reservation.Email &&
                this.Reservation.Name == other.Reservation.Name &&
                this.Reservation.Quantity == other.Reservation.Quantity) ? 0 : 1;

            return equal;
        }
    }
}
