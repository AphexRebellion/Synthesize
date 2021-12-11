using MGB.AudioGraph.Modules;
using MGB.AudioGraph.Nodes;
using UnityEngine;

[System.Serializable]
[CreateNodeMenu(KBaseMenu + KEffectsMenu + "Delay")]
public class DelayNode : EffectsNode
{
	[Output] public float Dry;
	[Output] public float Wet;

	[Tooltip("Time until delayed signal is heard.")]
	[Range(Delay.KMinTime, Delay.KMaxTime)]
	[Input(ShowBackingValue.Unconnected, ConnectionType.Override)] public float Time = 0.25f;

	[Tooltip("Amount of processed signal to feed back into input.")]
	[Range(0.0f, 1.0f)]
	[Input(ShowBackingValue.Unconnected, ConnectionType.Override)] public float Feedback = 0.5f;

	[Tooltip("Mix between input and processed signal.")]
	[Range(0.0f, 1.0f)]
	[Input(ShowBackingValue.Unconnected, ConnectionType.Override)] public float DryWet = 0.5f;

	//? [Input(ShowBackingValue.Unconnected, ConnectionType.Override)] public float FeedbackInput;

	public override ModuleBase CreateModule()
	{
		return new Delay(Time, DryWet, Feedback);
	}
}
