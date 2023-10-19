﻿using Innkeep.Data.Pretix.Models;

namespace Innkeep.Server.Interfaces.Services;

public interface IPretixService
{
    public IEnumerable<PretixOrganizer> Organizers { get; set; }
    
    public IEnumerable<PretixEvent> Events { get; set; }
    
    public PretixOrganizer? SelectedOrganizer { get; set; }
    
    public PretixEvent? SelectedEvent { get; set; }

    public IEnumerable<PretixSalesItem> SalesItems { get; set; }

    public event EventHandler ItemUpdated;

    public event EventHandler Initialized;
}