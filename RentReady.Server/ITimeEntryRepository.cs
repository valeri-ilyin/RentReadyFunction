using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentReady.Server
{
	public interface ITimeEntryRepository
	{
		IAsyncEnumerable<TimeEntry> GetTimeEntryListAsync(TimeInterval interval);
		Task<Guid> CreateTimeEntryAsync(TimeEntry item);
	}
}
