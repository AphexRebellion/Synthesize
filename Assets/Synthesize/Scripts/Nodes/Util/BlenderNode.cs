using MGB.AudioGraph.Modules;
using MGB.AudioGraph.Nodes;
using UnityEngine;

[System.Serializable]
[CreateNodeMenu(KBaseMenu + KUtilMenu + "Blender")]
public class BlenderNode : AudioNode
{
	[Output] public float Output;

	[Input(ShowBackingValue.Unconnected, ConnectionType.Override)] public float Input1;
	[Input(ShowBackingValue.Unconnected, ConnectionType.Override)] public float Input2;
	[Tooltip("Mix fraction of inputs.")]
	[Range(0.0f, 1.0f)]
	[Input(ShowBackingValue.Unconnected, ConnectionType.Override)] public float Mix;

	public override ModuleBase CreateModule()
	{
		return new Blender(Input1, Input2, Mix);
	}
}
