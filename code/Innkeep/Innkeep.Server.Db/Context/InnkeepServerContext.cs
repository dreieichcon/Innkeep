﻿using Innkeep.Server.Db.Models;
using Microsoft.EntityFrameworkCore;

namespace Innkeep.Server.Db.Context;

public class InnkeepServerContext(DbContextOptions<InnkeepServerContext> options) : DbContext(options)
{
	public DbSet<PretixConfig> PretixConfigs { get; init; } = null!;

	public DbSet<FiskalyConfig> FiskalyConfigs { get; init; } = null!;

	public DbSet<FiskalyTseConfig> TseConfigs { get; init; } = null!;

	public DbSet<Register> Registers { get; init; } = null!;
}