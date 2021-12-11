using UnityEngine;

namespace MGB.AudioGraph.Modules
{
	/// <summary>
	/// Basic frequency filter.
	/// Used for attenuating chosen frequencies in a signal.
	/// </summary>
	[System.Serializable]
	public class BasicFilter : ModuleBase
	{
		public float CutOffVal;
		[Range(0, 0.99f)]
		public float ResonanceVal;
		public FilterMode Mode;

		[ModuleInput]
		public ModuleOutputBinder CutOff { get; set; }
		[ModuleInput]
		public ModuleOutputBinder Resonance { get; set; }

		float b0, b1, b2, b3, b4;

		public BasicFilter(float aCutOff, float aResonance, FilterMode aMode)
		{
			CutOffVal = aCutOff;
			ResonanceVal = aResonance;
			Mode = aMode;
		}

		// http://musicdsp.org/archive.php?classid=3#196
		// todo: bad feedback at high cutoff and res!
		public override void Tick()
		{
			if (!CheckRouting())
				return;

			CutOffVal = GetInputVal(CutOff, CutOffVal);
			ResonanceVal = GetInputVal(Resonance, ResonanceVal);

			// MGB: make cutoff [0..1] for this code
			float cutOff = Mathf.Clamp01(CutOffVal / SampleRate);

			// Set coefficients given frequency & resonance [0.0...1.0]
			float q = 1.0f - cutOff;
			float p = cutOff + 0.8f * cutOff * q;
			float f = p + p - 1.0f;
			q = ResonanceVal * (1.0f + 0.5f * q * (1.0f - q + 5.6f * q * q));

			// Filter (in [-1.0...+1.0])
			float input = SumGenericInputs();
			//MGB:? input = Mathf.Clamp(input, -1, 1);

			input -= q * b4;    //feedback
			float t1 = b1;
			b1 = (input + b0) * p - b1 * f;
			float t2 = b2;
			b2 = (b1 + t1) * p - b2 * f;
			t1 = b3; b3 = (b2 + t2) * p - b3 * f;
			b4 = (b3 + t1) * p - b4 * f;
			b4 = b4 - b4 * b4 * b4 * 0.166667f; //clipping
			b0 = input;

			OutputValues[0] = Mode == FilterMode.LowPass ? b4 : Mode == FilterMode.HighPass ? input - b4 : 3 * (b3 - b4);
		}
	}
}
