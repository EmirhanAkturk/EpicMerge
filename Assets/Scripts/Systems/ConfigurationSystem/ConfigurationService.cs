using UnityEngine;

public static class ConfigurationService
{
	public static GameConfigurations Configurations
	{
		get
		{
			if (!isLoaded)
			{
				Load();
			}
			return configurations;
		}
	}

	private static bool isLoaded = false;
	private static GameConfigurations configurations;

	private const string ConfigurationPath = "Configurations/GameConfigurations";

	private static void Load()
	{
		configurations = Resources.Load<GameConfigurations>(ConfigurationPath);
		isLoaded = true;
	}
}