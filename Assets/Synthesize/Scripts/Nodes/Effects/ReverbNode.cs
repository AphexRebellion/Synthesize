using MGB.AudioGraph.Modules;
using MGB.AudioGraph.Nodes;
using UnityEngine;

[System.Serializable]
[CreateNodeMenu(KBaseMenu + KEffectsMenu + "Reverb")]
public class ReverbNode : EffectsNode
{
	[Tooltip("Length of effect.")]
	[Input(ShowBackingValue.Unconnected, ConnectionType.Override)]
	public float Time = 0.5f;

	[Tooltip("Mix between input and processed signal.")]
	[Range(0.0f, 1.0f)]
	[Input(ShowBackingValue.Unconnected, ConnectionType.Override)]
	public float DryWet = 0.5f;

	[Tooltip("Amount of signal to feed back in.")]
	[Range(0.0f, 1.0f)]
	[Input(ShowBackingValue.Unconnected, ConnectionType.Override)]
	public float Feedback = 0.5f;

	[Tooltip("Defines how the sound reflects over time.")]
	public AnimationCurve ImpulseResponse;

	[Tooltip("Number of sample points taken per second.")]
	public int TapsPerSec = 10;

	public override ModuleBase CreateModule()
	{
		var copy = new AnimationCurve(ImpulseResponse.keys);
		return new Reverb(Time, TapsPerSec, DryWet, Feedback, copy);
	}
}
