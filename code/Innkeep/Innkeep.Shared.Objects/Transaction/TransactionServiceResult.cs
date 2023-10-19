﻿using Innkeep.Core.DomainModels.KassSichV;
using Innkeep.Data.Pretix.Models;

namespace Innkeep.Shared.Objects.Transaction;

public class TransactionServiceResult
{
	public Transaction Transaction { get; init; } = null!;
	
	public PretixOrderResponse OrderResponse { get; init; } = null!;
	
	public TseResult TseResult { get; init; } = null!;
	
	public Guid Guid { get; init; }
	
	public string RegisterId { get; init; } = null!;

	public string? OrganizerInfo { get; init; }

	public string EventName { get; init; } = null!;
}