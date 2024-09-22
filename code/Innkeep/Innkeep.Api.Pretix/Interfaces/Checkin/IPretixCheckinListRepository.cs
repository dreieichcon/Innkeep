﻿using Innkeep.Api.Models.Pretix.Objects.Checkin;
using Lite.Http.Interfaces;

namespace Innkeep.Api.Pretix.Interfaces.Checkin;

public interface IPretixCheckinListRepository
{
	public Task<IHttpResponse<IEnumerable<PretixCheckinList>>> GetAll(string organizerSlug, string eventSlug);
}