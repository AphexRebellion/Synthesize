using UnityEngine;

namespace MGB.AudioGraph.Modules
{
	/// <summary>
	/// Custom wave generator.
	/// Used for custom wave shapes, modulation.
	/// </summary>
	[System.Serializable]
	public class CustomWave : GeneratorModule
	{
		public CustomWave(AnimationCurve aWave, float aFreq, float aAmp, float aFreqMod)
			: base(aFreq, aAmp, aFreqMod)
		{
			Wave = aWave;
		}

		public AnimationCurve Wave;

		private float mT;

		public override void Tick()
		{
			if (RoutingMode == RoutingMode.Frozen)
			{
				OutputValues[0] = 0;
				return;
			}

			AmpVal = GetInputVal(Amp, AmpVal);

			if (RoutingMode == RoutingMode.Active)
				OutputValues[0] = Wave.Evaluate(mT) * AmpVal;
			else
				OutputValues[0] = 0;

			FreqVal = GetInputVal(Freq, FreqVal);
			FreqModVal = GetInputVal(FreqMod, FreqModVal);

			mT += ((FreqVal + FreqModVal) / (float)SampleRate);
			if (mT >= 1.0f)
				mT -= 1.0f;
		}
	}
}
