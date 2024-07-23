using Domain.Repositories;
using Domain.Wrappers;
using Moq;
using System.Net.Mail;
using Shared.Models;

namespace Services.Tests
{
	internal class EmailSenderServiceTests
	{
		private Mock<IRepositoryManager> _repositoryManagerMock;
		private Mock<ISessionWrapper> _sessionMock;
		private Mock<ISmtpClientWrapper> _clientMock;
		private EmailSenderService _emailSenderService;

		[SetUp]
		public void Setup()
		{
			_repositoryManagerMock = new Mock<IRepositoryManager>();
			_sessionMock = new Mock<ISessionWrapper>();
			_clientMock = new Mock<ISmtpClientWrapper>();

			_emailSenderService = new EmailSenderService(_sessionMock.Object, _repositoryManagerMock.Object, _clientMock.Object);
		}

		[Test]
		public async Task SendEmailAsync_PassedValidEmail_CallsSendEmailAsync()
		{
			//Arrange
			var email = "testmail@gmail.com";
			var subject = "testsubject";
			var body = "test body\n body";

			//Act
			await _emailSenderService.SendEmailAsync(email, subject, body);

			//Assert
			_clientMock.Verify(x => x.SendMailAsync(It.IsAny<SmtpClient>(), It.IsAny<MailMessage>()), Times.Once);
		}

		[Test]
		public async Task SendConfirmationKey_PassedValidEmail_SendsKey()
		{
			//Arrange
			var email = "testmail@gmail.com";
			var subject = "testsubject";

			//Act
			var result = await _emailSenderService.SendConfirmationKey(email, subject);

			//Assert
			_clientMock.Verify(x => x.SendMailAsync(It.IsAny<SmtpClient>(), It.IsAny<MailMessage>()), Times.Once);
			Assert.IsNotNull(result);
			Assert.IsNotEmpty(result);
		}

		[Test]
		public async Task SendRegisterConfirmationEmail_Test_Works()
		{
			//Arrange
			var model = new RegisterViewModel
			{
				Email = "testmail@gmail.com",
			};

			//Act
			var result = await _emailSenderService.SendRegisterConfirmationEmail(model);

			//Assert
			_clientMock.Verify(x => x.SendMailAsync(It.IsAny<SmtpClient>(), It.IsAny<MailMessage>()), Times.Once);
			Assert.IsNotNull(result);
			Assert.IsNotEmpty(result.Key);
			Assert.That(result.Model.Email, Is.EqualTo(model.Email));
		}

		[Test]
		public async Task SendNameChangeConfirmationEmail_Test_Works()
		{
			//Arrange
			var model = new ChangeUserNameViewModel
			{
				Email = "testmail@gmail.com",
			};

			//Act
			var result = await _emailSenderService.SendNameChangeConfirmationEmail(model);

			//Assert
			_clientMock.Verify(x => x.SendMailAsync(It.IsAny<SmtpClient>(), It.IsAny<MailMessage>()), Times.Once);
			Assert.IsNotNull(result);
			Assert.IsNotEmpty(result.Key);
			Assert.That(result.Model.Email, Is.EqualTo(model.Email));
		}

		[Test]
		public async Task SendEmailRemovalConfirmationEmail_Test_Works()
		{
			//Arrange
			var model = new ChangeEmailViewModel
			{
				Email = "testmail@gmail.com",
			};

			//Act
			var result = await _emailSenderService.SendEmailRemovalConfirmationEmail(model);

			//Assert
			_clientMock.Verify(x => x.SendMailAsync(It.IsAny<SmtpClient>(), It.IsAny<MailMessage>()), Times.Once);
			Assert.IsNotNull(result);
			Assert.IsNotEmpty(result.Key);
			Assert.That(result.Model.Email, Is.EqualTo(model.Email));
		}

		[Test]
		public async Task SendEmailChangeConfirmationEmail_Test_Works()
		{
			//Arrange
			var model = new ChangeEmailViewModel
			{
				NewEmail = "testmail@gmail.com",
			};

			//Act
			var result = await _emailSenderService.SendEmailChangeConfirmationEmail(model);

			//Assert
			_clientMock.Verify(x => x.SendMailAsync(It.IsAny<SmtpClient>(), It.IsAny<MailMessage>()), Times.Once);
			Assert.IsNotNull(result);
			Assert.IsNotEmpty(result.Key);
			Assert.That(result.Model.NewEmail, Is.EqualTo(model.NewEmail));
		}
	}
}
