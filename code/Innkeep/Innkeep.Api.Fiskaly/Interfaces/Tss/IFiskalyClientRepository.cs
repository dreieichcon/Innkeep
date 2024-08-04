﻿using Innkeep.Api.Models.Fiskaly.Objects;

namespace Innkeep.Api.Fiskaly.Interfaces.Tss;

public interface IFiskalyClientRepository
{
	public Task<IEnumerable<FiskalyClient>> GetAll(string tssId);

	public Task<FiskalyClient?> GetOne(string tssId, string id);

	public Task<FiskalyClient?> CreateClient(string tssId, string id, string serialNumber);

	public Task<FiskalyClient?> UpdateClient(string tssId, string id, string state);
}