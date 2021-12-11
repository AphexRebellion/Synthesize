using UnityEngine;

namespace MGB.AudioGraph.Modules
{
	public class Eq3Band : ModuleBase
	{
		const float KSmall = 1/4294967295.0f;
		const float KLowFreq = 800;
		const float KHiFreq = 5000;

		public float mGainLow;
		public float mGainMed;
		public float mGainHi;

		float mLpf0, mLpf1, mLpf2, mLpf3;
		float mHpf0, mHpf1, mHpf2, mHpf3;
		float mLowFreq, mHiFreq;
		float ib0, ib1, ib2;	// Input delay buffer.

		public Eq3Band(float aLowGain, float aMedGain, float aHiGain)
		{
			mGainLow = aLowGain;
			mGainMed = aMedGain;
			mGainHi = aHiGain;

			mLowFreq = 2 * Mathf.Sin(Mathf.PI * (KLowFreq / (float)ModuleBase.SampleRate));
			mHiFreq = 2 * Mathf.Sin(Mathf.PI * (KHiFreq / (float)ModuleBase.SampleRate));
		}

		public override void Tick()
		{
			if (!CheckRouting())
				return;

			float inVal = SumGenericInputs();

			// Lowpass filter:
			mLpf0 += (mLowFreq * (inVal - mLpf0)) + KSmall;	// Add small delta to stop denormal.
			mLpf1 += (mLowFreq * (mLpf0 - mLpf1));
			mLpf2 += (mLowFreq * (mLpf1 - mLpf2));
			mLpf3 += (mLowFreq * (mLpf2 - mLpf3));
			float l = mLpf3;

			// Highpass filter:
			mHpf0 += (mHiFreq * (inVal - mHpf0)) + KSmall;	// Add small delta to stop denormal.
			mHpf1 += (mHiFreq * (mHpf0 - mHpf1));
			mHpf2 += (mHiFreq * (mHpf1 - mHpf2));
			mHpf3 += (mHiFreq * (mHpf2 - mHpf3));
			float h = ib2 - mHpf3;

			// Calculate midrange = out - (low + high)
			float m = ib2 - (h + l);

			// Propagate delay buffer:
			ib2 = ib1;
			ib1 = ib0;
			ib0 = inVal;

			l *= Mathf.Max(0, mGainLow);
			m *= Mathf.Max(0, mGainMed);
			h *= Mathf.Max(0, mGainHi);

			OutputValues[0] = l + m + h;
		}
	}
}
