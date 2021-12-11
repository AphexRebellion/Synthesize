using MGB.AudioGraph.Modules;
using MGB.AudioGraph.Nodes;
using UnityEngine;

[System.Serializable]
[CreateNodeMenu(KBaseMenu + KUtilMenu + "ADSR")]
public class ADSRNode : AudioNode
{
	[Output] public float Output;

	[Input] public float Input;
	[Tooltip("Gate/control. ADS applied while above zero (held). Sustain applied when zero (released).")]
	[Input(ShowBackingValue.Unconnected, ConnectionType.Override)] public float Control;
	[Tooltip("Time taken to reach max amplitude.")]
	[Input(ShowBackingValue.Unconnected, ConnectionType.Override)] public float Attack = 0.1f;
	[Tooltip("Time taken to fade to Sustain level.")]
	[Input(ShowBackingValue.Unconnected, ConnectionType.Override)] public float Decay = 0.5f;
	[Tooltip("Amplitude level of sustain phase.")]
	[Input(ShowBackingValue.Unconnected, ConnectionType.Override)] public float Sustain = 0.5f;
	[Tooltip("Time taken to fade from sustain level to zero.")]
	[Input(ShowBackingValue.Unconnected, ConnectionType.Override)] public float Release = 0.25f;

	public override ModuleBase CreateModule()
	{
		return new ADSR(Attack, Decay, Sustain, Release);
	}
}
