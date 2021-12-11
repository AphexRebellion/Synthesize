using UnityEngine;

namespace MGB.AudioGraph.Modules
{
	[System.Serializable]
	public class Envelope : ModuleBase
	{
		public Envelope(float aAmp, AnimationCurve aCurve, float aTime, bool aLoop, RetriggerType aTriggerType)
		{
			AmpVal = aAmp;
			Curve = aCurve;
			TimeVal = aTime;
			Loop = aLoop;
			ReTrigger = aTriggerType;
		}

		public AnimationCurve Curve;
		public float TimeVal;
		public float AmpVal;
		public bool Loop;
		public RetriggerType ReTrigger;

		[ModuleInput]
		public ModuleOutputBinder Time { get; set; }
		[ModuleInput]
		public ModuleOutputBinder Amp { get; set; }
		[ModuleInput]
		public ModuleOutputBinder Trigger { get; set; }

		public AudioModuleEvent OnEnvelopeEnded;

		private float mT;
		private float mLastTriggerVal;
		private bool mReachedEnd;
		private float mCurAmp;

		public override void Tick()
		{
			if (RoutingMode == RoutingMode.Frozen)
			{
				OutputValues[0] = 0;
				return;
			}
			else if (RoutingMode == RoutingMode.Bypass)
			{
				OutputValues[0] = SumGenericInputs();
				return;
			}

			CheckRetrigger();

			if (mReachedEnd && !Loop)
			{
				OutputValues[0] = 0;
				return;
			}

			if (RoutingMode == RoutingMode.Active)
			{
				float val = 1.0f;
				if (GenericInputs.Count > 0)
					val = SumGenericInputs();
				AmpVal = GetInputVal(Amp, AmpVal);
				float ct = Curve.Evaluate(mT);
				mCurAmp = Mathf.MoveTowards(mCurAmp, ct, KLevelSmoothDelta);	// Filter out mT discontinuities.
				val *= mCurAmp * AmpVal;
				OutputValues[0] = val;
			}
			else // Disabled
			{
				OutputValues[0] = 0;
			}

			TimeVal = GetInputVal(Time, TimeVal).ClampLower(0.001f);
			mT += (1 / (float)SampleRate) / TimeVal;	// Q: poor precision?!

			if (mT >= 1.0f)
			{
				if (!mReachedEnd && OnEnvelopeEnded != null)
					OnEnvelopeEnded.Invoke(this);

				if (Loop)
				{
					mT -= 1.0f;
					mReachedEnd = false;
				}
				else
				{
					mReachedEnd = true;
				}
			}
		}

		private void CheckRetrigger()
		{
			if (ReTrigger == RetriggerType.OneShot && !mReachedEnd)
				return;

			if (Trigger != null)
			{
				float tv = Trigger.GetValue();
				if (mLastTriggerVal <= 0 && tv > 0) // Trigger on rising above 0.
				{
					TriggerNow();
				}
				mLastTriggerVal = tv;
			}
		}

		[Util.AudioCall]
		public void TriggerNow()
		{
			if (ReTrigger == RetriggerType.OneShot && !mReachedEnd)
				return;

			mT = 0;
			mReachedEnd = false;
		}
	}
}
