
using UnityEngine;

namespace AudioGraph.Filters
{
	//http://www.martin-finke.de/blog/articles/audio-plugins-013-filter/
	public class BasicIIRFilter
	{
		public MGB.AudioGraph.Modules.FilterMode Mode;
		public float Cutoff;		// 0 < c < 1
		public float Resonance;		// 0..1
		public float Feedback;		// 0..1

		float mBuf0, mBuf1, mBuf2, mBuf3;

		public float Filter(float aVal)
		{
			//if (aVal == 0)	// Q: Won't this mess up val propagation/accumulation?
			//	return 0;

			float cutoff = Mathf.Max(Mathf.Min(Cutoff, 0.99f), 0.01f);
			float feedback = Mathf.Min(Resonance + Resonance/(1.0f - cutoff), 1.0f);
			mBuf0 += cutoff * (aVal - mBuf0 + feedback);
			mBuf1 += cutoff * (mBuf0 - mBuf1);
			mBuf2 += cutoff * (mBuf1 - mBuf2);
			mBuf3 += cutoff * (mBuf2 - mBuf1);

			switch (Mode)
			{
				case MGB.AudioGraph.Modules.FilterMode.LowPass:
					return mBuf3;
				case MGB.AudioGraph.Modules.FilterMode.HighPass:
					return aVal - mBuf3;
				case MGB.AudioGraph.Modules.FilterMode.BandPass:
					return mBuf0 - mBuf3;
			}

			return 0;
		}
	}

	// https://stackoverflow.com/questions/10205618/programming-a-low-pass-filter
	//public class BiquadFilter
	//{
	//	public BiquadFilter(float a1, float a2, float b1, float b2)
	//	{
	//		this.a1 = a1;
	//		this.a2 = a2;
	//		this.b1 = b1;
	//		this.b2 = b2;
	//	}

	//	float b1, b2;
	//	float a1, a2;
	//	float m1, m2;
	//	float dn = 1e-20f;

	//	public float Process(float aVal)
	//	{
	//		float w = aVal - a1 * m1 - a2 * m2 + dn;
	//		float result = b1 * m1 + b2 * m2 + w;
	//		m2 = m1; m1 = w;
	//		dn = -dn;
	//		return result;
	//	}
	//}
}
