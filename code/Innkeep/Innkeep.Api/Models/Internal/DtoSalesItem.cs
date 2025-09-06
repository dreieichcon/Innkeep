using System.Text.Json.Serialization;
using Innkeep.Api.Models.Pretix.Objects.Sales;

namespace Innkeep.Api.Models.Internal;

public class DtoSalesItem
{
	public int Id { get; set; }
	
	public int Position { get; set; }
	
	#region Pricing
	public required string Name { get; set; }
	
	public required decimal Price { get; set; }
	
	public required decimal TaxRate { get; set; }
	
	public required string Currency { get; set; }

	[JsonIgnore]
	private decimal TaxRateCalculation => TaxRate / 100;
   
	[JsonIgnore]
	public decimal NetPrice => Price - TaxAmount;

	[JsonIgnore]
	public decimal TotalNetPrice => NetPrice * CartCount;
 
 	[JsonIgnore]
	public decimal TaxAmount => Math.Round(Price / (1 + TaxRateCalculation) * TaxRateCalculation, 2);

	[JsonIgnore]
	public decimal GrossPrice => Price;

	[JsonIgnore]
	public decimal TotalPrice => GrossPrice * CartCount;

	[JsonIgnore]
	public decimal TotalTax => TaxAmount * CartCount;
	
	#endregion
	
	#region Quota
	public int CartCount { get; set; }
	
	public int? QuotaMax { get; set; }
	
	public int? QuotaLeft { get; set; }

	[JsonIgnore]
	public int? QuotaSold => QuotaMax - QuotaLeft;
	
	public bool SoldOut { get; set; }

	[JsonIgnore]
	public bool Infinite => QuotaLeft == null;
	
	#endregion
	
	public bool PrintCheckinVoucher { get; set; }

	public static DtoSalesItem FromPretix(PretixSalesItem pretixSalesItem)
	{
		return new DtoSalesItem()
		{
			Id = pretixSalesItem.Id,
			Name = pretixSalesItem.Name.German,
			Price = pretixSalesItem.DefaultPrice,
			TaxRate = pretixSalesItem.TaxRate,
			Currency = pretixSalesItem.Currency,
			Position = pretixSalesItem.Position,
			PrintCheckinVoucher = pretixSalesItem.InternalName?.StartsWith("print") ?? false,
		};
	}
}