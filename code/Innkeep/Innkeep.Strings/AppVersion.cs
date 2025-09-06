namespace Innkeep.Strings;

public static class AppVersion
{
	public const string Version = "0.2.9";

	public static string ClientAppTitle => $"Innkeep Client v{Version}";

	public static string ServerAppTitle => $"Innkeep Server v{Version}";
}