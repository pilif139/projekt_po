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
    private readonly Mock<IRbacService> _rbacMock;
    private readonly UserService _userService;

    public UserServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _loggerMock = new Mock<ILogger>();

        _rbacMock = new Mock<IRbacService>();
        _rbacMock.Setup(rbac => rbac.CheckPermission(It.IsAny<Resource>(), It.IsAny<Permission>())).Returns(true);

        _userService = new UserService(_userRepositoryMock.Object, _rbacMock.Object, _loggerMock.Object);
    }

    [Fact]
    public void AddUser_ShouldAddUser()
    {
        //Arrange
        User user = new User("JohnDoe","John", "Doe", "password", Role.Admin);

        // Act
        _userService.Add(user);

        //Assert
        _userRepositoryMock.Verify(repo => repo.Add(user.Login,user.Name, user.Surname, It.IsAny<string>(), user.Role), Times.Once);
        _rbacMock.Verify(rbac => rbac.CheckPermission(Resource.User, Permission.Create), Times.Once);
    }

    [Fact]
    public void DeleteUser_ShouldDeleteUser()
    {
        //Arrange
        int id = 1;

        // Act
        _userService.Delete(id);

        //Assert
        _userRepositoryMock.Verify(repo => repo.Delete(id), Times.Once);
        _rbacMock.Verify(rbac => rbac.CheckPermission(Resource.User, Permission.Delete), Times.Once);
    }

    [Fact]
    public void GetUserById_ShouldReturnUser()
    {
        //Arrange
        int id = 1;
        var expectedUser = new User("JohnDoe","John", "Doe", "password", Role.Admin) { Id = id };
        _userRepositoryMock.Setup(repo => repo.GetById(id)).Returns(expectedUser);



        // Act
        var result = _userService.GetById(id);

        //Assert
        Assert.NotNull(result);
        Assert.Equal(expectedUser, result);
        _userRepositoryMock.Verify(repo => repo.GetById(id), Times.Once);
        _rbacMock.Verify(rbac => rbac.CheckPermission(Resource.User,Permission.Read), Times.Once);
    }

    [Fact]
    public void GetUserById_ShouldReturnNull()
    {
        //Arrange
        int id = 1;
        _userRepositoryMock.Setup(repo => repo.GetById(id)).Returns((User?)null);

        // Act
        var result = _userService.GetById(id);

        //Assert
        Assert.Null(result);
        _userRepositoryMock.Verify(repo => repo.GetById(id), Times.Once);
        _rbacMock.Verify(rbac => rbac.CheckPermission(Resource.User,Permission.Read), Times.Once);
    }

    // runs after every test
    public void Dispose()
    {
        // checks if Log method was called after each test
        _loggerMock.Verify(logger => logger.Log(It.IsAny<string>()), Times.Once);
    }
}