using MGB.AudioGraph.Modules;
using MGB.AudioGraph.Nodes;
using UnityEngine;

[System.Serializable]
[CreateNodeMenu(KBaseMenu + KEffectsMenu + "Flanger")]
public class FlangerNode : EffectsNode
{
	[Output] public float Dry;
	[Output] public float Wet;

	[Tooltip("Amount of effect to apply.")]
	[Range(0.0f, 1.0f)]
	[Input(ShowBackingValue.Unconnected, ConnectionType.Override)]
	public float Depth = 1.0f;

	[Tooltip("Freq of modulator LFO.")]
	[Input(ShowBackingValue.Unconnected, ConnectionType.Override)]
	public float Rate = 0.1f;

	[Tooltip("Amount of signal to feed back in.")]
	[Range(0.0f, 1.0f)]
	[Input(ShowBackingValue.Unconnected, ConnectionType.Override)]
	public float Feedback = 0.5f;

	[Tooltip("Override the modulating LFO.")]
	[Input(ShowBackingValue.Unconnected, ConnectionType.Override)]
	public float Envelope = 0.0f;

	public override ModuleBase CreateModule()
	{
		return new Flanger(Depth, Rate, Feedback);
	}
}
