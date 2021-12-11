using MGB.AudioGraph.Modules;
using MGB.AudioGraph.Nodes;
using UnityEngine;

[System.Serializable]
[CreateNodeMenu(KBaseMenu + KGeneratorsMenu + "Sample")]
public class SampleNode : AudioNode
{
	[Output] public float Output;

	public AudioClip Sample;
	public bool Loop;
	[Input(ShowBackingValue.Unconnected, ConnectionType.Override)] public float Amp = 1;
	[Tooltip("Frequency multiplier for modulation.")]
	[Input(ShowBackingValue.Unconnected, ConnectionType.Override)] public float FreqMul = 1;
	[Tooltip("Restarts sample on rising above zero.")]
	[Input(ShowBackingValue.Unconnected, ConnectionType.Override)] public float Trigger = 0;
	public RetriggerType TrigType;
	[Input(ShowBackingValue.Unconnected, ConnectionType.Override)] public float PreDelay = 0;

	public override ModuleBase CreateModule()
	{
		return new Sample(Sample, Amp, FreqMul, Loop, TrigType, PreDelay);
	}
}
