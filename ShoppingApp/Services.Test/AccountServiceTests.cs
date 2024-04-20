using Domain.Entities;
using Domain.Exceptions;
using Domain.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Moq;
using Shared.Models;
using System.Text.Json;


namespace Services.Tests
{
	internal class AccountServiceTests
	{
		private Mock<IRepositoryManager> _repositoryManagerMock;
		private Mock<UserManager<AppUser>> _userManagerMock;
		private Mock<RoleManager<IdentityRole>> _roleManagerMock;
		private Mock<SignInManager<AppUser>> _signInManagerMock;
		private Mock<IRepository<Cart>> _cartRepositoryMock;
		private AccountService _accountService;

		[SetUp]
		public void SetUp()
		{
			_repositoryManagerMock = new Mock<IRepositoryManager>();
			_userManagerMock = new Mock<UserManager<AppUser>>(Mock.Of<IUserStore<AppUser>>(), null, null, null, null, null, null, null, null);
			_roleManagerMock = new Mock<RoleManager<IdentityRole>>(Mock.Of<IRoleStore<IdentityRole>>(), null, null, null, null);
			_signInManagerMock = new Mock<SignInManager<AppUser>>(
				_userManagerMock.Object, Mock.Of<IHttpContextAccessor>(), Mock.Of<IUserClaimsPrincipalFactory<AppUser>>(), null, null, null);
			_cartRepositoryMock = new Mock<IRepository<Cart>>();
			_repositoryManagerMock.Setup(x => x.CartRepository).Returns(_cartRepositoryMock.Object);

			_accountService = new AccountService(
				_repositoryManagerMock.Object,
				_userManagerMock.Object,
				_roleManagerMock.Object,
				_signInManagerMock.Object
				);
		}

		[Test]
		public async Task SignInAsync_UserNotFound_CreatesUser()
		{
			//Arrange
			var user = new AppUser
			{
				UserName = "test",
				Id = "1",
			};
			_userManagerMock.Setup(x => x.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(user);
			var loginVM = new LoginViewModel
			{
				Password = "password",
				UserName = user.UserName,
			};

			//Act
			await _accountService.SignInAsync(loginVM);

			//Assert
			_signInManagerMock.Verify(x => x.SignInAsync(user, false, loginVM.UserName), Times.Once);
		}

		[Test]
		public async Task SignInAsync_UserFound_ThrowsException()
		{
			//Arrange
			var loginVM = new LoginViewModel
			{
				Password = "password",
				UserName = "name",
			};
			_userManagerMock.Setup(m => m.FindByNameAsync(loginVM.UserName)).ReturnsAsync((AppUser)null);

			//Act && Assert
			Assert.ThrowsAsync<UserNotFoundException>(async () => await _accountService.SignInAsync(loginVM));
		}

		[Test]
		public async Task RegisterAsync_UserFound_ThrowsException()
		{
			//Arrange
			var user1 = new AppUser
			{
				UserName = "test1",
			};
			var user2 = new AppUser
			{
				Email = "testmail"
			};

			var registerVM1 = new RegisterViewModel
			{
				UserName = "test1",
				Email = "s",
			};
			var registerVM2 = new RegisterViewModel
			{
				UserName = "t",
				Email = "testmail"
			};

			_userManagerMock.Setup(x => x.FindByNameAsync(registerVM1.UserName)).ReturnsAsync(user1);
			_userManagerMock.Setup(x => x.FindByEmailAsync(registerVM2.Email)).ReturnsAsync(user2);

			//Act && Assert
			Assert.ThrowsAsync<UsernameOrEmailTakenException>(async () => await _accountService.RegisterAsync(registerVM2));
		}

		[Test]
		public async Task RegisterAsync_UserNotFound_RegistersUser()
		{
			//Arrange
			var registerVM = new RegisterViewModel
			{
				UserName = "test1",
				Email = "s",
			};
			_userManagerMock.Setup(x => x.FindByNameAsync(It.IsAny<string>())).ReturnsAsync((AppUser)null);
			_userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((AppUser)null);
			_userManagerMock.Setup(x => x.CreateAsync(It.IsAny<AppUser>(),It.IsAny<string>()))
							.ReturnsAsync(IdentityResult.Success);
			
			//Act
			await _accountService.RegisterAsync(registerVM);

			//Assert
			_signInManagerMock.Verify(x => x.SignInAsync(It.IsAny<AppUser>(), false, It.IsAny<string>()), Times.Once);
			_userManagerMock.Verify(x => x.AddToRoleAsync(It.IsAny<AppUser>(), "Guest"), Times.Once);
		}

		[Test]
		public async Task RegisterAsync_UserNotFoundCreateFailed_ThrowsException()
		{
			//Arrange
			var registerVM = new RegisterViewModel
			{
				UserName = "test1",
				Email = "s",
			};
			_userManagerMock.Setup(x => x.FindByNameAsync(It.IsAny<string>())).ReturnsAsync((AppUser)null);
			_userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((AppUser)null);
			_userManagerMock.Setup(x => x.CreateAsync(It.IsAny<AppUser>(), It.IsAny<string>()))
							.ReturnsAsync(IdentityResult.Failed());

			//Act && Assert
			Assert.ThrowsAsync<RegisterFailedException>(async () => await _accountService.RegisterAsync(registerVM));
		}

		[Test]
		public async Task GetUserSettingsViewModel_UserFound_ReturnsModel()
		{
			//Arrange
			var expected = new UserSettingsViewModel
			{
				UserName = "testname",
				Email = "testmail",
				Id = "12",
				ActionName = "asd"
			};
			_userManagerMock.Setup(x => x.FindByIdAsync("12")).ReturnsAsync(new AppUser
			{
				UserName = "testname",
				Email = "testmail",
				Id = "12",
			});

			//Act
			var result = await _accountService.GetUserSettingsViewModel("12", "asd");
			var resultJson = JsonSerializer.Serialize(result);
			var expectedJson = JsonSerializer.Serialize(expected);

			//Assert
			Assert.That(resultJson, Is.EqualTo(expectedJson));
		}

		[Test]
		public async Task UpdateUsername_UserFound_UpdatesUsername()
		{
			//Arrange
			var user = new AppUser
			{
				UserName = "3",
			};
			var model = new ChangeUserNameViewModel
			{
				Email = "2",
				Id = "1",
				NewUserName = "4",
			};
			_userManagerMock.Setup(x => x.FindByIdAsync("1")).ReturnsAsync(user);

			//Act
			await _accountService.UpdateUsername(model);

			//Assert
			_userManagerMock.Verify(x => x.UpdateAsync(It.IsAny<AppUser>()), Times.Once);
			Assert.That(user.UserName, Is.EqualTo(model.NewUserName));
		}

		[Test]
		public async Task UpdatePassword_UserFound_CallsChangePasswordAsync()
		{
			//Arrange
			var user = new AppUser();
			var model = new ChangePasswordViewModel();
			_userManagerMock.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(user);

			//Act
			await _accountService.UpdatePassword(model);

			//Assert
			_userManagerMock.Verify(x => x.ChangePasswordAsync(user, It.IsAny<string>(), It.IsAny<string>()),  Times.Once);
		}

		[Test]
		public async Task UpdateEmail_UserFound_Works()
		{
			//Arrange
			var user = new AppUser
			{
				Email = "test1",
			};
			var model = new EmailConfirmationViewModel<ChangeEmailViewModel>
			{
				Model = new ChangeEmailViewModel { 
					ConfirmNewEmail = "",
					Email = "",
					Id = "1",
					NewEmail = "test2",
				}
			};
			_userManagerMock.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(user);

			//Act
			await _accountService.UpdateEmail(model);

			//Assert
			_userManagerMock.Verify(x => x.ChangeEmailAsync(user, It.IsAny<string>(), It.IsAny<string>()), Times.Once);
		}
	}
}
