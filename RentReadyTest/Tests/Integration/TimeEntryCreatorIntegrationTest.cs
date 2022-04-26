using Microsoft.VisualStudio.TestTools.UnitTesting;
using RentReady.Server;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;

namespace RentReady.Test.Integration
{
	[TestClass]
	public class TimeEntryCreatorIntegrationTest
	{
		[TestMethod]
		public async Task CreateForInterval()
		{
			using var repo = new TimeEntryRepository(ConfigurationHelper.GetPowerAppConnection());
			var creator = new TimeEntryCreator(repo);
			await repo.DeleteAll();

			var start = DateTimeHelper.CreateDateTime(2022, 4, 10);
			var end = DateTimeHelper.CreateDateTime(2022, 4, 14);
						
			var count = await creator.CreateForIntervalAsync(new TimeInterval() { StartOn = start, EndOn = end });
			Assert.AreEqual(5, count, "Создано 5 элементов");
			var itemsAfter = await repo.GetAllTimeEntryAsync().ToListAsync();
			Assert.AreEqual(5, itemsAfter.Count, "5 элемент в базе");

			var start2 = DateTimeHelper.CreateDateTime(2022, 4, 12);
			var end2 = DateTimeHelper.CreateDateTime(2022, 4, 15);
			var count2 = await creator.CreateForIntervalAsync(new TimeInterval() { StartOn = start2, EndOn = end2 });
			Assert.AreEqual(1, count2, "Создано 1 элемент");

			var itemsAfter2 = await repo.GetAllTimeEntryAsync().ToListAsync();
			Assert.AreEqual(6, itemsAfter2.Count, "6 элемент в базе");
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public async Task CreateForIntervalMaxInterval()
		{
			using var repo = new TimeEntryRepository(ConfigurationHelper.GetPowerAppConnection());
			var creator = new TimeEntryCreator(repo);

			var start = DateTimeHelper.CreateDateTime(2022, 4, 01);
			var end = start.AddDays(TimeEntryCreator.MaxIntervalLengthInDays + 1);

			await creator.CreateForIntervalAsync(new TimeInterval() { StartOn = start, EndOn = end });
		}
	}
}