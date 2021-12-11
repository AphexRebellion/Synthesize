using MGB.AudioGraph.Modules;
using MGB.AudioGraph.Nodes;
using UnityEngine;

[System.Serializable]
[CreateNodeMenu(KBaseMenu + KUtilMenu + "Pan")]
public class PanNode : AudioNode
{
	[Output] public float Left;
	[Output] public float Right;

	[Input] public float Input;
	[Tooltip("Stereo balance to apply to input (-1 to +1)")]
	[Range(-1.0f, 1.0f)]
	[Input(ShowBackingValue.Unconnected, ConnectionType.Override)] public float Pan;

	public override ModuleBase CreateModule()
	{
		return new PanModule(Pan);
	}
}
