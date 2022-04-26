using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using RentReady.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentReady.Server
{
    /// <summary>
    /// Парсинг и валидация JSON с запросом к функции TimeEntryFunction
    /// Валидация Json производится по предоставленной в исходном документ схеме.
    /// </summary>
	public class TimeIntervalParser
	{
        private IsoDateTimeConverter dateTimeConverter;

        public TimeIntervalParser()
		{
            dateTimeConverter = new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd" };
        }

        string schemaJson = @"{
  '$schema': 'http://json-schema.org/draft-04/schema#',
  'type': 'object',
  'properties': {
    'StartOn': {
      'type': 'string',
      'format': 'date'
    },
    'EndOn': {
      'type': 'string',
      'format': 'date'
    }
  },
    'required': [
        'StartOn',
        'EndOn'
    ]
}
";

        public TimeInterval Parse(string requestBody)
		{
            var schema = JSchema.Parse(schemaJson);
            JObject request = JObject.Parse(requestBody);
            request.Validate(schema);

            var result = JsonConvert.DeserializeObject<TimeInterval>(requestBody, dateTimeConverter);
            if (result == null)
			{
                throw new ArgumentException("TimeInterval deserialization error");
			}

            if (result.StartOn > result.EndOn)
			{
                throw new ArgumentException("StartOn must be before than EndOn");
            }

            return result;  
        }


    }
}
