using Innkeep.Api.Models.Internal;
using Innkeep.Api.Models.Pretix.Objects.Sales;
using Innkeep.Api.Pretix.Interfaces.Quota;
using Innkeep.Api.Pretix.Interfaces.Sales;
using Innkeep.Services.Server.Interfaces.Internal;
using Innkeep.Services.Server.Interfaces.Pretix;
using Serilog;

namespace Innkeep.Services.Server.Pretix;

public class PretixSalesItemService(
	IPretixSalesItemRepository salesItemRepository,
	IPretixQuotaRepository quotaRepository,
	IEventStateService eventStateService
) : IPretixSalesItemService
{
	private const int QuotaRefreshInterval = 2;
	private readonly PeriodicTimer _reloadTimer = new(TimeSpan.FromMinutes(QuotaRefreshInterval));

	public event EventHandler? SalesItemsUpdated;

	public IEnumerable<PretixSalesItem> SalesItems { get; set; } = new List<PretixSalesItem>();

	public IEnumerable<DtoSalesItem> DtoSalesItems { get; set; } = new List<DtoSalesItem>();

	public DateTime LastQuotaUpdate { get; set; } = DateTime.Now;

	public DateTime LastFullUpdate { get; set; } = DateTime.Now;

	public async Task ReloadTimer()
	{
		while (await _reloadTimer.WaitForNextTickAsync())
		{
			await LoadQuotas();
			Log.Debug("Reloaded Quotas");
		}
	}

	public async Task Load()
	{
		if (!eventStateService.IsEventConfigured) return;

		var itemResponse = await salesItemRepository.GetItems(
			eventStateService.PretixOrganizerSlug,
			eventStateService.PretixEventSlug
		);

		if (!itemResponse.IsSuccess)
			return;

		SalesItems =
			itemResponse.Object!.Where(item => item.AllSalesChannels || item.SalesChannels.Contains("pretixpos"));

		DtoSalesItems = SalesItems.Select(DtoSalesItem.FromPretix);

		eventStateService.EventCurrency = DtoSalesItems.FirstOrDefault()?.Currency ?? "EUR";

		LastFullUpdate = DateTime.Now;

		await LoadQuotas();
	}

	public async Task LoadQuotas()
	{
		var quotas = await quotaRepository.GetAll(
			eventStateService.PretixOrganizerSlug,
			eventStateService.PretixEventSlug
		);

		var toUpdate = DtoSalesItems.ToList();

		foreach (var item in toUpdate)
		{
			var itemQuotas = quotas.Object!.Where(x => x.Items.Contains(item.Id)).MaxBy(x => x.Size);

			if (itemQuotas is null)
			{
				item.QuotaMax = null;
				item.QuotaLeft = null;
				item.SoldOut = false;

				continue;
			}

			item.QuotaMax = itemQuotas.Size;
			item.QuotaLeft = itemQuotas.AmountAvailable;
			item.SoldOut = itemQuotas.Closed;
		}

		DtoSalesItems = toUpdate;

		LastQuotaUpdate = DateTime.Now;

		SalesItemsUpdated?.Invoke(this, EventArgs.Empty);
	}
}