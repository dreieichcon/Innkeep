﻿using Innkeep.Api.Enum.Fiskaly.Transaction;
using Innkeep.Api.Models.Internal;
using Innkeep.Api.Models.Internal.Transaction;
using Innkeep.Api.Server.Interfaces;
using Innkeep.Db.Client.Models;
using Innkeep.Services.Client.Interfaces.Hardware;
using Innkeep.Services.Client.Interfaces.Pos;
using Innkeep.Services.Interfaces;

namespace Innkeep.Services.Client.Pos;

public partial class ClientPosService(
	ITransactionRepository transactionRepository,
	IPrinterService printerService,
	IDbService<ClientConfig> clientConfigService
) : IClientPosService
{
	private decimal _moneyGiven;

	public event EventHandler? MoneyUpdated;

	public event EventHandler? PaymentStateUpdated;

	public bool IsPaymentMode { get; set; }

	public void ClearMoneyGiven()
	{
		MoneyGiven = 0;
		MoneyUpdated?.Invoke(this, EventArgs.Empty);
	}

	public void CancelTransaction()
	{
		IsPaymentMode = false;
		PaymentStateUpdated?.Invoke(this, EventArgs.Empty);
	}

	public async Task<TransactionReceipt?> CommitTransaction()
	{
		var transaction = new ClientTransaction()
		{
			SalesItems = CartItems,
			AmountGiven = MoneyGiven,
			PaymentType = PaymentType.Cash,
			Currency = CartItems[0].Currency,
		};

		LastReceipt = await transactionRepository.CommitTransaction(transaction);

		return LastReceipt;
	}

	public decimal MoneyRequired => CartItems.Sum(x => x.TotalPrice);

	public decimal TaxInRequired => CartItems.Sum(x => x.TotalTax);

	public TransactionReceipt? LastReceipt { get; set; }

	public decimal MoneyGiven
	{
		get => _moneyGiven;
		set
		{
			_moneyGiven = value;
			MoneyUpdated?.Invoke(this, EventArgs.Empty);
		}
	}

	public decimal MoneyBack => MoneyGiven - MoneyRequired;

	public decimal MoneyRemaining => MoneyRequired - MoneyGiven;
}