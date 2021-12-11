using UnityEngine;

namespace MGB.AudioGraph.Modules
{
	/// <summary>
	/// Clips signal amplitudes to below the specified level.
	/// </summary>
	[System.Serializable]
	public class Distortion : ModuleBase
	{
		[Range(0.01f, 1.0f)]
		public float LevelVal;

		[ModuleInput]
		public ModuleOutputBinder Level {get; set;}

		public Distortion(float aLevel)
		{
			LevelVal = aLevel;
		}

		public override void Tick()
		{
			if (!CheckRouting())
				return;

			float input = SumGenericInputs();

			LevelVal = GetInputVal(Level, LevelVal).ClampLower(0.01f);

			if (!Mathf.Approximately(LevelVal, 1.0f)) // todo: eh?
			{
				// todo: try this: If X>L then Y=2L - L/X
				if (input > 0)
					input = Mathf.Min(input, LevelVal);
				else
					input = Mathf.Max(input, -LevelVal);

				// Scale to account for clipping, and roughly handle RMS changes:  todo: scales too loud at low levels (or too quiet at hi levels!)
				float scale = Mathf.Sqrt(0.5f / LevelVal);
				input *= scale;
			}

			OutputValues[0] = input;
		}
	}
}
