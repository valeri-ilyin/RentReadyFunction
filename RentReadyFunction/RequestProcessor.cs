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
	public class RequestProcessor
	{
		private ILogger log;

		public RequestProcessor(ILogger log)
		{
			this.log = log;
		}

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
			catch (Exception ex)
			{
				log.LogError((EventId)0, ex, "Server error");
				throw;
			}
		}
	}
}
