﻿using Innkeep.Api.Models.Internal;
using Innkeep.Api.Pretix.Interfaces;
using Innkeep.Api.Pretix.Repositories.Order;
using Innkeep.Api.Pretix.Repositories.Sales;
using Innkeep.Api.Pretix.Tests.Data;
using Innkeep.Api.Pretix.Tests.Mock;

namespace Innkeep.Api.Pretix.Tests.Repositories;

[TestClass]
public class PretixOrderRepositoryTests
{
	private readonly ITestAuth _testAuth = new TestAuth();
	private PretixSalesItemRepository _itemRepository = null!;

	private IPretixOrderRepository _orderRepository = null!;

	[TestInitialize]
	public void Initialize()
	{
		var authenticationService = new PretixAuthenticationServiceMock();
		_itemRepository = new PretixSalesItemRepository(authenticationService);
		_orderRepository = new PretixOrderRepository(authenticationService);
	}

	[TestMethod]
	public async Task CreateOrder_ReturnsSuccessfully()
	{
		var items = await _itemRepository.GetItems(_testAuth.PretixTestOrganizerSlug, _testAuth.PretixTestEventSlug);

		Assert.IsTrue(items.Object?.Any());

		var dto = DtoSalesItem.FromPretix(items.Object!.First());

		dto.CartCount = 2;

		var cart = new List<DtoSalesItem>
		{
			dto,
		};

		var result = await _orderRepository.CreateOrder(
			_testAuth.PretixTestOrganizerSlug,
			_testAuth.PretixTestEventSlug,
			cart,
			true
		);

		Assert.IsNotNull(result);
	}
}