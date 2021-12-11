using UnityEngine;

namespace MGB.AudioGraph.Modules
{
	/// <summary>
	/// Expanded noise generator.
	/// Use for audio noise, random number generation, control/modulation.
	/// </summary>
	[System.Serializable]
	public class NoiseEx : ModuleBase
	{
		public float FreqVal;
		public float Min;
		public float Max;
		public float ChangeRate;

		private System.Random mRnd;
		private float mOldCur;
		private float mTgtVal;
		private int mTick;

		[ModuleInput]
		public ModuleOutputBinder Freq { get; set; }

		public NoiseEx(int aSeed, float aMin, float aMax, float aFreq, float aChangeRate)
		{
			Min = aMin;
			Max = aMax;
			FreqVal = aFreq;
			ChangeRate = aChangeRate;

			mRnd = new System.Random(aSeed);
			OutputValues[0] = mOldCur = mTgtVal = CalcNextVal();
		}

		public override void Tick()
		{
			if (RoutingMode != RoutingMode.Active)
			{
				OutputValues[0] = 0;
				return;
			}

			FreqVal = GetInputVal(Freq, FreqVal).ClampLower(0.001f);

			mTick++;
			int tt = (int)(SampleRate * (1 / FreqVal));

			if (!Mathf.Approximately(OutputValues[0], mTgtVal))
			{
				if (ChangeRate <= 0)
				{
					OutputValues[0] = mTgtVal;
				}
				else
				{
					float frac = ChangeRate * (mTick / (float)tt);
					OutputValues[0] = Mathf.Lerp(mOldCur, mTgtVal, frac);
				}
			}

			if (mTick >= tt)
			{
				mOldCur = OutputValues[0];
				mTgtVal = CalcNextVal();
				mTick -= tt;
			}
		}

		private float CalcNextVal()
		{
			return Mathf.Lerp(Min, Max, (float)mRnd.NextDouble());
		}
	}
}
