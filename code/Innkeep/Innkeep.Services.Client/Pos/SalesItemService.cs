using Innkeep.Api.Internal.Interfaces.Server.Pos;
using Innkeep.Api.Models.Internal;
using Innkeep.Services.Client.Interfaces.Internal;
using Innkeep.Services.Client.Interfaces.Pos;
using Serilog;

namespace Innkeep.Services.Client.Pos;

public class SalesItemService : ISalesItemService
{
	private readonly PeriodicTimer _reloadTimer = new(TimeSpan.FromMinutes(1));
	private readonly IEventRouter _router;
	private readonly ISalesItemRepository _salesItemRepository;

	public SalesItemService(ISalesItemRepository salesItemRepository, IEventRouter router)
	{
		_salesItemRepository = salesItemRepository;
		_router = router;

		_router.OnRegisterConnected += async (_, _) => await Load();
	}

	public DateTime? LastUpdated { get; set; }

	public event EventHandler? ItemsUpdated;

	public IEnumerable<DtoSalesItem> SalesItems { get; set; } = new List<DtoSalesItem>();

	public async Task Load()
	{
		try
		{
			var items = await _salesItemRepository.GetSalesItems();

			if (items.Object is null || !items.Object.Any())
				return;
            
			SalesItems = items.Object!.OrderBy(x => x.Position);
			_router.SalesItemsReloaded();
			LastUpdated = DateTime.Now;
			ItemsUpdated?.Invoke(this, EventArgs.Empty);
		}
		catch (Exception ex)
		{
			Log.Error(ex, "Error loading sales items");
		}
	}

	public async Task ReloadTask()
	{
		while (await _reloadTimer.WaitForNextTickAsync())
		{
			await Load();
			Log.Debug("Reloaded Sales Items");
		}
	}
}