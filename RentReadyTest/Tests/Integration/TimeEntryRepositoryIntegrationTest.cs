using Microsoft.VisualStudio.TestTools.UnitTesting;
using RentReady.Server;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;

namespace RentReady.Test.Integration
{
	/// <summary>
	/// Интеграционный тесты для TimeEntryRepository. В ходе работы этих тестов записи TimeEntry физически создаются в PowerApp
	/// Внимание: в этих тестах происходит полная очистка таблицы данных при каждом запуске  
	/// </summary>
	[TestClass]
	public class TimeEntryRepositoryIntegrationTest
	{
		[TestMethod]
		public async Task AddTimeEntry()
		{
			using var repo = new TimeEntryRepository(ConfigurationHelper.GetPowerAppConnection());
			var start = new DateTime(2022, 4, 10, 0, 0, 0, DateTimeKind.Utc);
			var end = new DateTime(2022, 4, 10, 0, 0, 0, DateTimeKind.Utc);
			var id = await repo.CreateTimeEntryAsync(new TimeEntry() { Start = start, End = end });
			var item = await repo.GetTimeEntryAsync(id);
			Assert.AreEqual(start, item.Start, "Start создан верно");
			Assert.AreEqual(end, item.End, "End создан верно");
		}

		[TestMethod]
		public async Task GetTimeEntryList()
		{
			using var repo = new TimeEntryRepository(ConfigurationHelper.GetPowerAppConnection());

			await repo.DeleteAll();
			var itemsBefore = await repo.GetAllTimeEntryAsync().ToListAsync();
			Assert.AreEqual(0, itemsBefore.Count, "Пустой список вначале");
			var start = DateTimeHelper.CreateDateTime(2022, 4, 10);
			var end = DateTimeHelper.CreateDateTime(2022, 4, 10); ;
			await repo.CreateTimeEntryAsync(new TimeEntry() { Start = start, End = end });
			var itemsAfter = await repo.GetAllTimeEntryAsync().ToListAsync();
			Assert.AreEqual(1, itemsAfter.Count, "1 элемент добавлен");

			var iterval1 = new TimeInterval() { StartOn = start, EndOn = end };
			var itemsFiltered = await repo.GetTimeEntryListAsync(iterval1).ToListAsync();
			Assert.AreEqual(1, itemsFiltered.Count, "1 элементов фильтре");

			var iterval2 = new TimeInterval() 
			{ 
				StartOn = DateTimeHelper.CreateDateTime(2022, 4, 8), 
				EndOn = DateTimeHelper.CreateDateTime(2022, 4, 9)
			};
			var itemsFiltered2 = await repo.GetTimeEntryListAsync(iterval2).ToListAsync();
			Assert.AreEqual(0, itemsFiltered2.Count, "0 элементов фильтре, который не соответвует данным");
		}
	}
}