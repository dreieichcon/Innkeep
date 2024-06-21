﻿using ESCPOS;
using Innkeep.Client.Services.Legacy.Interfaces.File;
using Innkeep.Client.Services.Legacy.Interfaces.Hardware;
using Innkeep.Models.Printer;
using Innkeep.Printer.Document;
using Serilog;

namespace Innkeep.Client.Services.Legacy.Services.Hardware;

public class PrintService : IPrintService
{
	private readonly IClientSettingsService _clientSettingsService;
	
	public Receipt? LastReceipt { get; set; }

	public PrintService(IClientSettingsService clientSettingsService)
	{
		_clientSettingsService = clientSettingsService;
	}
	
	public void TestPage()
	{
		if (string.IsNullOrEmpty(_clientSettingsService.Setting.PrinterComPort))
		{
			Log.Debug("No Com Port in Settings - Aborting Print.");
			return;
		}

		Log.Debug("Test Page Printing.");
		var manager = new DocumentManager(_clientSettingsService.Setting.PrinterComPort);
		manager.AddTitle("Test Page").Cut().Print();

	}

	public void Drawer()
	{
		Log.Debug("OPENING DRAWER");
		var manager = new DocumentManager(_clientSettingsService.Setting.PrinterComPort);
		manager.Drawer();
	}

	public void PrintImage(string path)
	{
		var manager = new DocumentManager(_clientSettingsService.Setting.PrinterComPort);
		manager.AddImage(path)
				.Cut()
				.Print();
	}

	public void Print()
	{
		if (string.IsNullOrEmpty(_clientSettingsService.Setting.PrinterComPort)) return;
		var manager = new DocumentManager(_clientSettingsService.Setting.PrinterComPort);

		foreach (var line in LastReceipt.Lines)
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
				
				case LineType.Qr:
					manager.AddQr(line.Content);
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