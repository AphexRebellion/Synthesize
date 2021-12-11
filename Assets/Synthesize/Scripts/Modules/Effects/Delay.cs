using UnityEngine;

namespace MGB.AudioGraph.Modules
{
	[System.Serializable]
	public class Delay : ModuleBase
	{
		public const float KMinTime = 0.0f;
		public const float KMaxTime = 3.0f;

		[Range(KMinTime, KMaxTime)]
		public float TimeVal;
		[Range(0, 1)]
		public float FeedbackVal;
		[Range(0, 1)]
		public float DryWetVal;

		[ModuleInput]
		public ModuleOutputBinder Time { get; set; }
		[ModuleInput]
		public ModuleOutputBinder DryWet { get; set; }
		[ModuleInput]
		public ModuleOutputBinder Feedback { get; set; }

		protected float[] mDelayLine;
		protected int mWriteIndex;

		public Delay(float aTime, float aDryWet, float aFeedback) : base(new[] { "Output", "Dry", "Wet" })
		{
			TimeVal = aTime;
			DryWetVal = aDryWet;
			FeedbackVal = aFeedback;

			mDelayLine = new float[1024];
		}

		// Ensures delay line is big enough for the requested delay time.
		protected void AllocBuffer()
		{
			int samples = (int)(SampleRate * TimeVal);
			if (mDelayLine == null || mDelayLine.Length < samples)
			{
				var buffer = new float[samples * 2];  // Double up buffer to help reduce reallocs.
				Debug.Log("Realloc delay buffer, new len: " + buffer.Length);
				if (mDelayLine != null)
					mDelayLine.CopyTo(buffer, 0);
				mDelayLine = buffer;
			}
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

			// Calc tap index of delayed signal:
			int delayOffset = (int)(SampleRate * TimeVal);
			int tapIndex = (mWriteIndex - delayOffset);
			if (tapIndex < 0)
				tapIndex += mDelayLine.Length;

			float wet = mDelayLine[tapIndex];

			if (FeedbackVal > 0)
			{
				var fbi = 0;//FeedbackInput != null ? FeedbackInput.GetValue() : 0;
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
