﻿using projekt_po.Model;
using projekt_po.Database;

namespace projekt_po.Repository;

public interface IReservationRepository
{
    Reservation? Get(int reservationId);
    List<Reservation> GetAll(int userId);
    Reservation? GetByDate(DateTime date);
    Reservation Add(int userId, string details, DateTime reservationDate);
    bool Delete(int reservationId);
}

public class ReservationRepository : IReservationRepository
{
    private readonly DatabaseContext _db;

    public ReservationRepository(DatabaseContext db) 
    {
        _db = db;
    }
    
    public Reservation? Get(int reservationId)
    {
        return _db.Reservations.Find(reservationId);
    }
    
    public List<Reservation> GetAll(int userId){
        return _db.Reservations
            .Where(r => r.UserId == userId)
            .ToList();
    }

    public Reservation? GetByDate(DateTime date)
    {
        return _db.Reservations.FirstOrDefault(r => r.Date == date);
    }

    public Reservation Add(int userId, string details, DateTime reservationDate)
    {
        var reservation = new Reservation
        {
            UserId = userId,
            Details = details,
            Date = reservationDate 
        };

        _db.Reservations.Add(reservation);
        _db.SaveChanges();
        Console.WriteLine("Reservation added successfully");
        return reservation;
    }

    public bool Delete(int reservationId)
    {
        var reservation = _db.Reservations.Find(reservationId);
        if (reservation == null)
        {
            return false;
        }
        _db.Reservations.Remove(reservation);
        _db.SaveChanges();
        
        return true;
    }
}