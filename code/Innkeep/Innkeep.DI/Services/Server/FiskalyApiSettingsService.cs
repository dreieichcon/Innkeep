﻿using Innkeep.Server.Data.Interfaces.Fiskaly;
using Innkeep.Server.Data.Models;

namespace Innkeep.DI.Services.Server;

public class FiskalyApiSettingsService : IFiskalyApiSettingsService
{
	private readonly IFiskalyApiSettingsRepository _apiSettingsRepository;

	public FiskalyApiSettingsService(IFiskalyApiSettingsRepository apiSettingsRepository)
	{
		_apiSettingsRepository = apiSettingsRepository;
		Read();
	}
	
	public FiskalyApiSettings ApiSettings { get; set; }

	public bool AuthenticationSuccessful { get; set; }

	public void Read()
	{
		ApiSettings = _apiSettingsRepository.GetOrCreate();
	}

	public void Save()
	{
		throw new NotImplementedException();
	}
}