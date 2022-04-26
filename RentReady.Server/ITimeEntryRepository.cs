using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentReady.Server
{
	/// <summary>
	////Интерфейс репозитория таблицы TimeEntry. Содержит методы, доступные для этой таблицы
	/// </summary>
	public interface ITimeEntryRepository
	{
		/// <summary>
		/// Получить список TimeEntry записей, удовлетворяющий переданному фильтру 
		/// </summary>
		/// <param name="filter"></param>
		/// <returns></returns>
		IAsyncEnumerable<TimeEntry> GetTimeEntryListAsync(TimeInterval filter);

		/// <summary>
		/// Создать TimeEntry запись 
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		Task<Guid> CreateTimeEntryAsync(TimeEntry item);
	}
}
