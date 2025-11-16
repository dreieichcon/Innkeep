using Innkeep.Api.Enum.Fiskaly.Transaction;

namespace Innkeep.Api.Models.Internal.Transfer;

public class ClientTransfer
{
	public decimal Amount { get; set; }

	public int Factor => IsRetrieve || IsCancellation ? -1 : 1;
	
	public bool IsRetrieve { get; set; }
	
	public bool IsCancellation { get; set; }
}