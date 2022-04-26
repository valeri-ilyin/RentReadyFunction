using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentReady.Server
{
	/// <summary>
	/// Доменный объект для записи в таблице TimeEntry
	/// </summary>
	public class TimeEntry
	{
		public Guid Id { get; set; }
		public DateTime Start { get; set; }
		public DateTime End { get; set; }
	}
}
