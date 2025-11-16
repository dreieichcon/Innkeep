using DemoPOS.Document;
using ESCPOS;
using Innkeep.Api.Models.Internal;
using Innkeep.Api.Models.Internal.Transaction;
using Innkeep.Api.Models.Internal.Transfer;

namespace Innkeep.Client.Extensions;

public static class ReceiptPrinterExtensions
{
	private const int MaxLength = 42;

	public static DocumentManager AddSeparatedLines(this DocumentManager manager, string input)
	{
		foreach (var line in input.Split(Environment.NewLine))
		{
			manager.AddLine(line, Justification.Center);
		}

		return manager;
	}

	public static DocumentManager AddDashedLine(this DocumentManager manager) => manager.AddLine(new string('-', 42));

	public static DocumentManager AddReceiptLines(this DocumentManager manager, TransactionReceipt receipt)
	{
		if (receipt.Lines.Exists(x => x.HasTax))
			manager.AddLine(SpaceEvenlyAcross("Stk.", "Artikel  ", "MwSt.", receipt.Currency, "SUM"));

		else
			manager.AddLine(SpaceEvenlyAcross("Stk.", "Artikel  ", receipt.Currency, "SUM"));

		manager.AddDashedLine();

		foreach (var line in receipt.Lines)
		{
			manager.AddLine(CreateReceiptLine(line));
		}

		manager.AddEmptyLine();

		return manager;
	}

	public static DocumentManager AddTransferLine(this DocumentManager manager, TransferReceipt receipt)
	{
		manager.AddLine(SpaceEvenlyAcross("Art", receipt.Currency));
		manager.AddDashedLine();

		if (receipt.IsCancellation)
			manager.AddLine(SpaceEvenlyAcross("STORNO", receipt.AmountString));

		else
			manager.AddLine(
				receipt.IsRetrieve
					? SpaceEvenlyAcross("Auszahlung", receipt.AmountString)
					: SpaceEvenlyAcross("Einzahlung", receipt.AmountString)
			);

		return manager;
	}

	public static DocumentManager AddReceiptSum(this DocumentManager manager, TransactionReceipt receipt)
	{
		manager.AddDashedLine();
		manager.AddLine(SpaceEvenlyAcross("Gesamt", receipt.Sum.TotalAmountString));

		manager.AddEmptyLine();

		manager.AddLine(SpaceEvenlyAcross("Gegeben", receipt.Sum.AmountGivenString));
		manager.AddLine(SpaceEvenlyAcross("Zurück", receipt.Sum.AmountReturnedString));
		
		return manager;
	}

	public static DocumentManager AddReceiptTaxIfExists(this DocumentManager manager, TransactionReceipt receipt)
	{
		if (!receipt.Lines.Exists(x => x.HasTax)) return manager;
		
		manager.AddDashedLine();
		manager.AddLine(SpaceEvenlyAcross("Satz", "Netto ", "MwSt  ", "Brutto"));

		foreach (var tax in receipt.TaxInformation)
		{
			manager.AddLine(SpaceEvenlyAcross(tax.NameString, tax.NetString, tax.TaxAmountString, tax.GrossString));
		}

		return manager;
	}

	public static DocumentManager AddTransactionInfo(this DocumentManager manager, TransactionReceipt receipt)
	{
		if (receipt.TransactionCounter > 0)
			manager.AddLine(SpaceEvenlyAcross("Transaktion:", receipt.TransactionCounter.ToString()));
		else
			manager.AddLine(SpaceEvenlyAcross("Transaktion:", "ERR"));
		
		if (!string.IsNullOrEmpty(receipt.TransactionId))
			manager.AddLine(SpaceEvenlyAcross("Id:", receipt.TransactionId));
		
		return manager;
	}
	
	public static DocumentManager AddTransactionInfo(this DocumentManager manager, TransferReceipt receipt)
	{
		manager.AddLine(SpaceEvenlyAcross("Transaktion:", receipt.TransactionCounter.ToString()));
		
		return manager;
	}

	public static DocumentManager AddVouchers(this DocumentManager manager, TransactionReceipt receipt)
	{
		var i = 1;
		
		foreach (var voucher in receipt.ReceiptVouchers)
		{
			manager.AddTitle(receipt.Title);
			manager.AddEmptyLine();
			manager.AddLine(voucher.ItemName);
			manager.AddEmptyLine();
			manager.AddLine(SpaceEvenlyAcross("Id:", receipt.TransactionId));
			manager.AddEmptyLine();
			manager.AddQrCode(voucher.Secret, QRCodeModel.Model2, QRCodeSize.Large);
			manager.AddLine(SpaceEvenlyAcross("", $"{i}/{receipt.ReceiptVouchers.Count}"));
			manager.AddEmptyLine();
			manager.Cut();
			i++;
		}

		return manager;
	}

	private static string SpaceEvenlyAcross(params string[] strings)
	{
		var totalLength = strings.Sum(x => x.Length);

		var spacing = (int) Math.Floor((decimal) (MaxLength - totalLength) / (strings.Length - 1));
		var spacer = new string(' ', spacing);

		return string.Join(spacer, strings);
	}

	private static string CreateReceiptLine(ReceiptLine line)
	{
		// 8 (15) 4 5 5
		return $"{line.CountString}{line.NameString}{line.TaxRateString}{line.PriceString}{line.TotalPrice}";
	}
}