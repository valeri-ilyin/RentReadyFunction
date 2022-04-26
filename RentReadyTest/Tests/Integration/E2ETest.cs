using Microsoft.VisualStudio.TestTools.UnitTesting;
using RentReady.Server;
using RentReady.Client;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace RentReady.Test.Integration
{


	/// <summary>
	// Для запуска этих  E2E тестов необходима функция, запущенная локально и доступная по functionUrl 
	// Для этого можно запустить функцию в отдельной Visual Studio, а тесты запускать здесь.
	// Эти тест так же можно использовать для проверки функции размещенной на хостинге. Для этого достаточно поменять functionUrl
	// Внимание: в этих тестах происходит полная очистка таблицы данных при каждом запуске  
	/// </summary>
	[TestClass]
	public class E2ETest
	{
		// Для локальной проверки
		public static readonly string functionUrl = "http://localhost:7071/api/TimeEntryFunction";
		// Для проверки на хостинге (рабочий url)
		//public static readonly string functionUrl = "https://rentready.azurewebsites.net/api/TimeEntryFunction";

		[TestMethod]
		public async Task AddInterval()
		{
			using var repo = new TimeEntryRepository(ConfigurationHelper.GetPowerAppConnection());
			await repo.DeleteAll();

			var client = new TimeEntryClient(functionUrl);
			var item = new RentReady.Client.TimeInterval()
			{
				StartOn = new DateTime(2022, 04, 14),
				EndOn = new DateTime(2022, 04, 14)
			};

			int count = await client.AddInterval(item);
			Assert.AreEqual(1, count);
		}

		/// <summary>
		/// Проверка скорости заливки 100 элементов
		/// </summary>
		/// <returns></returns>
		[TestMethod]
		public async Task AddInterval100Elements()
		{
			using var repo = new TimeEntryRepository(ConfigurationHelper.GetPowerAppConnection());
			await repo.DeleteAll();

			var client = new TimeEntryClient(functionUrl);
			var start = new DateTime(2022, 04, 01);
			var end = start.AddDays(99);
			var item = new RentReady.Client.TimeInterval()
			{
				StartOn = start,
				EndOn = end
			};

			int count = await client.AddInterval(item);
			Assert.AreEqual(100, count);
		}

		/// <summary>
		/// Проверка exception, 
		/// </summary>
		/// <returns></returns>
		[TestMethod]
		[ExpectedException(typeof(HttpRequestException))]
		public async Task AddInterval2000Elements()
		{
			using var repo = new TimeEntryRepository(ConfigurationHelper.GetPowerAppConnection());
			await repo.DeleteAll();

			var client = new TimeEntryClient(functionUrl);
			var start = new DateTime(2022, 04, 01);
			var end = start.AddDays(2000);
			var item = new RentReady.Client.TimeInterval()
			{
				StartOn = start,
				EndOn = end
			};

			try
			{
				int count = await client.AddInterval(item);
			}
			catch (HttpRequestException ex)
			{
				Assert.AreEqual(400, ex.StatusCode != null ?(int)ex.StatusCode : 0);
				throw;
			}
		}

		/// <summary>
		/// Начальная дата больше чем конечная
		/// </summary>
		/// <returns></returns>
		[TestMethod]
		[ExpectedException(typeof(HttpRequestException))]
		public async Task AddIntervalIncorrectInterval()
		{
			using var repo = new TimeEntryRepository(ConfigurationHelper.GetPowerAppConnection());
			await repo.DeleteAll();

			var client = new TimeEntryClient(functionUrl);
			var end = new DateTime(2022, 04, 01);
			var start = end.AddDays(2);  // Начальная дата больше чем конечная
			var item = new RentReady.Client.TimeInterval()
			{
				StartOn = start,
				EndOn = end
			};


			try
			{
				int count = await client.AddInterval(item);
			}
			catch (HttpRequestException ex)
			{
				Assert.AreEqual(400, ex.StatusCode != null ? (int)ex.StatusCode : 0);
				throw;
			}
		}
	}
}