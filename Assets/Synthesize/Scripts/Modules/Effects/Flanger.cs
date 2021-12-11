using UnityEngine;

namespace MGB.AudioGraph.Modules
{
	/// <summary>
	/// Applies an animated phasing effect to the sound, modulated with a built-in sine LFO.
	/// Effectively like applying a frequency-independent comb filter.
	/// Notes: High feedback can extremely change the sound!
	/// </summary>
	[System.Serializable]
	public class Flanger : Delay
	{
		const float KFlangerMaxDelayTime = 0.02f;

		[Range(0.0f, 1.0f)]
		public float DepthVal;
		public float RateVal;

		[ModuleInput]
		public ModuleOutputBinder Depth { get; set; }
		[ModuleInput]
		public ModuleOutputBinder Rate { get; set; }
		[ModuleInput]
		public ModuleOutputBinder Envelope { get; set; }

		private float mFreqAcc;

		public Flanger(float aDepth, float aRate, float aFeedback) : base(KMinTime, 0.5f, aFeedback)
		{
			DepthVal = aDepth;
			RateVal = aRate;
		}

		public override void Tick()
		{
			if (RoutingMode == RoutingMode.Frozen)
			{
				OutputValues[0] = OutputValues[1] = OutputValues[2] = 0;
				return;
			}
			else if (RoutingMode == RoutingMode.Bypass)
			{
				OutputValues[0] = OutputValues[1] = OutputValues[2] = SumGenericInputs();
				return;
			}

			DepthVal = GetInputVal(Depth, DepthVal).Clamp01();
			float depthMul = Mathf.Lerp(0, KFlangerMaxDelayTime, DepthVal);

			RateVal = GetInputVal(Rate, RateVal);
			float t = mFreqAcc/(float)SampleRate;

			float modVal = 0.0f;
			if (Envelope != null)
			{
				modVal = Envelope.GetValue();
			}
			else
			{
				// Use LFO sine to mod depth:
				float phase = t * Mathf.PI * 2;
				modVal = (Mathf.Sin(phase) * 0.5f) + 0.5f;
			}

			// Depth is actally delay time...
			TimeVal = depthMul * Mathf.Abs(modVal);

			base.Tick();

			if (Envelope == null)
			{
				mFreqAcc += RateVal;
				if (mFreqAcc >= SampleRate)
					mFreqAcc -= SampleRate;
			}
		}
	}
}
