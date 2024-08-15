﻿using System.Text.Json.Serialization;

namespace Innkeep.Api.Models.Pretix.Objects.Order;

public class PretixOrderResponse
{
	[JsonPropertyName("code")]
	public required string Code { get; set; }
	
	[JsonPropertyName("secret")]
	public required string Secret { get; set; }
	
	[JsonIgnore]
	public string ReceiptHeader { get; set; }
	
	[JsonIgnore]
	public string EventTitle { get; set; }

}