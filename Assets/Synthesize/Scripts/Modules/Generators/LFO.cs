
namespace MGB.AudioGraph.Modules
{
	/// <summary>
	/// Low frequency oscillator - pretty much same as a wave.
	/// Used for mainly control/modulation, or wave generation.
	/// </summary>
	[System.Serializable]
	public class LFO : Wave
	{
		public float Offset;

		public LFO(WaveType aWaveType, float aFreq, float aAmp, float aPulseWidth, float aFreqMod, bool aInvert, float aOffset)
			: base(aWaveType, aFreq, aAmp, aPulseWidth, Polarity.Bipolar, aFreqMod, aInvert)
		{
			Offset = aOffset;
		}

		public override void Tick()
		{
			base.Tick();
			OutputValues[0] += Offset;
		}
	}
}
