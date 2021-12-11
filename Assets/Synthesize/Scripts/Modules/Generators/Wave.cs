using UnityEngine;

namespace MGB.AudioGraph.Modules
{
	/// <summary>
	/// Basic waveform generator.
	/// </summary>
	[System.Serializable]
	public class Wave : GeneratorModule
	{
		public Wave(WaveType aWaveType, float aFreq, float aAmp, float aPulseWidth, Polarity aPolarity, float aFreqMod, bool aInvert)
			: base(aFreq, aAmp, aFreqMod)
		{
			WaveType = aWaveType;
			Polarity = aPolarity;
			PulseWidthVal = aPulseWidth;
			Invert = aInvert;
		}

		public WaveType WaveType;
		[Range(0, 1)]
		public Polarity Polarity;
		[Range(0.0001f, 0.9999f)]
		public float PulseWidthVal;
		public bool Invert;

		[ModuleInput]
		public ModuleOutputBinder PulseWidth { get; set; }

		float mFreqAcc;

		public override void Tick()
		{
			if (RoutingMode == RoutingMode.Frozen)
			{
				OutputValues[0] = 0;
				return;
			}

			float t = mFreqAcc / (float)SampleRate;
			float val = 0.0f;
			switch (WaveType)
			{
				case WaveType.Sine:
					float rad = t * Mathf.PI * 2;
					val = Mathf.Sin(rad) * 0.5f;
					break;

				case WaveType.Square:
					val = t < 0.5f ? 0.5f : -0.5f;
					break;

				case WaveType.Pulse:
				{
					PulseWidthVal = GetInputVal(PulseWidth, PulseWidthVal);
					val = t < PulseWidthVal ? 0.5f : -0.5f;
					break;
				}

				case WaveType.Saw:
					val = t - 0.5f;
					break;

				case WaveType.Triangle:
					val = (t < 0.5f ? t : 1.0f - t) * 2 - 0.5f;
					break;
			}

			AmpVal = GetInputVal(Amp, AmpVal);
			val = Invert ? -val : val;
			val += Polarity == Polarity.Positive ? 0.5f : Polarity == Polarity.Negative ? -0.5f : 0.0f;
			val *= AmpVal;
			
			if (RoutingMode == RoutingMode.Active)
				OutputValues[0] = val;
			else
				OutputValues[0] = 0;

			FreqVal = GetInputVal(Freq, FreqVal);
			FreqModVal = GetInputVal(FreqMod, FreqModVal);
			mFreqAcc += FreqVal + FreqModVal;
			if (mFreqAcc >= SampleRate)
				mFreqAcc -= SampleRate;
		}
	}
}
