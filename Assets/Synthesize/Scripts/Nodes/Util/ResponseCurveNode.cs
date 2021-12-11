using MGB.AudioGraph.Modules;
using MGB.AudioGraph.Nodes;
using UnityEngine;

[System.Serializable]
[CreateNodeMenu(KBaseMenu + KUtilMenu + "Response Curve")]
public class ResponseCurveNode : AudioNode
{
	[Output] public float Output;

	public AnimationCurve Curve;
	[Tooltip("Value to use on Curve.")]
	[Range(0.0f, 1.0f)]
	[Input(ShowBackingValue.Unconnected, ConnectionType.Override)] public float T = 0.0f;

	[Tooltip("Input pre-scale value.")]
	[Input(ShowBackingValue.Unconnected, ConnectionType.Override)] public float InputScale = 1.0f;

	public override ModuleBase CreateModule()
	{
		var copy = new AnimationCurve(Curve.keys);
		return new ResponseCurve(copy, T, InputScale);
	}
}
