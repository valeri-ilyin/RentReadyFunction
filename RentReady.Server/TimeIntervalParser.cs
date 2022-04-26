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
    public class RequestValidationException : ApplicationException
	{
        public RequestValidationException()
        {
        }

        public RequestValidationException(string message)
            : base(message)
        {
        }

        public RequestValidationException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }

    /// <summary>
    /// Парсинг и валидация JSON с запросом к функции TimeEntryFunction
    /// Валидация Json производится по предоставленной в исходном документ схеме.
    /// Добавлено ограничение на максимальный интервал, который готовы обработать - 1000 дней. Ограничение введено, 
    /// чтобы избежать получения на обработку "бесконечных" интервалов. 
    /// При необходимости максимальный может быть увеличен.
    /// </summary>
    public class TimeIntervalParser
	{
        public static readonly int MaxIntervalLengthInDays = 1000;
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

            try
            {
                request.Validate(schema);
            }
            catch (JSchemaValidationException ex)
			{
                throw new RequestValidationException(ex.Message, ex);   
			}

            var result = JsonConvert.DeserializeObject<TimeInterval>(requestBody, dateTimeConverter);
            if (result == null)
			{
                throw new RequestValidationException("TimeInterval deserialization error");
			}

            if (result.StartOn > result.EndOn)
			{
                throw new RequestValidationException("StartOn must be before than EndOn");
            }

            if ((result.EndOn - result.StartOn).TotalDays > MaxIntervalLengthInDays)
            {
                throw new RequestValidationException("Max interval length is 1000 days");
            }

            return result;  
        }


    }
}
