using Microsoft.VisualStudio.TestTools.UnitTesting;
using RentReady.Server;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;
using Moq;
using System.Collections.Generic;

namespace RentReady.Test.Unit
{
	[TestClass]
	public class TimeEntryCreatorTest
	{

		[TestMethod]
		public async Task CreateForIntervalEmptyDb()
		{
			var repoMock = new Mock<ITimeEntryRepository>();
			var resultList = new List<TimeEntry>();
			repoMock.Setup(m => m.CreateTimeEntryAsync(It.IsAny<TimeEntry>()).Result).Returns(Guid.NewGuid());
			repoMock.Setup(m => m.GetTimeEntryListAsync(It.IsAny<TimeInterval>())).Returns(resultList.ToAsyncEnumerable());

			var creator = new TimeEntryCreator(repoMock.Object);
			var start = DateTimeHelper.CreateDateTime(2022, 4, 10);
			var end = DateTimeHelper.CreateDateTime(2022, 4, 11);
			var count = await creator.CreateForIntervalAsync(new TimeInterval() { StartOn = start, EndOn = end });
			Assert.AreEqual(2, count);
			repoMock.Verify(s => s.CreateTimeEntryAsync(It.IsAny<TimeEntry>()), Times.Exactly(2));

		}

		public async Task CreateForIntervalNotEmptyDb()
		{
			var repoMock = new Mock<ITimeEntryRepository>();
			var resultList = new List<TimeEntry>();
			resultList.Add(new TimeEntry()
			{
				Start = DateTimeHelper.CreateDateTime(2022, 4, 10),
				End = DateTimeHelper.CreateDateTime(2022, 4, 10)
			});

			repoMock.Setup(m => m.CreateTimeEntryAsync(It.IsAny<TimeEntry>()).Result).Returns(Guid.NewGuid());
			repoMock.Setup(m => m.GetTimeEntryListAsync(It.IsAny<TimeInterval>())).Returns(resultList.ToAsyncEnumerable());

			var creator = new TimeEntryCreator(repoMock.Object);
			var start = DateTimeHelper.CreateDateTime(2022, 4, 10);
			var end = DateTimeHelper.CreateDateTime(2022, 4, 11);
			var count = await creator.CreateForIntervalAsync(new TimeInterval() { StartOn = start, EndOn = end });
			Assert.AreEqual(1, count);
			repoMock.Verify(s => s.CreateTimeEntryAsync(It.IsAny<TimeEntry>()), Times.Exactly(1));
		}
	}
}