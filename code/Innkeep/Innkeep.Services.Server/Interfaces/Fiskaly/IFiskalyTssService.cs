﻿using Innkeep.Api.Models.Fiskaly.Objects;
using Innkeep.Api.Models.Fiskaly.Objects.Tss;

namespace Innkeep.Services.Server.Interfaces.Fiskaly;

public interface IFiskalyTssService
{
	public event EventHandler? ItemsUpdated;
	
	public IEnumerable<FiskalyTss> TssObjects { get; set; }
	
	public FiskalyTss? CurrentTss { get; set; }

	public Task Load();

	public Task<bool> Save();

	public Task<bool> CreateNew();

	public Task<bool> Deploy();

	public Task<bool> ChangeAdminPin();

	public Task<bool> InitializeTss();
	
}