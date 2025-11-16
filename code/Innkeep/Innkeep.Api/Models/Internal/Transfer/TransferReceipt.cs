using System.Globalization;
using System.Text.Json.Serialization;

namespace Innkeep.Api.Models.Internal.Transfer;

public class TransferReceipt
{
	public decimal Amount { get; set; }

	public string Subtitle { get; set; } = "";
	
	public bool IsRetrieve { get; set; }
	
	public bool IsCancellation { get; set; }
	
	public string Currency { get; set; }
	
	public DateTime BookingTime { get; set; }

	public string QrCode { get; set; } = "";

	public long TransactionCounter { get; set; }

	public string? TransactionId { get; set; }

	[JsonIgnore]
	public string AmountString =>
		$"{(IsRetrieve ? "-" : string.Empty)}{Amount.ToString("F2", CultureInfo.InvariantCulture)}";
}