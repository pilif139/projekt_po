using projekt_po.Utils;
using Moq;
using projekt_po.Model;
using projekt_po.Repository;
using projekt_po.Services;

namespace Tests;

public class UserServiceTests
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
        _loggerMock.Verify(logger => logger.Log(It.IsAny<string>()), Times.Once);
    }
}