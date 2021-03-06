using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace RentReady.Server
{
	/// <summary>
	/// Репозиторий для таблицы TimeEntry. Реализует все необходимые CRUD операции для таблицы TimeEntry в PowerApp
	/// </summary>
	public sealed class TimeEntryRepository : ITimeEntryRepository, IDisposable
	{
		public static readonly string StartField = "msdyn_start";
		public static readonly string EndField = "msdyn_end";
		public static readonly string EntityName = "msdyn_timeentry";

		public ServiceClient client { get; set; }

		public TimeEntryRepository(string connection)
		{
			client = new ServiceClient(connection);
			if (client == null || !client.IsReady)
			{
				throw new ApplicationException("PowerApp client creation exception");
			}
		}

		/// <summary>
		////Получить TimeEntry запись по id
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public async Task<TimeEntry> GetTimeEntryAsync(Guid id)
		{
			var columns = new ColumnSet();
			columns.AddColumns(StartField, EndField);
			return EntityToTimeEntry(await client.RetrieveAsync(EntityName, id, columns));
		}

		/// <summary>
		/// Получить список TimeEntry записей по переденному фильтру
		/// Граничные значения фильтра сравниваются через >= и <=, те они попадают в результирующую выборку 
		/// </summary>
		/// <param name="interval"></param>
		/// <returns></returns>
		public async IAsyncEnumerable<TimeEntry> GetTimeEntryListAsync(TimeInterval interval)
		{
			ConditionExpression condition1 = new ConditionExpression();
			condition1.AttributeName = StartField;
			condition1.Operator = ConditionOperator.GreaterEqual;
			condition1.Values.Add(interval.StartOn);

			ConditionExpression condition2 = new ConditionExpression();
			condition2.AttributeName = StartField;
			condition2.Operator = ConditionOperator.LessEqual;
			condition2.Values.Add(interval.EndOn);

			FilterExpression filter1 = new FilterExpression();
			filter1.Conditions.Add(condition1);
			filter1.Conditions.Add(condition2);
			filter1.FilterOperator = LogicalOperator.And;

			QueryExpression query = new QueryExpression(EntityName);
			query.ColumnSet.AddColumns(StartField, EndField);
			query.Criteria.AddFilter(filter1);

			var items = await client.RetrieveMultipleAsync(query);


			foreach (var item in EntityListToTimeEntry(items.Entities))
			{
				yield return item;	
			}
		}

		/// <summary>
		/// Получение ВСЕХ записей из таблицы TimeEntry. Используется только в тестах 
		/// </summary>
		/// <returns></returns>
		public async IAsyncEnumerable<TimeEntry> GetAllTimeEntryAsync()
		{
			QueryExpression query = new QueryExpression(EntityName);
			query.ColumnSet.AddColumns(StartField, EndField);
			var items = await client.RetrieveMultipleAsync(query);

			foreach (var item in EntityListToTimeEntry(items.Entities))
			{
				yield return item;
			}
		}

		/// <summary>
		/// Создать запись TimeEntry
		/// </summary>
		/// <param name="entry"></param>
		/// <returns></returns>
		/// <exception cref="ApplicationException"></exception>
		public async Task<Guid> CreateTimeEntryAsync(TimeEntry entry)
		{
			var item = new Entity(EntityName);
			item[StartField] = entry.Start;
			item[EndField] = entry.End;
			var id = await client.CreateAsync(item);

			if (id == Guid.Empty)
			{
				throw new ApplicationException($"TimeEntry creation error: {client.LastError} {client.LastException.Message}");
			}

			return id;
		}

		/// <summary>
		/// Удаление ВСЕХ записей в таблице TimeEntry. Используется только для тестов
		/// </summary>
		/// <returns></returns>
		public async Task DeleteAll()
		{
			await foreach (var item in GetAllTimeEntryAsync())
			{
				await client.DeleteAsync(EntityName, item.Id);
			}
		}

		public void Dispose()
		{
			if (client != null)
			{
				client.Dispose();
				GC.SuppressFinalize(this);
			}
		}

		~TimeEntryRepository() => Dispose();

		/// <summary>
		/// Конвертация списка Entity в список TimeEntry
		/// </summary>
		/// <param name="entities"></param>
		/// <returns></returns>
		private IEnumerable<TimeEntry> EntityListToTimeEntry(DataCollection<Entity> entities)
		{
			foreach (var item in entities)
			{
				yield return EntityToTimeEntry(item);
			}
		}

		/// <summary>
		/// Конвертация Entity в TimeEntry
		/// </summary>
		/// <param name="entity"></param>
		/// <returns></returns>
		private TimeEntry EntityToTimeEntry(Entity entity)
		{
			var start = entity.Attributes.ContainsKey(StartField) ? (DateTime)entity.Attributes[StartField] : DateTime.MinValue;
			var end = entity.Attributes.ContainsKey(EndField) ? (DateTime)entity.Attributes[EndField] : DateTime.MinValue;
			return new TimeEntry()
			{
				Id = entity.Id,
				Start = start,
				End = end
			};
		}
	}
}
