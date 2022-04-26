using Microsoft.VisualStudio.TestTools.UnitTesting;
using RentReady.Server;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;
using Moq;
using System.Collections.Generic;
using Newtonsoft.Json.Schema;

namespace RentReady.Test.Unit
{
	[TestClass]
	public class TimeIntervalParserTest
	{

		[TestMethod]
		public void Parse()
		{
			string json = @"{
				'StartOn': '2022-01-01',
				'EndOn': '2022-02-02'
			}";

			var timeInterval = new TimeIntervalParser().Parse(json);
			Assert.AreEqual(DateTimeHelper.CreateDateTime(2022, 1, 1),  timeInterval.StartOn);
			Assert.AreEqual(DateTimeHelper.CreateDateTime(2022, 2, 2), timeInterval.EndOn);	
		}

		[TestMethod]
		[ExpectedException(typeof(JSchemaValidationException))]
		public void ParseEmptyField()
		{
			string json = @"{
				'StartOn': '2022-01-01',
			}";

			var timeInterval = new TimeIntervalParser().Parse(json);
		}

		[TestMethod]
		[ExpectedException(typeof(JSchemaValidationException))]
		public void ParseIncorrectDate()
		{
			string json = @"{
				'StartOn': '2022-50-50',
				'EndOn': '2022-02-02'
			}";

			var timeInterval = new TimeIntervalParser().Parse(json);
		}


		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void ParseStartDateAfterEndDate()
		{
			string json = @"{
				'StartOn': '2022-02-02',
				'EndOn': '2022-02-01'
			}";

			var timeInterval = new TimeIntervalParser().Parse(json);
		}

		[TestMethod]
		[ExpectedException(typeof(JSchemaValidationException))]
		public void ParseIncorrectDateWithTime()
		{
			string json = @"{
				'StartOn': '2022-02-02 0:00:00',
				'EndOn': '2022-02-01'
			}";

			var timeInterval = new TimeIntervalParser().Parse(json);
		}


	}
}