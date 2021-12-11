using MGB.AudioGraph.Modules;
using MGB.AudioGraph.Nodes;
using UnityEngine;

[System.Serializable]
[CreateNodeMenu(KBaseMenu + KGeneratorsMenu + "Custom Wave")]
public class CustomWaveNode : GeneratorNode
{
	public AnimationCurve Wave;
	[Input(ShowBackingValue.Unconnected, ConnectionType.Override)] public float Freq = 261.6f; // = Middle C
	[Input(ShowBackingValue.Unconnected, ConnectionType.Override)] public float Amp = 1.0f;
	[Tooltip("Value added to frequency.  Used for modulation.")]
	[Input(ShowBackingValue.Unconnected, ConnectionType.Override)] public float FreqMod = 0.0f;

	public override ModuleBase CreateModule()
	{
		var copy = new AnimationCurve(Wave.keys);
		return new CustomWave(copy, Freq, Amp, FreqMod);
	}
}
