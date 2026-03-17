using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;

public class UserServiceTests
{
    [Fact]
    public async Task RegisterAsync_ValidInput_ShouldReturnSuccess()
    {
        // Arrange
        var mockUserManager = CreateMockUserManager();
        var mockSignInManager = CreateMockSignInManager(mockUserManager.Object);

        mockUserManager
            .Setup(x => x.CreateAsync(It.IsAny<IdentityUser>(), "Test123!"))
            .ReturnsAsync(IdentityResult.Success);

        var service = new UserService(mockUserManager.Object, mockSignInManager.Object);

        // Act
        var result = await service.RegisterAsync("test@example.com", "Test123!");

        // Assert
        Assert.True(result.Succeeded);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public async Task RegisterAsync_InvalidPassword_ShouldReturnErrors()
    {
        // Arrange
        var mockUserManager = CreateMockUserManager();
        var mockSignInManager = CreateMockSignInManager(mockUserManager.Object);

        var errors = new List<IdentityError>
        {
            new IdentityError { Description = "Password too short" }
        };

        mockUserManager
            .Setup(x => x.CreateAsync(It.IsAny<IdentityUser>(), "123"))
            .ReturnsAsync(IdentityResult.Failed(errors.ToArray()));

        var service = new UserService(mockUserManager.Object, mockSignInManager.Object);

        // Act
        var result = await service.RegisterAsync("test@example.com", "123");

        // Assert
        Assert.False(result.Succeeded);
        Assert.Contains("Password too short", result.Errors);
    }

    [Fact]
    public async Task LoginAsync_ValidCredentials_ShouldReturnTrue()
    {
        // Arrange
        var mockUserManager = CreateMockUserManager();
        var mockSignInManager = CreateMockSignInManager(mockUserManager.Object);

        mockSignInManager
            .Setup(x => x.PasswordSignInAsync("test@example.com", "password", false, false))
            .ReturnsAsync(SignInResult.Success);

        var service = new UserService(mockUserManager.Object, mockSignInManager.Object);

        // Act
        var result = await service.LoginAsync("test@example.com", "password", false);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task LoginAsync_InvalidCredentials_ShouldReturnFalse()
    {
        // Arrange
        var mockUserManager = CreateMockUserManager();
        var mockSignInManager = CreateMockSignInManager(mockUserManager.Object);

        mockSignInManager
            .Setup(x => x.PasswordSignInAsync("test@example.com", "wrong", false, false))
            .ReturnsAsync(SignInResult.Failed);

        var service = new UserService(mockUserManager.Object, mockSignInManager.Object);

        // Act
        var result = await service.LoginAsync("test@example.com", "wrong", false);

        // Assert
        Assert.False(result);
    }

    // === Helper Methods ===

    private static Mock<UserManager<IdentityUser>> CreateMockUserManager()
    {
        var store = new Mock<IUserStore<IdentityUser>>();
        return new Mock<UserManager<IdentityUser>>(
            store.Object, null, null, null, null, null, null, null, null
        );
    }

    private static Mock<SignInManager<IdentityUser>> CreateMockSignInManager(UserManager<IdentityUser> userManager)
    {
        return new Mock<SignInManager<IdentityUser>>(
            userManager,
            new Mock<IHttpContextAccessor>().Object,
            new Mock<IUserClaimsPrincipalFactory<IdentityUser>>().Object,
            null, null, null, null
        );
    }
}
