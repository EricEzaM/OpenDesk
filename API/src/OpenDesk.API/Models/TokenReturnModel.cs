using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OpenDesk.API.Models
{
	public class TokenReturnModel
	{
		[JsonPropertyName("id_token")]
		public string IdToken { get; set; }
	}
}
