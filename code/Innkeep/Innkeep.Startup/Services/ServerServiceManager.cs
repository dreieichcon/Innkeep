﻿using Innkeep.Api.Auth;
using Innkeep.Api.Fiskaly.Interfaces.Auth;
using Innkeep.Api.Fiskaly.Interfaces.Tss;
using Innkeep.Api.Fiskaly.Repositories.Auth;
using Innkeep.Api.Fiskaly.Repositories.Tss;
using Innkeep.Api.Pretix.Interfaces;
using Innkeep.Api.Pretix.Repositories.Auth;
using Innkeep.Api.Pretix.Repositories.General;
using Innkeep.Api.Pretix.Repositories.Sales;
using Innkeep.Db.Interfaces;
using Innkeep.Db.Server.Context;
using Innkeep.Db.Server.Models;
using Innkeep.Db.Server.Repositories.Config;
using Innkeep.Db.Server.Repositories.Registers;
using Innkeep.Server.Controllers.Endpoints;
using Innkeep.Server.Services.Authentication;
using Innkeep.Server.Services.Database;
using Innkeep.Server.Services.Fiskaly;
using Innkeep.Server.Services.Interfaces;
using Innkeep.Server.Services.Interfaces.Fiskaly;
using Innkeep.Server.Services.Interfaces.Pretix;
using Innkeep.Server.Services.Interfaces.Registers;
using Innkeep.Server.Services.Pretix;
using Innkeep.Server.Services.Registers;
using Innkeep.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Innkeep.Startup.Services;

public static class ServerServiceManager
{
	public static void ConfigureServices(IServiceCollection collection, bool isKestrel = false)
	{
		ConfigureDatabase(collection);
		ConfigureDbRepositories(collection);
		ConfigureDbServices(collection);
		ConfigureHttpRepositories(collection);
		ConfigureHttpServices(collection);
		
		if (isKestrel)
			ConfigureControllers(collection);
	}

	private static void ConfigureControllers(IServiceCollection collection)
	{
		collection.AddControllers().AddApplicationPart(typeof(RegisterController).Assembly);
	}

	private static void ConfigureDatabase(IServiceCollection collection)
	{
		if (!Directory.Exists("./db")) 
			Directory.CreateDirectory("./db");
		
		collection.AddDbContextFactory<InnkeepServerContext>(options => options.UseSqlite("DataSource=./db/server.db"));
		collection.AddDbContext<InnkeepServerContext>();
	}

	private static void ConfigureDbRepositories(IServiceCollection collection)
	{
		collection.AddSingleton<IDbRepository<PretixConfig>, PretixConfigRepository>();
		collection.AddSingleton<IDbRepository<FiskalyConfig>, FiskalyConfigRepository>();
		collection.AddSingleton<IDbRepository<FiskalyTseConfig>, FiskalyTseConfigRepository>();
		collection.AddSingleton<IDbRepository<Register>, RegisterRepository>();
	}
	
	private static void ConfigureDbServices(IServiceCollection collection)
	{
		collection.AddSingleton<IDbService<PretixConfig>, PretixConfigService>();
		collection.AddSingleton<IPretixAuthenticationService, PretixAuthenticationService>();
		
		collection.AddSingleton<IDbService<FiskalyConfig>, FiskalyConfigService>();
		collection.AddSingleton<IFiskalyAuthenticationService, FiskalyAuthenticationService>();
		collection.AddSingleton<IFiskalyTssService, FiskalyTssService>();

		collection.AddSingleton<IRegisterService, RegisterService>();
	}

	private static void ConfigureHttpRepositories(IServiceCollection collection)
	{
		collection.AddSingleton<IPretixAuthRepository, PretixAuthRepository>();
		collection.AddSingleton<IPretixOrganizerRepository, PretixOrganizerRepository>();
		collection.AddSingleton<IPretixEventRepository, PretixEventRepository>();
		collection.AddSingleton<IPretixSalesItemRepository, PretixSalesItemRepository>();

		collection.AddSingleton<IFiskalyAuthRepository, FiskalyAuthRepository>();
		collection.AddSingleton<IFiskalyTssRepository, FiskalyTssRepository>();
	}

	private static void ConfigureHttpServices(IServiceCollection collection)
	{
		collection.AddSingleton<IPretixSalesItemService, PretixSalesItemService>();

		collection.AddSingleton<IFiskalyTssService, FiskalyTssService>();
	}
}