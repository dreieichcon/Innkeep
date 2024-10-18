﻿using Serilog;
using Serilog.Events;
using Serilog.Sinks.Discord;

namespace Innkeep.Startup.Services;

public static class LoggingManager
{
	public static void InitializeLogger(string serviceName)
	{
		var configuration = new LoggerConfiguration()
							.WriteTo.Trace()
							.WriteTo.Debug()
							.WriteTo.Console()
							.WriteTo.File(
								"./log/log.txt",
								rollingInterval: RollingInterval.Day,
								restrictedToMinimumLevel: LogEventLevel.Information
							)
							.Enrich.FromLogContext()
							.MinimumLevel.Override(
								"Microsoft.EntityFrameworkCore.Database.Command",
								LogEventLevel.Warning
							);

		var webhookUrl = Environment.GetEnvironmentVariable("DISCORD_WEBHOOK_URL");

		if (webhookUrl is not null)
			configuration.WriteTo.Discord(
				LogEventLevel.Warning,
				config =>
				{
					config.WebhookUrl = webhookUrl;
					config.ServiceName = serviceName;
				}
			);

		Log.Logger = configuration.CreateLogger();
	}
}