﻿using System.Globalization;
using System.Text.Json;
using Innkeep.Core.DomainModels.Print;
using Innkeep.Shared.Objects.Transaction;
using Innkeep.Server.Data.Interfaces;
using Innkeep.Server.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace Innkeep.Server.Api.Pretix;

public class PretixRequestController
{
	private readonly IRegisterService _registerService;
	private readonly IPretixService _pretixService;

	public PretixRequestController(IRegisterService registerService, IPretixService pretixService)
	{
		Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
		_registerService = registerService;
		_pretixService = pretixService;
	}

	[Route("Pretix/Organizer/{registerId}")]
	public IActionResult GetOrganizer([FromRoute] string registerId)
	{
		Log.Debug("Received Organizer Request from Register: {RegisterId}", registerId);
		if (_registerService.CurrentRegistersContains(registerId))
		{
			Log.Debug("Register {RegisterId} found in trusted clients, sending organizer", registerId);
			return new OkObjectResult(JsonSerializer.Serialize(_pretixService.SelectedOrganizer));
		}

		Log.Debug("Register {RegisterId} not trusted", registerId);
		return new UnauthorizedResult();
	}
	
	[Route("Pretix/Event/{registerId}")]
	public IActionResult GetEvent([FromRoute] string registerId)
	{
		Log.Debug("Received Event Request from Register: {RegisterId}", registerId);
		if (_registerService.CurrentRegistersContains(registerId))
		{
			Log.Debug("Register {RegisterId} found in trusted clients, sending event", registerId);
			return new OkObjectResult(JsonSerializer.Serialize(_pretixService.SelectedEvent));
		}

		Log.Debug("Register {RegisterId} not trusted", registerId);
		return new UnauthorizedResult();
	}
	
	[Route("Pretix/SalesItems/{registerId}")]
	public IActionResult GetSalesItems([FromRoute] string registerId)
	{
		Log.Debug("Received Sales Items Request from Register: {RegisterId}", registerId);
		if (_registerService.CurrentRegistersContains(registerId))
		{
			Log.Debug("Register {RegisterId} found in trusted clients, sending sales items", registerId);
			return new OkObjectResult(JsonSerializer.Serialize(_pretixService.SalesItems));
		}

		Log.Debug("Register {RegisterId} not trusted", registerId);
		return new UnauthorizedResult();
	}

	[HttpPost]
	[Route("Pretix/Transaction/{registerId}")]
	public async Task<IActionResult> ReceiveTransaction([FromRoute] string registerId, [FromBody]Shared.Objects.Transaction.Transaction transaction)
	{
		Log.Debug("Received Sales Items Request from Register: {RegisterId}", registerId);

		if (_registerService.CurrentRegistersContains(registerId))
		{
			Log.Debug("Register {RegisterId} found in trusted clients, accepting transaction", registerId);

			var receipt = await _pretixService.CreateOrder(transaction);
			return new JsonResult(receipt);

			// TODO - Create Transaction on Pretix Side, Send back Receipt if successful
			// TODO - Create Receipt Settings
		}
		
		Log.Debug("Register {RegisterId} not trusted", registerId);
		return new UnauthorizedResult();
	}
}