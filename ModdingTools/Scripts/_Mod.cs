using UnityEngine;
using System;


	class Mod
	{
		public static void Main(string[] args)
		{
			
		}
		void Init()
		{
			RenderSettings.fog =true;
			RenderSettings.fogMode = FogMode.ExponentialSquared;
			RenderSettings.fogDensity=22;
			bs._Hud.WriteChat("hello 22");
		}
	}
