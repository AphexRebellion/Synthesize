using UnityEngine;

namespace MGB.AudioGraph.Modules
{
	/// <summary>
	/// Maps input to output value via curve lookup.
	/// Uses: an envelope, lookup table, value transfer fn, basic amplitude waveshaping, distortion.
	/// </summary>
	[System.Serializable]
	public class ResponseCurve : ModuleBase
	{
		public AnimationCurve Curve;
		[Range(-1, 1)]
		public float TVal;
		public float InputScaleVal;

		[ModuleInput]
		public ModuleOutputBinder T { get; set; }
		[ModuleInput]
		public ModuleOutputBinder InputScale { get; set; }

		public ResponseCurve(AnimationCurve aCurve, float aT, float aInputScale)
		{
			Curve = aCurve;
			TVal = aT;
			InputScaleVal = aInputScale;
		}

		public override void Tick()
		{
			if (RoutingMode == RoutingMode.Frozen || RoutingMode == RoutingMode.Disabled)
			{
				OutputValues[0] = 0;
				return;
			}
			else if (RoutingMode == RoutingMode.Bypass)
			{
				OutputValues[0] = GetInputVal(T, TVal);
				return;
			}

			TVal = GetInputVal(T, TVal);
			InputScaleVal = GetInputVal(InputScale, InputScaleVal);
			OutputValues[0] = Curve.Evaluate(TVal * InputScaleVal);
		}
	}
}
