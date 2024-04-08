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
# Add a folder named images in ShoppingApp/ShoppingApp/wwwroot/.

In `Seed.cs`, remember to fill in the email with the actual email. Your administrator user account details are:

- Username: `metrokacigamer`
- Password: `@Adminpass1`

In `ShoppingApp/Core/Services/`, add the item `EmailSenderService.cs` that implements `IEmailSenderService`.

```csharp

// EmailSenderService.cs
internal class EmailSenderService : IEmailSenderService
{
    // Constructor and methods go here...
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

- Search for products by name, description, or category.
- Filter products by name, description, category, or price.
- Sorting options: by ascending price, category, or name.
- Paging is implemented for search and filtering.