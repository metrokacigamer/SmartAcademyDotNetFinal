# Startup Instructions

1. Add `appsettings.json` in `ShoppingApp/ShoppingApp/`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "ConString": "Server=SERVER-SERVER;Database=DBNAME;Trusted_Connection=True; Encrypt=False"
  }
}
````
In "ConString" field enter names for your server and database:
````
	"ConString": "Server=SERVER-SERVER;Database=DBNAME;
````
# Add a folder named images in ShoppingApp/ShoppingApp/wwwroot/.

In `Seed.cs`, remember to fill in the email with the actual email. Your administrator user account details are:

- Username: `metrokacigamer`
- Password: `@Adminpass1`

In `ShoppingApp/Core/Services/`, add the item `EmailSenderService.cs` that implements `IEmailSenderService`.

```csharp

internal class EmailSenderService : IEmailSenderService
	{
		private readonly IRepositoryManager _repositoryManager;
		private readonly ISessionWrapper _session;
		private readonly ISmtpClientWrapper _smtpClient;

		public EmailSenderService(ISessionWrapper session, IRepositoryManager repositoryManager, ISmtpClientWrapper smtpClient)
		{
			_session = session;
			_repositoryManager = repositoryManager;
			_smtpClient = smtpClient;
		}

		public async Task SendEmailAsync(string email, string subject, string body)
		{
			var mail = "use your email for email sending service";
			var pw = "app password of your email";
			var client = new SmtpClient("smtp.Gmail.com")
			{
				Port = 587,
				EnableSsl = true,
				Credentials = new NetworkCredential(mail, pw),
			};

			await _smtpClient.SendMailAsync(client, new MailMessage(
								from: mail,
								to: email,
								subject,
								body
								));
		}

		public async Task<string> SendConfirmationKey(string email, string subject)
		{
			var key = Guid.NewGuid().ToString();
			var message = $"Confirmation key: {key}";

			await SendEmailAsync(email, subject, message);

			return key;
		}

		public async Task<EmailConfirmationViewModel<RegisterViewModel>> SendRegisterConfirmationEmail(RegisterViewModel model)
		{
			var key = await SendConfirmationKey(model.Email, "Register confirmation");

			return new EmailConfirmationViewModel<RegisterViewModel> { Key = key, Model = model };
		}

		public async Task<EmailConfirmationViewModel<ChangeUserNameViewModel>> SendNameChangeConfirmationEmail(ChangeUserNameViewModel model)
		{
			var key = await SendConfirmationKey(model.Email, "Name Change confirmation");

			return new EmailConfirmationViewModel<ChangeUserNameViewModel> { Key = key, Model = model };
		}

		public async Task<EmailConfirmationViewModel<ChangeEmailViewModel>> SendEmailRemovalConfirmationEmail(ChangeEmailViewModel model)
		{
			var key = await SendConfirmationKey(model.Email, "Email Change confirmation");

			return new EmailConfirmationViewModel<ChangeEmailViewModel> { Key = key, Model = model };
		}

		public async Task<EmailConfirmationViewModel<ChangeEmailViewModel>> SendEmailChangeConfirmationEmail(ChangeEmailViewModel model)
		{
			var key = await SendConfirmationKey(model.NewEmail, "Email Change confirmation");

			return new EmailConfirmationViewModel<ChangeEmailViewModel> { Key = key, Model = model };
		}
	}
```
Don't forget to add your email for email sending service. For Gmail, you'll need to set up two-factor authentication and generate an app password. Use this password in the SendEmailAsync method.

```csharp
// SendEmailAsync method
public async Task SendEmailAsync(string email, string subject, string body)
{
    var mail = "your-email@example.com";
    var pw = "app-password";
    // SmtpClient setup...
}
```

Finally, run the migration, update the database, compile the solution and you are ready to run the web-application.
# Application Overview

The application is a Shopping website project written in ASP.NET Core MVC, utilizing Onion architecture, Repository pattern, unit testing, and integration testing. Here's an overview:

- View and add items to the cart with specified quantity.
- For invalid quantity, redirection to an error page occurs.
- Registered users can save their carts to the database.
- Authentication system allows users to register, log in, and log out.
- Users can explicitly change quantity or remove items from the cart.
- Purchasing items removes them from the cart and adjusts stock quantities.
- Administrator user is seeded in the database on the first run.

## Administrator Features

- Add new products with multiple images.
- Edit product information including adding/removing images.
- Remove products, which also removes associated cart items.

## User Features

- Change username, password, or email (email removal not supported).
- Confirmation keys are sent to email for username and email changes.

## Search, Filter, Sort, and Paging

- Search for products by name or description.
- Filter products by name, description, category, or price.
- Sorting options: by ascending price, category, or name.
- Paging is implemented for search and filtering.