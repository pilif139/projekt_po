using projekt_po.Model;
using projekt_po.Repository;
using projekt_po.Utils;
using Spectre.Console;

namespace projekt_po.Services;

public class LaneService : BaseService,IModelService<Lane>
{
    private const Resource Resource = Services.Resource.Lane;
    private readonly IRbacService _rbacService;
    private readonly ILaneRepository _laneRepository;
    private readonly IReservationRepository _reservationRepository;
    
    public LaneService(IRbacService rbacService, ILaneRepository laneRepository, IReservationRepository reservationRepository, ILogger logger) : base(logger)
    {
        _rbacService = rbacService;
        _laneRepository = laneRepository;
        _reservationRepository = reservationRepository;
    }
    
    public bool Add(Lane lane)
    {
        if(!_rbacService.CheckPermission(Resource, Permission.Create)) return false;
        ValidateLane(lane);
        _laneRepository.Add(lane.Number, lane.Status, lane.Price, lane.UserId);
        AnsiConsole.MarkupLine("[green]Lane added successfully.[/]");
        Log($"Lane with number {lane.Number} added.");
        return true;
    }

    public bool Delete(int id)
    {
        if(!_rbacService.CheckPermission(Resource, Permission.Delete)) return false;
        var lane = _laneRepository.Get(id);
        if (lane == null)
        {
            AnsiConsole.MarkupLine("[red]Lane not found.[/]");
            Log($"Tried to delete non-existent lane with {id} id.");
            return false;
        }
        bool success = _laneRepository.Delete(id);
        if (success)
        {
            AnsiConsole.MarkupLine("[green]Lane deleted successfully.[/]");
            Log($"Lane with id {id} deleted.");
        }
        else
        {
            AnsiConsole.MarkupLine("[red]Failed to delete lane.[/]");
            Log($"Failed to delete lane with id {id}.");
        }
        return success;
    }

    public bool Update(Lane lane)
    {
        if(!_rbacService.CheckPermission(Resource, Permission.Update)) return false;
        ValidateLane(lane);
        var existingLane = _laneRepository.Get(lane.Id);
        if (existingLane == null)
        {
            AnsiConsole.MarkupLine("[red]Lane not found.[/]");
            Log($"Tried to update non-existent lane with {lane.Id} id.");
            return false;
        }
        _laneRepository.Update(lane);
        AnsiConsole.MarkupLine("[green]Lane updated successfully.[/]");
        Log($"Lane with id {lane.Id} updated.");
        return true;
    }

    public Lane? GetById(int id)
    {
        if (!_rbacService.CheckPermission(Resource, Permission.Read)) return null;
        var lane = _laneRepository.Get(id);
        if (lane == null)
        {
            AnsiConsole.MarkupLine("[red]Lane not found.[/]");
            Log($"Lane with id {id} not found.");
            return null;
        }
        Log($"Lane with id {id} found.");
        return lane;
    }

    public List<Lane>? GetAll()
    {
        if(!_rbacService.CheckPermission(Resource, Permission.Read)) return null;
        var lanes = _laneRepository.GetAll();
        if (lanes == null || lanes.Count == 0)
        {
            AnsiConsole.MarkupLine("[red]No lanes found.[/]");
            Log("No lanes found.");
            return null;
        }
        Log("Lanes found.");
        return lanes;
    }

    public List<Lane>? GetByDate(DateTime date)
    {
        if(!_rbacService.CheckPermission(Resource, Permission.Read)) return null;
        var lanes = GetAvailable();
        if (lanes == null || lanes.Count == 0)
        {
            AnsiConsole.MarkupLine("[red]No lanes found.[/]");
            Log("No lanes found.");
            return null;
        }
        var reservations = _reservationRepository.GetByDate(date);
        if (reservations == null || reservations.Count == 0)
        {
            Log($"No reservations found for {date} date.");
            return lanes;
        }
        
        foreach (var reservation in reservations)
        {
            var lane = lanes.FirstOrDefault(l => l.Id == reservation.LaneId);
            if (lane != null)
            {
                lanes.Remove(lane);
            }
        }
        if (lanes.Count == 0)
        {
            AnsiConsole.MarkupLine("[red]No lanes found.[/]");
            Log("No lanes found.");
            return null;
        }
        Log($"Available lanes for {date} date found.");
        return lanes;
    }

    public List<Lane>? GetByStatus(LaneStatus status)
    {
        if(!_rbacService.CheckPermission(Resource, Permission.Read)) return null;
        var lanes = _laneRepository.GetByStatus(status);
        if (lanes.Count == 0)
        {
            AnsiConsole.MarkupLine("[red]No lanes found.[/]");
            Log($"No lanes found with status {status}.");
            return null;
        }
        Log($"Lanes found with status {status}.");
        return lanes;
    }

    public List<Lane>? GetAvailable()
    {
        return GetByStatus(LaneStatus.Available);
    }

    public bool CloseLane(int id)
    {
        if (!_rbacService.CheckPermission(Resource, Permission.All)) return false;
        var lane = _laneRepository.Get(id);
        if (lane == null)
        {
            AnsiConsole.MarkupLine("[red]Lane not found.[/]");
            Log($"Tried to close non-existent lane with {id} id.");
            return false;
        }
        if (lane.Status == LaneStatus.Closed)
        {
            AnsiConsole.MarkupLine("[red]Lane is already closed.[/]");
            Log($"Tried to close already closed lane with {id} id.");
            return false;
        }
        lane.Status = LaneStatus.Closed;
        _laneRepository.Update(lane);
        
        var reservations = _reservationRepository.GetByLane(id);
        foreach (var reservation in reservations)
        {
            if (reservation.Date > DateTime.Now)
            {
                _reservationRepository.Delete(reservation.Id);
            }
        }
        AnsiConsole.MarkupLine("[green]Lane closed successfully.[/]");
        Log($"Lane with id {id} closed.");
        return true;
    }

    private bool ValidateLane(Lane lane)
    {
        var existingLane = _laneRepository.GetByNumber(lane.Number);
        if (existingLane != null && existingLane.Id != lane.Id)
        {
            AnsiConsole.MarkupLine("[red]Lane with this number already exists.[/]");
            Log("Tried to add lane with existing number.");
            return false;
        }
        if(lane.Number < 0)
        {
            AnsiConsole.MarkupLine("[red]Lane number cannot be negative.[/]");
            Log("Tried to add lane with negative number.");
            return false;
        }

        if (lane.Price < 0)
        {
            AnsiConsole.MarkupLine("[red]Lane price cannot be negative.[/]");
            Log("Tried to add lane with negative price.");
            return false;
        }

        return true;
    }
}