using Microsoft.AspNetCore.Mvc.ApiExplorer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Ploeh.Samples.BookingApi
{
    public class Reservation
    {
        public Reservation(
            DateTimeOffset date,
            string email,
            string name,
            int quantity) : this(date, email, name, quantity, false)
        { }

        private Reservation(DateTimeOffset date,
            string email,
            string name,
            int quantity,
            bool isAccepted)
        {
            Date = date;
            Email = email;
            Name = name;
            Quantity = quantity;
            IsAccepted = isAccepted;
        }

        [Required]
        public DateTimeOffset Date { get; }

        [Required, EmailAddress, DefaultValue("person@email.com")]
        public string Email { get; set;  }

        [Required, DefaultValue("name")]
        public string Name { get; set; }

        [Required, DefaultValue(1)]
        public int Quantity { get; set; }

        public bool IsAccepted { get; }

        public Reservation Accept()
        {
            return new Reservation(
                Date,
                Email,
                Name,
                Quantity,
                true);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Reservation other))
                return false;

            return Equals(Date, other.Date)
                && Equals(Email, other.Email)
                && Equals(Name, other.Name)
                && Equals(Quantity, other.Quantity)
                && Equals(IsAccepted, other.IsAccepted);
        }

        public override int GetHashCode()
        {
            return
                Date.GetHashCode() ^
                Email.GetHashCode() ^
                Name.GetHashCode() ^
                Quantity.GetHashCode() ^
                IsAccepted.GetHashCode();
        }
    }
}
