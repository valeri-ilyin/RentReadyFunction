using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentReady.Client
{
	/// <summary>
	/// Клиентская версия Интервала (начальная и конечная даты) по которому создаются и запрашиваются записи TimeEntry
	/// </summary>
	public class TimeInterval
	{
		public DateTime StartOn { get; set; }
		public DateTime EndOn { get; set; }
	}
}
