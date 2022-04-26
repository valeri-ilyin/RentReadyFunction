using Newtonsoft.Json;
using System.Text;

namespace RentReady.Client
{
	public class TimeEntryClient
	{
		private static readonly HttpClient client = new HttpClient();

		private JsonSerializerSettings jsonSettings = new JsonSerializerSettings();

		public string Url { get; private set; }

		public TimeEntryClient(string url)
		{
			Url = url;
			jsonSettings.DateFormatString = "yyyy-MM-dd";
		}

		public async Task<int> AddInterval(TimeInterval interval)
		{
			var content = new StringContent(JsonConvert.SerializeObject(interval, jsonSettings), Encoding.UTF8, "application/json");
			var response = await client.PostAsync(Url, content);
			response.EnsureSuccessStatusCode();
			var result = await response.Content.ReadAsStringAsync();
			return JsonConvert.DeserializeObject<int>(result);
		}

	}
}