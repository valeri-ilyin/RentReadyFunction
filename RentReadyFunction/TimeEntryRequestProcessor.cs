using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using RentReady.Server;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentReadyFunction
{

	/// <summary>
	/// Класс содежит высокоуровневую логику обработки запроса, пришедшего в функцию
	/// </summary>
	public class TimeEntryRequestProcessor
	{
		private ILogger log;

		public TimeEntryRequestProcessor(ILogger log)
		{
			this.log = log;
		}


		/// <summary>
		/// Главная входная точка. Метод должен запускаться из основого тела функции 
		/// Метод парсит запрос, пришедщий из функции, создает все необходымые экземпляры классов и запускает обработку запроса
		/// </summary>
		/// <param name="requestBody"></param>
		/// <returns></returns>
		public async Task<IActionResult> ProcessAsync(string requestBody)
		{
			try
			{
				var interval = new TimeIntervalParser().Parse(requestBody);
				var connection = Environment.GetEnvironmentVariable("PowerAppConnection");
				using var repo = new TimeEntryRepository(connection);
				var creator = new TimeEntryCreator(repo);
				int count = await creator.CreateForIntervalAsync(interval);

				return new OkObjectResult(count);
			}
			catch (RequestValidationException ex)
			{
				return new BadRequestObjectResult(ex.Message);
			}
			catch (Exception ex)
			{
				log.LogError((EventId)0, ex, "Server error");
				throw;
			}
		}
	}
}
