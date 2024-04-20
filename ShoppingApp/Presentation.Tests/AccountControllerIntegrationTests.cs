using Azure.Core;
using Domain.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Moq;
using NUnit;
using NUnit.Framework.Interfaces;
using Persistence;
using Service.Abstactions;
using Services;
using System.Text;
using System.Text.Json;

namespace Presentation.Tests
{
	public class AccountControllerIntegrationTests
	{
		private WebApplicationFactory<ShoppingApp.Program> _factory;
		private HttpClient _client;
		private TestServer _server;

		[OneTimeSetUp]
		public void SetUp()
		{
			_factory = new WebApplicationFactory<ShoppingApp.Program>();
			_client = _factory.CreateClient();
		}

		[OneTimeTearDown]
		public void TearDown()
		{
			_client?.Dispose();
			_factory?.Dispose();
		}

		[Test]
		public async Task Register_Test_Works()
		{
			//Arrange
			_factory.WithWebHostBuilder(builder =>
			{
				builder.ConfigureTestServices(services =>
				{
					services.RemoveAll(typeof(DbContextOptions<ShoppingAppDbContext>));
					services.AddDbContext<ShoppingAppDbContext>(options =>
					{
						options.UseInMemoryDatabase("test1");
					});
				});
			});
			using (var scope = _factory.Services.CreateScope())
			{
				var scopeService = scope.ServiceProvider;
				var dbContext = scopeService.GetRequiredService<ShoppingAppDbContext>();

				dbContext.Database.EnsureCreated();
			}

				var model = new RegisterViewModel
				{
					Email = "fdsfsd@gmail.com",
					Password = "@Password123",
					UserName = "username23",
				};

			//Act
			var response = await _client.PostAsync("/Account/Register", new StringContent(
				JsonSerializer.Serialize(model), Encoding.UTF8, "application/json"));

			//Assert
			response.EnsureSuccessStatusCode();
		}

		[Test]
		public async Task AddToCart_GuestCart_Works()
		{
			//Arrange
			_factory.WithWebHostBuilder(builder =>
			{
				builder.ConfigureTestServices(services =>
				{
					services.RemoveAll(typeof(DbContextOptions<ShoppingAppDbContext>));
					services.AddDbContext<ShoppingAppDbContext>(options =>
					{
						options.UseInMemoryDatabase("test2");
					});
				});
			});
			using (var scope = _factory.Services.CreateScope())
			{
				var scopeService = scope.ServiceProvider;
				var dbContext = scopeService.GetRequiredService<ShoppingAppDbContext>();
				dbContext.Database.EnsureDeleted();
				dbContext.Database.EnsureCreated();

				dbContext.Products.Add(new Product
				{
					Id = "1",
					QuantityInStock = 2,
					Description = "test",
					Name = "Test",
					Category = Domain.Enums.Category.Mouses,
				});
				dbContext.SaveChanges();
			}

			var args = new { productId = "1",  quantity = 1 };
			var requestContent = new FormUrlEncodedContent(new[]
				{
					new KeyValuePair<string, string>("productId", args.productId),
					new KeyValuePair<string, string>("quantity", args.quantity.ToString())
				}
			);

			//Act
			var response = await _client.PostAsync("/Cart/AddToCart/", requestContent);

			//Assert
			response.EnsureSuccessStatusCode();
		}
	}
}