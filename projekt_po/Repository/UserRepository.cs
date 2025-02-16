using projekt_po.Database;
using projekt_po.Model;

namespace projekt_po.Repository;

// interface for mocking UserRepository in tests
public interface IUserRepository
{
    User Add(string name, string surname, string password, Role role);
    User? GetById(int id);
    List<User> GetAll();
    List<User> GetAllByRole(Role role);
    User? GetByNameAndSurname(string name, string surname);
    bool Delete(int id);
}

// class that uses repository design pattern to seperate database logic from service logic
public class UserRepository : IUserRepository
{
    private readonly DatabaseContext _db;
    
    public UserRepository(DatabaseContext db)
    {
        _db = db;
    }
    
    public User Add(string name, string surname, string password, Role role)
    {
        var user = _db.Users.Add(new User(name, surname, password, role));
        _db.SaveChanges();
        return user.Entity;
    }
    
    public User? GetById(int id)
    {
        return _db.Users.Find(id);
    }
    
    public List<User> GetAll()
    {
        return _db.Users.ToList();
    }

    public List<User> GetAllByRole(Role role)
    {
        return _db.Users.Where(u => u.Role == role).ToList();
    }
    
    public User? GetByNameAndSurname(string name, string surname)
    {
        return _db.Users.FirstOrDefault(u => u.Name == name && u.Surname == surname);
    }
    
    public bool Delete(int id)
    {
        var user = _db.Users.Find(id);
        if (user != null)
        {
            _db.Users.Remove(user);
            _db.SaveChanges();
            return true;
        }

        return false;
    }
}