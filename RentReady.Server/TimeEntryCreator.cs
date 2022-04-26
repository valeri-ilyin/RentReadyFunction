using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace RentReady.Server
{
	public class TimeEntryCreator
	{
		public static readonly int BatchSize = 50; // PowerApp doesn't accept more then 52 parallel requests 
		public static readonly int MaxIntervalLengthInDays = 1000;  

		private ITimeEntryRepository repo;

		public TimeEntryCreator(ITimeEntryRepository repo)
		{
			this.repo = repo;
		}

		public async Task<int> CreateForIntervalAsync(TimeInterval interval)
		{
			if ((interval.EndOn - interval.StartOn).TotalDays > MaxIntervalLengthInDays)
			{
				throw new ArgumentException("Max interval length is 1000 days"); 
			}

			var createdItems = await repo.GetTimeEntryListAsync(interval).ToDictionaryAsync(i => i.Start);

			int processedCount = 0;	
			var currentDate = interval.StartOn;
			while (currentDate <= interval.EndOn)
			{
				var tasks = new List<Task>();
				while (currentDate <= interval.EndOn && tasks.Count < BatchSize)
				{

					if (!createdItems.ContainsKey(currentDate))
					{
						var item = new TimeEntry() { Start = currentDate, End = currentDate };
						tasks.Add(repo.CreateTimeEntryAsync(item));
					}
					currentDate = currentDate.AddDays(1);
				}
				await Task.WhenAll(tasks);
				processedCount += tasks.Count;
			}

			return processedCount;
		}
	}
}
