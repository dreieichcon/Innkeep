﻿using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Innkeep.Api.Fiskaly.Data;
using Innkeep.Api.Fiskaly.Interfaces;
using Innkeep.Api.Fiskaly.Models;
using Innkeep.Core.Core;
using Innkeep.Endpoints.Fiskaly;
using Innkeep.Models.Transaction;

namespace Innkeep.Api.Fiskaly.Repositories;

public class FiskalyTransactionRepository : BaseHttpRepository, IFiskalyTransactionRepository
{
	private readonly IFiskalyApiSettingsService _settingsService;

	public FiskalyTransactionRepository(IFiskalyApiSettingsService settingsService)
	{
		_settingsService = settingsService;
	}
	
	public async Task<TransactionResponseModel?> StartTransaction(PretixTransaction pretixTransaction)
	{
		var endpoint = new FiskalyTransactionEndpointBuilder()
						.WithTss(_settingsService.ApiSettings.TseId)
						.WithTransaction(pretixTransaction.TransactionId.ToString())
						.WithRevision("1").Build();

		var requestModel = TransactionStartSerializer.CreateTransactionStart(pretixTransaction);
		
		var json = JsonSerializer.Serialize(requestModel);
		var jsonContent = new StringContent(json, Encoding.UTF8, "application/json");

		var response = await ExecutePutRequest(endpoint, jsonContent);

		var deserialized = JsonSerializer.Deserialize<TransactionResponseModel>(response);

		if (deserialized is not null) return deserialized;
		Serilog.Log.Debug("Received null response for TransactionStart for {TransactionId}", pretixTransaction.TransactionId);
		return null;
	}

	public async Task<TransactionResponseModel?> UpdateTransaction(TransactionUpdateRequestModel requestModel, string transactionId)
	{
		var endpoint = new FiskalyTransactionEndpointBuilder()
						.WithTss(_settingsService.ApiSettings.TseId)
						.WithTransaction(transactionId)
						.WithRevision("2").Build();
		
		var json = JsonSerializer.Serialize(requestModel);
		var jsonContent = new StringContent(json, Encoding.UTF8, "application/json");

		var response = await ExecutePutRequest(endpoint, jsonContent);

		var deserialized = JsonSerializer.Deserialize<TransactionResponseModel>(response);

		if (deserialized is not null) return deserialized;
		Serilog.Log.Debug("Received null response for TransactionUpdate for {TransactionId}", transactionId);
		return null;
	}

	public async Task<TransactionResponseModel?> EndTransaction(TransactionUpdateRequestModel requestModel, string transactionId)
	{
		var endpoint = new FiskalyTransactionEndpointBuilder()
						.WithTss(_settingsService.ApiSettings.TseId)
						.WithTransaction(transactionId)
						.WithRevision("2").Build();
		
		var json = JsonSerializer.Serialize(requestModel);
		var jsonContent = new StringContent(json, Encoding.UTF8, "application/json");

		var response = await ExecutePutRequest(endpoint, jsonContent);

		var deserialized = JsonSerializer.Deserialize<TransactionResponseModel>(response);

		if (deserialized is not null) return deserialized;
		Serilog.Log.Debug("Received null response for TransactionEnd for {TransactionId}", transactionId);
		return null;
	}
	
	protected override void PrepareGetHeaders(HttpRequestMessage message)
	{
		throw new NotImplementedException();
	}

	protected override void PreparePostHeaders()
	{
		throw new NotImplementedException();
	}

	protected override void PreparePutHeaders()
	{
		Client.DefaultRequestHeaders.Authorization =
			new AuthenticationHeaderValue("Bearer", _settingsService.GetToken());
	}
}