﻿using Innkeep.Db.Models;

namespace Innkeep.Db.Server.Models;

public class Register : AbstractDbItem
{
	public string RegisterIdentifier { get; set; }
	
	public string RegisterDescription { get; set; }
}