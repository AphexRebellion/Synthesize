using UnityEngine;
using UnityEngine.Events;

namespace MGB.AudioGraph.Modules
{
	[System.Serializable]
	public class Sample : ModuleBase
	{
		public AudioClip Clip;
		public bool Loop;
		public float AmpVal;
		public float FreqMulVal;
		public float PreDelayVal;

		[ModuleInput]
		public ModuleOutputBinder Amp { get; set; }
		[ModuleInput]
		public ModuleOutputBinder FreqMul { get; set; }
		[ModuleInput]
		public ModuleOutputBinder Trigger { get; set; }
		[ModuleInput]
		public ModuleOutputBinder PreDelay { get; set; }

		public RetriggerType ReTrigger;

		public AudioModuleEvent OnClipEnded;

		protected float[] mClipData;
		protected int mSampleIndex;
		protected int mTick;
		protected int mClipFreq;

		protected float mLastTriggerVal;
		protected bool mReachedEnd;
		protected float mDelay;

		public Sample(AudioClip aClip, float aAmp, float aFreqMod, bool aLoop, RetriggerType aTriggerType, float aPreDelay)
		{
			if (aClip == null)
				return;

			Clip = aClip;
			AmpVal = aAmp;
			FreqMulVal = aFreqMod;
			Loop = aLoop;
			ReTrigger = aTriggerType;

			mClipFreq = Clip.frequency;

			if (Clip.loadType != AudioClipLoadType.DecompressOnLoad)
				Clip.LoadAudioData();

			mClipData = new float[Clip.samples * Clip.channels];    // OPT: might wanna stream part of sample?
			Clip.GetData(mClipData, 0);

			mDelay = PreDelayVal = aPreDelay;
		}

		public override void Tick()
		{
			if (RoutingMode == RoutingMode.Frozen)
			{
				OutputValues[0] = 0;
				return;
			}

			CheckRetrigger();

			if (mReachedEnd && !Loop)
			{
				OutputValues[0] = 0;
				return;
			}

			if (mDelay > 0)
			{
				mDelay -= 1.0f / (float)SampleRate;
				OutputValues[0] = 0;
				return;
			}

			FreqMulVal = GetInputVal(FreqMul, FreqMulVal);
			mSampleIndex = (int)(mTick * ((mClipFreq * FreqMulVal) / (float)SampleRate));

			if (mSampleIndex >= mClipData.Length)
			{
				if (!mReachedEnd && OnClipEnded != null)
					OnClipEnded.Invoke(this);

				if (Loop)
				{
					mTick = 0;
					mSampleIndex -= mClipData.Length;
					mReachedEnd = false;
					PreDelayVal = GetInputVal(PreDelay, PreDelayVal);
					mDelay = PreDelayVal;
				}
				else
				{
					mReachedEnd = true;
					return;
				}
			}

			AmpVal = GetInputVal(Amp, AmpVal);

			if (RoutingMode == RoutingMode.Active)
				OutputValues[0] = mClipData[mSampleIndex] * AmpVal;
			else
				OutputValues[0] = 0;

			mTick++;
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

			mTick = 0;
			mReachedEnd = false;
			mSampleIndex = 0;

			PreDelayVal = GetInputVal(PreDelay, PreDelayVal);
			mDelay = PreDelayVal;
		}
	}
}
