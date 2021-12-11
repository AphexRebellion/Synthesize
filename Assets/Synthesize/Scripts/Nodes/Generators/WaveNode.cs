using MGB.AudioGraph.Modules;
using MGB.AudioGraph.Nodes;
using UnityEngine;

[System.Serializable]
[CreateNodeMenu(KBaseMenu + KGeneratorsMenu + "Wave")]
public class WaveNode : GeneratorNode
{
	public WaveType WaveType;
	public Polarity Polarity;
	[Input(ShowBackingValue.Unconnected, ConnectionType.Override)] public float Freq = 440.0f;
	[Input(ShowBackingValue.Unconnected, ConnectionType.Override)] public float Amp = 1.0f;
	[Input(ShowBackingValue.Unconnected, ConnectionType.Override)] public float PulseWidth = 0.25f;
	[Tooltip("Value added to frequency, for modulation.")]
	[Input(ShowBackingValue.Unconnected, ConnectionType.Override)] public float FreqMod = 0.0f;
	[Tooltip("Inverts the wave's amplitude.")]
	public bool Invert;

	public override ModuleBase CreateModule()
	{
		return new Wave(WaveType, Freq, Amp, PulseWidth, Polarity, FreqMod, Invert);
	}
}
