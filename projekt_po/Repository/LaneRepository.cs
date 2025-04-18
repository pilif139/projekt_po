using Microsoft.EntityFrameworkCore;
using projekt_po.Database;
using projekt_po.Model;

namespace projekt_po.Repository;

public interface ILaneRepository
{
    Lane? Get(int laneId);
    List<Lane>? GetAll();
    Lane? GetByNumber(int number);
    List<Lane> GetByStatus(LaneStatus status);
    List<Lane> GetByUserId(int userId);
    Lane Add(int number, LaneStatus status, decimal price, int userId);
    bool Delete(int laneId);
    bool Update(Lane lane);
}

public class LaneRepository : ILaneRepository
{
    private readonly DatabaseContext _db;
    
    public LaneRepository(DatabaseContext db)
    {
        _db = db;
    }
    
    public Lane? Get(int laneId)
    {
        return _db.Lanes.Include(l => l.User).FirstOrDefault(x => x.Id == laneId);
    }

    public List<Lane> GetAll()
    {
        return _db.Lanes.OrderBy(x => x.Id).Include(l => l.User).ToList();
    }

    public Lane? GetByNumber(int number)
    {
        return _db.Lanes.Include(l => l.User).FirstOrDefault(x => x.Number == number);
    }

    public List<Lane> GetByStatus(LaneStatus status)
    {
        return _db.Lanes.Where(x => x.Status == status).Include(l => l.User).ToList();
    }

    public List<Lane> GetByUserId(int userId)
    {
        return _db.Lanes.Where(x => x.UserId == userId).Include(l => l.User).ToList();
    }

    public Lane Add(int number, LaneStatus status, decimal price, int userId)
    {
        var lane = new Lane(status, number,price, userId);
        _db.Lanes.Add(lane);
        _db.SaveChanges();
        return lane;
    }

    public bool Delete(int laneId)
    {
        var lane = _db.Lanes.Find(laneId);
        if (lane == null) return false;
        _db.Lanes.Remove(lane);
        _db.SaveChanges();
        return true;
    }

    public bool Update(Lane lane)
    {
        var existingLane = _db.Lanes.Find(lane.Id);
        if (existingLane == null) return false;
        existingLane.Status = lane.Status;
        existingLane.Number = lane.Number;
        existingLane.Price = lane.Price;
        existingLane.UserId = lane.UserId;
        _db.SaveChanges();
        return true;
    }
}