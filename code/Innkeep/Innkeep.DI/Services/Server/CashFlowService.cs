﻿using Innkeep.Core.DomainModels.Dashboard;
using Innkeep.Server.Data.Interfaces;
using Innkeep.Server.Data.Models;
using Transaction = Innkeep.Shared.Objects.Transaction.Transaction;

namespace Innkeep.DI.Services.Server;

public class CashFlowService : ICashFlowService
{
	private readonly ICashFlowRepository _cashFlowRepository;
	private readonly IApplicationSettingsService _applicationSettingsService;

	public CashFlowService
		(ICashFlowRepository cashFlowRepository, IApplicationSettingsService applicationSettingsService)
	{
		_cashFlowRepository = cashFlowRepository;
		_applicationSettingsService = applicationSettingsService;
	}
	
	public void CreateCashFlow(Register register, Transaction transaction)
	{
		_cashFlowRepository.Create(
			new CashFlow()
			{
				TimeStamp = DateTime.Now,
				Event = _applicationSettingsService.ActiveSetting.SelectedEvent!,
				Register = register,
				MoneyAdded = transaction.AmountGiven,
				MoneyRemoved = Math.Abs(transaction.Return),
			}
		);
	}

	public List<RegisterCashInfo> GetCurrentCashState()
	{
		if (_applicationSettingsService.ActiveSetting.SelectedEvent == null) return new List<RegisterCashInfo>();

		var cashFlows = _cashFlowRepository
						.GetAllCustom(x => x.Event.Slug == _applicationSettingsService.ActiveSetting.SelectedEvent.Slug)
						.GroupBy(x => x.Register);

		return GetCashInfo(cashFlows);
	}

	private List<RegisterCashInfo> GetCashInfo(IEnumerable<IGrouping<Register, CashFlow>> cashFlows)
	{
		return cashFlows.Select(
							grouping => new RegisterCashInfo()
							{
								RegisterId = grouping.Key.DeviceId,
								CashState = grouping.Sum(x => x.TotalMoney),
							}
						)
						.ToList();
	}
}