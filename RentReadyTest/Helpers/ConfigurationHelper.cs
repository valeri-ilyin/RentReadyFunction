using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentReady.Test
{
	public static class ConfigurationHelper
	{
		public static string GetPowerAppConnection()
		{
			return new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("AppSettings")["PowerAppConnection"];
		}
	}
}
