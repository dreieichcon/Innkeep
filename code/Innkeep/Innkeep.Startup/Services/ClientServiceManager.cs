﻿using Demolite.Db.Interfaces;
using Innkeep.Api.Internal.Interfaces.Server.Checkin;
using Innkeep.Api.Internal.Interfaces.Server.Pos;
using Innkeep.Api.Internal.Interfaces.Server.Register;
using Innkeep.Api.Internal.Repositories.Server.Checkin;
using Innkeep.Api.Internal.Repositories.Server.Pos;
using Innkeep.Api.Internal.Repositories.Server.Register;
using Innkeep.Client.Controllers.Endpoints;
using Innkeep.Db.Client.Context;
using Innkeep.Db.Client.Models;
using Innkeep.Db.Client.Repositories.Config;
using Innkeep.Services.Client.Checkin;
using Innkeep.Services.Client.Database;
using Innkeep.Services.Client.Interfaces.Checkin;
using Innkeep.Services.Client.Interfaces.Internal;
using Innkeep.Services.Client.Interfaces.Pos;
using Innkeep.Services.Client.Interfaces.Registers;
using Innkeep.Services.Client.Internal;
using Innkeep.Services.Client.Pos;
using Innkeep.Services.Client.Registers;
using Innkeep.Services.Hardware;
using Innkeep.Services.Interfaces;
using Innkeep.Services.Interfaces.Hardware;
using Innkeep.Services.Interfaces.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Innkeep.Startup.Services;

public static class ClientServiceManager
{
	public static void ConfigureServices(IServiceCollection collection, bool isKestrel = false)
	{
		ConfigureLocalServices(collection);
		ConfigureDatabase(collection);
		ConfigureDbRepositories(collection);
		ConfigureDbServices(collection);
		ConfigureHttpRepositories(collection);
		ConfigureHttpServices(collection);

		if (isKestrel)
			ConfigureControllers(collection);

		collection.AddSingleton<IStartupService, StartupService>();
	}

	private static void ConfigureLocalServices(IServiceCollection collection)
	{
		collection.AddSingleton<IEventRouter, EventRouter>();
		collection.AddSingleton<IHardwareService, HardwareService>();
	}

	private static void ConfigureControllers(IServiceCollection collection)
	{
		collection
			.AddControllers()
			.AddApplicationPart(typeof(ServerDataController).Assembly);
	}

	private static void ConfigureDatabase(IServiceCollection collection)
	{
		if (!Directory.Exists("./db"))
			Directory.CreateDirectory("./db");

		collection.AddDbContextFactory<InnkeepClientContext>(options => options.UseSqlite("DataSource=./db/client.db"));
		collection.AddDbContext<InnkeepClientContext>();
	}

	private static void ConfigureDbRepositories(IServiceCollection collection)
	{
		collection.AddSingleton<IDbRepository<ClientConfig>, ClientConfigRepository>();
	}

	private static void ConfigureDbServices(IServiceCollection collection)
	{
		collection.AddSingleton<IDbService<ClientConfig>, ClientConfigService>();
	}

	private static void ConfigureHttpRepositories(IServiceCollection collection)
	{
		collection.AddSingleton<IRegisterConnectionRepository, RegisterConnectionRepository>();
		collection.AddSingleton<ISalesItemRepository, SalesItemRepository>();
		collection.AddSingleton<ITransactionRepository, TransactionRepository>();
		collection.AddSingleton<ICheckinRepository, CheckinRepository>();
	}

	private static void ConfigureHttpServices(IServiceCollection collection)
	{
		collection.AddSingleton<IRegisterConnectionService, RegisterConnectionService>();
		collection.AddSingleton<ISalesItemService, SalesItemService>();
		collection.AddSingleton<IClientPosService, ClientPosService>();
		collection.AddSingleton<ICheckinService, CheckinService>();
	}
}