﻿using Innkeep.Data.Pretix.Models;
using Innkeep.Server.Data.Interfaces;
using Innkeep.Server.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Innkeep.DI.Services;

public class ApplicationSettingsService : IApplicationSettingsService
{
    private readonly IApplicationSettingsRepository _applicationSettingsRepository;
    private readonly IOrganizerRepository _organizerRepository;
    private readonly IEventRepository _eventRepository;

    public ApplicationSettingsService(
        IApplicationSettingsRepository applicationSettingsRepository, 
        IOrganizerRepository organizerRepository, 
        IEventRepository eventRepository)
    {
        _applicationSettingsRepository = applicationSettingsRepository;
        _organizerRepository = organizerRepository;
        _eventRepository = eventRepository;
        Load();
    }

    private void Load()
    {
        ActiveSetting = _applicationSettingsRepository.GetSetting();
    }

    public void UpdateSetting(PretixOrganizer pretixOrganizer, PretixEvent pretixEvent)
    {
        var db = _organizerRepository.CreateContext();
        
        var organizerFromDb = _organizerRepository.GetOrCreate(pretixOrganizer, db);
        var eventFromDb = _eventRepository.GetOrCreate(pretixEvent, organizerFromDb, db);

        ActiveSetting.SelectedOrganizer = organizerFromDb;
        ActiveSetting.SelectedEvent = eventFromDb;
        
        Save(db);
        db.Dispose();
    }

    public void Save(DbContext db)
    {
        _applicationSettingsRepository.Update(ActiveSetting, db);
        Load();
    }
    
    public ApplicationSetting ActiveSetting { get; set; }
}