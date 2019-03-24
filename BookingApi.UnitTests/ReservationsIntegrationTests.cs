﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Ploeh.Samples.BookingApi.UnitTests
{
    public class ReservationsIntegrationTests
    {
        [Fact]
        public async Task ReservationSucceeds()
        {
            var repo = new FakeReservationsRepository();
            var sut = new ReservationsController(10, repo);

            var reservation = new Reservation(
                date: new DateTimeOffset(2018, 8, 13, 16, 53, 0, TimeSpan.FromHours(2)),
                email: "mark@example.com",
                name: "Mark Seemann",
                quantity: 4);
            var actual = await sut.Post(reservation);

            Assert.True(repo.Contains(reservation.Accept()));
            var expectedId = repo.GetId(reservation.Accept());
            var ok = Assert.IsAssignableFrom<OkActionResult>(actual);
            Assert.Equal(expectedId, ok.Value);
        }

        [Fact]
        public async Task ReservationFails()
        {
            var repo = new FakeReservationsRepository();
            var sut = new ReservationsController(10, repo);

            var reservation = new Reservation(
                date: new DateTimeOffset(2018, 8, 13, 16, 53, 0, TimeSpan.FromHours(2)),
                email: "mark@example.com",
                name: "Mark Seemann",
                quantity: 11);
            var actual = await sut.Post(reservation);

            Assert.False(reservation.IsAccepted);
            Assert.False(repo.Contains(reservation));
            Assert.IsAssignableFrom<InternalServerErrorActionResult>(actual);
        }

        [Fact]
        public async Task ReadReservationsFromDatabaseThrowsException()
        {
            var repo = new FakeReservationsRepository().WithReadException(new DataException());
            var sut = new ReservationsController(10, repo);

            var reservation = new Reservation(
                date: new DateTimeOffset(2018, 8, 13, 16, 53, 0, TimeSpan.FromHours(2)),
                email: "mark@example.com",
                name: "Mark Seemann",
                quantity: 11);
            var actual = await sut.Post(reservation);

            Assert.False(reservation.IsAccepted);
            Assert.False(repo.Contains(reservation));
            Assert.IsAssignableFrom<InternalServerErrorActionResult>(actual);
        }

        [Fact]
        public async Task CreateReservationInDatabaseThrowsException()
        {
            var repo = new FakeReservationsRepository().WithCreateException(new TimeoutException());
            var sut = new ReservationsController(10, repo);

            var reservation = new Reservation(
                date: new DateTimeOffset(2018, 8, 13, 16, 53, 0, TimeSpan.FromHours(2)),
                email: "mark@example.com",
                name: "Mark Seemann",
                quantity: 11);
            var actual = await sut.Post(reservation);

            Assert.False(reservation.IsAccepted);
            Assert.False(repo.Contains(reservation));
            Assert.IsAssignableFrom<InternalServerErrorActionResult>(actual);
        }
    }
}
