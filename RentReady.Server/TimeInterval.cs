using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentReady.Server
{
	/// <summary>
	/// Серверная версия Интервала (начальная и конечная даты) 
	/// </summary>
	public class TimeInterval
	{
		public DateTime StartOn { get; set; }
		public DateTime EndOn { get; set; }
	}
}
