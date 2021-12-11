using MGB.AudioGraph.Modules;
using MGB.AudioGraph.Nodes;
using UnityEngine;

[System.Serializable]
[CreateNodeMenu(KBaseMenu + KGeneratorsMenu + "LFO")]
public class LFONode : GeneratorNode
{
	public WaveType WaveType;
	[Tooltip("Value added to signal.")]
	[Input(ShowBackingValue.Unconnected, ConnectionType.Override)] public float Offset;
	[Input(ShowBackingValue.Unconnected, ConnectionType.Override)] public float Freq = 2.0f;
	[Input(ShowBackingValue.Unconnected, ConnectionType.Override)] public float Amp = 1.0f;
	[Tooltip("Width of peak for pulse wave.")]
	[Input(ShowBackingValue.Unconnected, ConnectionType.Override)] public float PulseWidth = 0.25f;
	[Tooltip("Value added to frequency.  Used for modulation.")]
	[Input(ShowBackingValue.Unconnected, ConnectionType.Override)] public float FreqMod = 0.0f;
	[Tooltip("Inverts the wave's amplitude.")]
	public bool Invert;

	public override ModuleBase CreateModule()
	{
		return new LFO(WaveType, Freq, Amp, PulseWidth, FreqMod, Invert, Offset);
	}
}
