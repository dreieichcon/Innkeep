﻿using Innkeep.Api.Auth;
using Innkeep.Api.Endpoints;
using Innkeep.Api.Models.Pretix.Objects.General;
using Innkeep.Api.Models.Pretix.Objects.Sales;
using Innkeep.Api.Pretix.Interfaces;
using Innkeep.Api.Pretix.Repositories.Core;

namespace Innkeep.Api.Pretix.Repositories.Sales;

public class PretixSalesItemRepository(IPretixAuthenticationService authenticationService)
	: AbstractPretixRepository<PretixSalesItem>(authenticationService), IPretixSalesItemRepository
{
	public async Task<IEnumerable<PretixSalesItem>> GetItems(PretixOrganizer pOrganizer, PretixEvent pEvent)
	{
		var uri = new PretixEndpointBuilder().WithOrganizer(pOrganizer).WithEvent(pEvent).WithItems().Build();

		return await GetItemsInternal(uri);
	}

	public async Task<IEnumerable<PretixSalesItem>> GetItems(string pOrganizerSlug, string pEventSlug)
	{
		var uri = new PretixEndpointBuilder().WithOrganizer(pOrganizerSlug).WithEvent(pEventSlug).WithItems().Build();

		return await GetItemsInternal(uri);
	}

	private async Task<IEnumerable<PretixSalesItem>> GetItemsInternal(string uri)
	{
		var response = await Get(uri);

		if (!response.IsSuccess)
			return new List<PretixSalesItem>();

		var result = Deserialize(response.Content);

		return result is not null ? result.Results : new List<PretixSalesItem>();
	}
}