﻿using System.Globalization;
using Innkeep.Api.Json;
using Innkeep.Server.Controllers.Abstract;
using Innkeep.Server.Services.Interfaces.Pretix;
using Innkeep.Server.Services.Interfaces.Registers;
using Microsoft.AspNetCore.Mvc;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Innkeep.Server.Controllers.Endpoints;

[Route("items")]
public class SalesItemController : AbstractServerController
{
	private readonly IPretixSalesItemService _salesItemService;

	public SalesItemController(IRegisterService registerService, IPretixSalesItemService salesItemService) : base(registerService)
	{
		Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
		Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
		_salesItemService = salesItemService;
	}

	[HttpGet]
	[Route("get")]
	public IActionResult Get(string identifier)
	{
		if (!IsKnown(identifier)) return new UnauthorizedResult();
		
		var json = JsonSerializer.Serialize(_salesItemService.DtoSalesItems.ToArray(), SerializerOptions.GetServerOptions());

		return new OkObjectResult(json);
	}
}