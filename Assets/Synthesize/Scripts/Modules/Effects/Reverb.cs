using UnityEngine;

namespace MGB.AudioGraph.Modules
{
	/// <summary>
	/// Simulates sound reflections in enclosed spaces.
	/// Uses simple time domain amplitude convolve with variable taps.
	/// </summary>
	[System.Serializable]
	public class Reverb : Delay
	{
		public AnimationCurve ImpulseResponse;
		public int TapsPerSec;

		public Reverb(float aTime, int aTapsPerSec, float aDryWet, float aFeedback, AnimationCurve aImpulseResponse) : base(aTime, aDryWet, aFeedback)
		{
			TapsPerSec = aTapsPerSec;
			ImpulseResponse = aImpulseResponse;
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

			TimeVal = GetInputVal(Time, TimeVal).Clamp(KMinTime, KMaxTime);
			DryWetVal = GetInputVal(DryWet, DryWetVal).Clamp01();
			FeedbackVal = GetInputVal(Feedback, FeedbackVal).Clamp01();

			AllocBuffer();

			float dry = SumGenericInputs();
			mDelayLine[mWriteIndex] = dry;

			float wet = 0.0f;
			int taps = (int)(TapsPerSec * TimeVal);
			for (int tap=0 ; tap<taps ; tap++)
			{
				float irT = tap/(float)taps;
				float irC = ImpulseResponse.Evaluate(irT);
				int delayOffset = (int)(SampleRate * TimeVal * irT);
				int tapIndex = (mWriteIndex - delayOffset);
				if (tapIndex < 0)
					tapIndex += mDelayLine.Length;
				wet += mDelayLine[tapIndex] * irC;
			}

			if (FeedbackVal > 0)
			{
				var fbi = 0;
				float fVal = dry + (FeedbackVal * (wet + fbi));
				mDelayLine[mWriteIndex] = fVal;
			}

			if (RoutingMode == RoutingMode.Active)
			{
				OutputValues[0] = SoundUtils.WetDryCombine(dry, wet, DryWetVal);
				OutputValues[1] = dry;
				OutputValues[2] = wet;
			}
			else
			{
				OutputValues[0] = OutputValues[1] = OutputValues[2] = 0;
			}

			mWriteIndex++;
			if (mWriteIndex >= mDelayLine.Length)
				mWriteIndex = 0;
		}
	}
}
