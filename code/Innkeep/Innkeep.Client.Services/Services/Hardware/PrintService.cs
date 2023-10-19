﻿using ESCPOS;
using Innkeep.Client.Services.Interfaces.File;
using Innkeep.Client.Services.Interfaces.Hardware;
using Innkeep.Core.DomainModels.Print;
using Innkeep.Models.Printer;
using Innkeep.Printer.Document;

namespace Innkeep.Client.Services.Services.Hardware;

public class PrintService : IPrintService
{
	private readonly IClientSettingsService _clientSettingsService;

	public PrintService(IClientSettingsService clientSettingsService)
	{
		_clientSettingsService = clientSettingsService;
	}
	
	public void TestPage()
	{
		if (string.IsNullOrEmpty(_clientSettingsService.Setting.PrinterComPort)) return;

		var manager = new DocumentManager(_clientSettingsService.Setting.PrinterComPort);
		manager.AddTitle("Test Page").Cut().Print();

	}

	public void Print(Receipt result)
	{
		if (string.IsNullOrEmpty(_clientSettingsService.Setting.PrinterComPort)) return;
		var manager = new DocumentManager(_clientSettingsService.Setting.PrinterComPort);

		foreach (var line in result.Lines)
		{
			switch (line.LineType)
			{
				case LineType.Title:
					manager.AddTitle(line.Content);
					break;

				case LineType.Line:
					manager.AddLine(line.Content);
					break;
				
				case LineType.Center:
					manager.AddLine(line.Content, Justification.Center);
					break;
				
				case LineType.Sum:
					manager.AddLine(line.Content, Justification.Right);
					break;

				case LineType.Blank:
					manager.AddEmptyLine();
					break;

				case LineType.Cut:
					manager.Cut();
					break;

				case LineType.Divider:
					manager.AddLine(new string('-', 42));
					break;
				
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		manager.Cut();
		manager.Print();
	}
}