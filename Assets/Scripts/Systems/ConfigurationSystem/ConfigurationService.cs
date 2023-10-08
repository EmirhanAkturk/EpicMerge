using GameDepends;
using UnityEngine;

namespace Systems.ConfigurationSystem
{
	public static class ConfigurationService
	{
		public static GameConfigurations Configurations
		{
			get
			{
				if (!_isLoaded)
				{
					Load();
				}
				return _configurations;
			}
		}

		private static bool _isLoaded = false;
		private static GameConfigurations _configurations;

		private const string CONFIGURATION_PATH = "Configurations/GameConfigurations";

		private static void Load()
		{
			_configurations = Resources.Load<GameConfigurations>(CONFIGURATION_PATH);
			_isLoaded = true;
		}
	}
}