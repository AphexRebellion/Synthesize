using UnityEngine;

namespace MGB.AudioGraph.Modules
{
	/// <summary>
	/// Selects between one of four inputs.
	/// </summary>
	[System.Serializable]
	public class Switcher : ModuleBase
	{
		public float ChannelVal;
		public bool Fade;

		[ModuleInput]
		public ModuleOutputBinder A { get; set; }
		[ModuleInput]
		public ModuleOutputBinder B { get; set; }
		[ModuleInput]
		public ModuleOutputBinder C { get; set; }
		[ModuleInput]
		public ModuleOutputBinder D { get; set; }
		[ModuleInput]
		public ModuleOutputBinder Channel { get; set; }

		private ModuleOutputBinder[] mInputs;
		private float mSmoothedAmp;

		public Switcher(float aChannel, bool aFade)
		{
			ChannelVal = aChannel;
			Fade = aFade;
		}

		public override void Initialise()
		{
			base.Initialise();

			mInputs = new[] { A, B, C, D };
		}

		public override void Tick()
		{
			ChannelVal = GetInputVal(Channel, ChannelVal).Clamp(0.0f, 3.9999f);
			int idx = (int)ChannelVal;
			OutputValues[0] = GetInputVal(mInputs[idx], 0.0f);

			if (Fade)
			{
				float dc = ChannelVal - idx;
				float amp = 1.0f - Mathf.Abs((dc - 0.5f) * 2);
				mSmoothedAmp = Mathf.MoveTowards(mSmoothedAmp, amp, KLevelSmoothDelta);
				OutputValues[0] *= mSmoothedAmp;
			}
		}
	}
}
