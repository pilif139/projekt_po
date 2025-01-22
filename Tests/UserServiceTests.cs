using projekt_po.Utils;
using Moq;
using projekt_po.Model;
using projekt_po.Repository;
using projekt_po.Services;

namespace Tests;

public class UserServiceTests : IDisposable
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<ILogger> _loggerMock;
    private readonly UserService _userService;

    public UserServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _loggerMock = new Mock<ILogger>();
        _userService = new UserService(_userRepositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public void AddUser_ShouldAddUser()
    {
        //Arrange
        string name = "John";
        string surname = "Doe";
        string password = "password";
        Role role = Role.User;

        // Act
        _userService.AddUser(name, surname, password, role);

        //Assert
        _userRepositoryMock.Verify(repo => repo.Add(name, surname, It.IsAny<string>(), role), Times.Once);
    }

    [Fact]
    public void DeleteUser_ShouldDeleteUser()
    {
        //Arrange
        int id = 1;

        // Act
        _userService.DeleteUser(id);

        //Assert
        _userRepositoryMock.Verify(repo => repo.Delete(id), Times.Once);
    }

    [Fact]
    public void GetUserById_ShouldReturnUser()
    {
        //Arrange
        int id = 1;
        var expectedUser = new User("John", "Doe", "password", Role.User){Id = id};
        _userRepositoryMock.Setup(repo => repo.GetById(id)).Returns(expectedUser);
        
        
        
        // Act
        var result = _userService.GetUserById(id);
        
        //Assert
        Assert.NotNull(result);
        Assert.Equal(expectedUser, result);
        _userRepositoryMock.Verify(repo => repo.GetById(id), Times.Once);
    }
    
    [Fact]
    public void GetUserById_ShouldReturnNull()
    {
        //Arrange
        int id = 1;
        _userRepositoryMock.Setup(repo => repo.GetById(id)).Returns((User?)null);
        
        // Act
        var result = _userService.GetUserById(id);
        
        //Assert
        Assert.Null(result);
        _userRepositoryMock.Verify(repo => repo.GetById(id), Times.Once);
    }
    
    // runs after every test
    public void Dispose()
    {
        // checks if Log method was called after each test
        _loggerMock.Verify(logger => logger.Log(It.IsAny<string>()), Times.Once);
    }
}