using UnityEngine;

namespace MGB.AudioGraph.Modules
{
	/// <summary>
	/// Set levels of up to four inputs.
	/// </summary>
	[System.Serializable]
	public class Mixer : ModuleBase
	{
		public float LevelAVal;
		public float LevelBVal;
		public float LevelCVal;
		public float LevelDVal;

		public Mixer(float aLevelA, float aLevelB, float aLevelC, float aLevelD)
		{
			LevelAVal = mLevelA = aLevelA;
			LevelBVal = mLevelB = aLevelB;
			LevelCVal = mLevelC = aLevelC;
			LevelDVal = mLevelD = aLevelD;
		}

		[ModuleInput]
		public ModuleOutputBinder A { get; set; }
		[ModuleInput]
		public ModuleOutputBinder B { get; set; }
		[ModuleInput]
		public ModuleOutputBinder C { get; set; }
		[ModuleInput]
		public ModuleOutputBinder D { get; set; }
		[ModuleInput]
		public ModuleOutputBinder LevelA{ get; set; }
		[ModuleInput]
		public ModuleOutputBinder LevelB{ get; set; }
		[ModuleInput]
		public ModuleOutputBinder LevelC{ get; set; }
		[ModuleInput]
		public ModuleOutputBinder LevelD{ get; set; }

		// To smooth level changes to reduce popping.
		private float mLevelA;
		private float mLevelB;
		private float mLevelC;
		private float mLevelD;

		public override void Tick()
		{
			OutputValues[0] = 0;

			if (A != null)
			{
				LevelAVal = GetInputVal(LevelA, LevelAVal);
				mLevelA = Mathf.MoveTowards(mLevelA, LevelAVal, KLevelSmoothDelta);
				OutputValues[0] += A.GetValue() * mLevelA;
			}

			if (B != null)
			{
				LevelBVal = GetInputVal(LevelB, LevelBVal);
				mLevelB = Mathf.MoveTowards(mLevelB, LevelBVal, KLevelSmoothDelta);
				OutputValues[0] += B.GetValue() * mLevelB;
			}

			if (C != null)
			{
				LevelCVal = GetInputVal(LevelC, LevelCVal);
				mLevelC = Mathf.MoveTowards(mLevelC, LevelCVal, KLevelSmoothDelta);
				OutputValues[0] += C.GetValue() * mLevelC;
			}

			if (D != null)
			{
				LevelDVal = GetInputVal(LevelD, LevelDVal);
				mLevelD = Mathf.MoveTowards(mLevelD, LevelDVal, KLevelSmoothDelta);
				OutputValues[0] += D.GetValue() * mLevelD;
			}
		}
	}
}
