﻿using Innkeep.Api.Models.Internal;
using Innkeep.Api.Models.Pretix.Objects.Sales;
using Innkeep.Api.Pretix.Interfaces;
using Innkeep.Server.Db.Models;
using Innkeep.Server.Services.Interfaces;
using Innkeep.Services.Interfaces;

namespace Innkeep.Server.Services.Pretix;

public class PretixSalesItemService : IPretixSalesItemService
{
	private readonly IDbService<PretixConfig> _pretixConfigService;
	private readonly IPretixSalesItemRepository _salesItemRepository;

	public PretixSalesItemService(IDbService<PretixConfig> pretixConfigService, IPretixSalesItemRepository salesItemRepository)
	{
		_pretixConfigService = pretixConfigService;
		_salesItemRepository = salesItemRepository;

		_pretixConfigService.ItemsUpdated += async (_, _) => await Load();
	}
	
	public event EventHandler? SalesItemsUpdated;

	public IEnumerable<PretixSalesItem> SalesItems { get; set; } = new List<PretixSalesItem>();

	public IEnumerable<DtoSalesItem> DtoSalesItems { get; set; } = new List<DtoSalesItem>();

	private string? OrganizerSlug => _pretixConfigService.CurrentItem.SelectedOrganizerSlug;

	private string? EventSlug => _pretixConfigService.CurrentItem.SelectedEventSlug;

	public async Task Load()
	{
		if (OrganizerSlug is null || EventSlug is null) return;
		
		SalesItems = await _salesItemRepository.GetItems(OrganizerSlug, EventSlug);
		
		DtoSalesItems = SalesItems.Select(DtoSalesItem.FromPretix);
	}
}