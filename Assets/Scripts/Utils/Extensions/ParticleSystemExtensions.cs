using UnityEngine;

namespace Utils.Extensions
{
	public static class ParticleSystemExtensions
	{
		public static void SetDuration(this ParticleSystem particle, float duration)
		{
			ParticleSystem.MainModule mainModule = particle.main;
			mainModule.duration = duration;
		}
	}
}