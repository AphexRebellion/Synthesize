
namespace MGB.AudioGraph.Modules
{
	/// <summary>
	/// Basic noise generator.
	/// </summary>
	[System.Serializable]
	public class Noise : GeneratorModule
	{
		public Polarity Polarity;

		private System.Random mRnd;

		public Noise(int aSeed, Polarity aPolarity, float aAmp) : base(0, aAmp, 0)
		{
			Polarity = aPolarity;
			mRnd = new System.Random(aSeed);
		}

		public override void Tick()
		{
			if (RoutingMode != RoutingMode.Active)
			{
				OutputValues[0] = 0;
				return;
			}

			AmpVal = GetInputVal(Amp, AmpVal);

			OutputValues[0] = (float)mRnd.NextDouble() * AmpVal;

			if (Polarity == Polarity.Bipolar)
				OutputValues[0] -= 0.5f;
			else if (Polarity == Polarity.Negative)
				OutputValues[0] = -OutputValues[0];
		}
	}
}
