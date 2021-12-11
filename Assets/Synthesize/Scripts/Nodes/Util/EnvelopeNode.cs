using MGB.AudioGraph.Modules;
using MGB.AudioGraph.Nodes;
using UnityEngine;

[System.Serializable]
[CreateNodeMenu(KBaseMenu + KUtilMenu + "Envelope")]
public class EnvelopeNode : AudioNode
{
	[Output] public float Output;

	[Input] public float Input;
	[Tooltip("Envelope duration.")]
	[Input(ShowBackingValue.Unconnected, ConnectionType.Override)] public float Time = 2;
	[Input(ShowBackingValue.Unconnected, ConnectionType.Override)] public float Amp = 1;
	[Tooltip("Restarts envelope on rising above zero.")]
	[Input(ShowBackingValue.Unconnected, ConnectionType.Override)] public float Trigger = 0;
	public RetriggerType TrigType;

	public bool Loop;
	public AnimationCurve Curve;

	public override ModuleBase CreateModule()
	{
		var copy = new AnimationCurve(Curve.keys);
		return new Envelope(Amp, copy, Time, Loop, TrigType);
	}
}
