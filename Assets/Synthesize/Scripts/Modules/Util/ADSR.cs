using UnityEngine;

namespace MGB.AudioGraph.Modules
{
	/// <summary>
	/// General wave envelope.
	/// Use to shape wave amplitude.
	/// </summary>
	[System.Serializable]
	public class ADSR : ModuleBase
	{
		public ADSR(float aAttack, float aDecay, float aSustain, float aRelease)
		{
			AttackVal = aAttack;
			DecayVal = aDecay;
			SustainVal = aSustain;
			ReleaseVal = aRelease;
		}

		public float ControlVal;	// Simulates key press: 0 = released, above 0 = held.
		public float AttackVal;		// Time from 0 to full amplitude.
		public float DecayVal;		// Time from max amp to sustain level.
		[Range(0.0f, 1.0f)]
		public float SustainVal;	// Amp % level after decay.
		public float ReleaseVal;    // Time from sustain level to zero.

		[ModuleInput]
		public ModuleOutputBinder Control { get; set; }
		[ModuleInput]
		public ModuleOutputBinder Attack { get; set; }
		[ModuleInput]
		public ModuleOutputBinder Decay { get; set; }
		[ModuleInput]
		public ModuleOutputBinder Sustain { get; set; }
		[ModuleInput]
		public ModuleOutputBinder Release { get; set; }

		private float mT;
		private float mAmpMod;
		private float mReleaseStartVal;
		private float mLastCtrlVal;
		private bool mCtrlHeld;

		private float mCurAmp;

		// todo: test handling of zero vals
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

			ControlVal = GetInputVal(Control, ControlVal);
			mCtrlHeld = ControlVal > 0;

			if (mLastCtrlVal <= 0 && ControlVal > 0)
			{
				// Restart:
				mT = 0;
				mAmpMod = 0;
				mReleaseStartVal = 0;
			}
			else if (mLastCtrlVal > 0 && ControlVal <= 0)
			{
				// Enter Release phase:
				mT = 0;
				mReleaseStartVal = mAmpMod;
			}
			mLastCtrlVal = ControlVal;

			if (mCtrlHeld)
			{
				AttackVal = GetInputVal(Attack, AttackVal);
				DecayVal = GetInputVal(Decay, DecayVal).ClampLower(0.01f);
				SustainVal = GetInputVal(Sustain, SustainVal);

				if (mT >= 0 && mT < AttackVal)
				{
					// Attack phase:
					mAmpMod = Mathf.Lerp(0, 1, mT / AttackVal);
				}
				else if (mT >= AttackVal && mT < AttackVal + DecayVal)
				{
					// Decay phase:
					float t01 = (mT - AttackVal) / DecayVal;
					mAmpMod = Mathf.Lerp(1, SustainVal, t01);
				}
				else // Attack & decay are zero - straight to sustain level:
				{
					mAmpMod = SustainVal;
				}
			}
			else
			{
				// Release phase:
				if (mReleaseStartVal > 0)
				{
					ReleaseVal = GetInputVal(Release, ReleaseVal);
					if (ReleaseVal > 0)
						mAmpMod = Mathf.Lerp(mReleaseStartVal, 0, mT / ReleaseVal);
					else
						mAmpMod = 0;
				}
			}

			mT += (1.0f / (float)SampleRate);
			mCurAmp = Mathf.MoveTowards(mCurAmp, mAmpMod, KLevelSmoothDelta);	// Filter amp discontinuities.

			if (mCurAmp <= 0 || RoutingMode == RoutingMode.Disabled)
			{
				OutputValues[0] = 0.0f;
			}
			else // Active
			{
				float input = SumGenericInputs();
				OutputValues[0] = input * mCurAmp;
			}
		}

		// Manual control hold.
		[Util.AudioCall]
		public void ControlHold()
		{
			ControlVal = 1;
		}

		// Manual control release.
		[Util.AudioCall]
		public void ControlRelease()
		{
			ControlVal = 0;
		}
	}
}
