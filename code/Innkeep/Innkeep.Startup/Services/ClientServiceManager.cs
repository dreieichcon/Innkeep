﻿using Innkeep.Client.Controllers.Endpoints;
using Innkeep.Client.Services.Database;
using Innkeep.Db.Client.Context;
using Innkeep.Db.Client.Models;
using Innkeep.Db.Client.Repositories.Config;
using Innkeep.Db.Interfaces;
using Innkeep.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Innkeep.Startup.Services;

public static class ClientServiceManager
{
	public static void ConfigureServices(IServiceCollection collection, bool isKestrel = false)
	{
		ConfigureDatabase(collection);
		ConfigureDbRepositories(collection);
		ConfigureDbServices(collection);
		
		if (isKestrel)
			ConfigureControllers(collection);
	}

	private static void ConfigureControllers(IServiceCollection collection)
	{
		collection.AddControllers().AddApplicationPart(typeof(ServerDataController).Assembly);
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
}