﻿using Innkeep.Db.Client.Context;
using Innkeep.Db.Client.Models;
using Innkeep.Db.Client.Repositories.Core;
using Innkeep.Db.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Innkeep.Db.Client.Repositories.Config;

public class ClientConfigRepository(IDbContextFactory<InnkeepClientContext> contextFactory)
	: BaseInnkeepClientRepository<ClientConfig>(contextFactory), IDbRepository<ClientConfig>
{
	
}