﻿using System.Net;
using System.Net.Sockets;
using Innkeep.Api.Pretix.Models.Objects;
using Innkeep.Client.Services.Interfaces.File;
using Innkeep.Client.Services.Interfaces.Server;
using Innkeep.Models.Printer;
using Innkeep.Models.Transaction;
using Serilog;

namespace Innkeep.Client.Services.Services.Server;

public class ClientServerConnectionService : IClientServerConnectionService
{
	private readonly IClientSettingsService _clientSettingsService;
	private readonly IClientServerConnectionRepository _clientServerConnectionRepository;

	private readonly List<string> _serverConnectionOptions = new()
	{
		"http://127.0.0.1:1337",
		"https://127.0.0.1:1338"
	};

	public ClientServerConnectionService(IClientSettingsService clientSettingsService, 
										IClientServerConnectionRepository clientServerConnectionRepository)
	{
		_clientSettingsService = clientSettingsService;
		_clientServerConnectionRepository = clientServerConnectionRepository;
		_serverConnectionOptions.Add(GetCurrentIp());
	}

	public async Task<bool> TestConnection()
	{
		return await _clientServerConnectionRepository.TestConnection(_clientSettingsService.Setting.ServerUri);
	}

	public async Task<bool> RegisterToServer()
	{
		Log.Debug("Test Log.");
		return await _clientServerConnectionRepository.RegisterToServer();
	}

	public bool AutoDiscover(out Uri? uri)
	{
		
		uri = null;
		return false;
	}

	public async Task<Receipt?> SendTransaction(PretixTransaction pretixTransaction)
	{
		return await _clientServerConnectionRepository.SendTransaction(pretixTransaction);
	}

	public async Task<PretixCheckinResponse?> SendCheckIn(PretixCheckin checkin)
	{
		return await _clientServerConnectionRepository.SendCheckIn(checkin);
	}

	private string GetCurrentIp()
	{
		var host = Dns.GetHostEntry(Dns.GetHostName());
		foreach (var ip in host.AddressList)
		{
			if (ip.AddressFamily == AddressFamily.InterNetwork)
			{
				return ip.ToString();
			}
		}

		return string.Empty;
	}
}