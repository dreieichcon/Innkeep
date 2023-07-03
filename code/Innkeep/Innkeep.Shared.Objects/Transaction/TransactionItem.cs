﻿using Innkeep.Core.Interfaces.Transaction;
using Innkeep.Data.Pretix.Models;

namespace Innkeep.Shared.Objects.Transaction;

public class TransactionItem : ITransactionItem
{
	public decimal Price { get; set; }

	public decimal TaxPrice { get; set; }

	public decimal TaxRate { get; set; }

	public int Count { get; set; }

	public string Currency { get; set; }

	public TransactionItem(PretixCartItem<PretixSalesItem> item)
	{
		Price = item.Price;
		TaxPrice = item.TaxPrice;
		TaxRate = item.Item.TaxRate;
		Count = item.Count;
		Currency = item.Item.Currency;
	}
}