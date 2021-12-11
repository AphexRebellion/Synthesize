using MGB.AudioGraph.Modules;
using MGB.AudioGraph.Nodes;
using UnityEngine;

[System.Serializable]
[CreateNodeMenu(KBaseMenu + KUtilMenu + "Switcher")]
public class SwitcherNode : AudioNode
{
	[Output] public float Output;

	[Input(ShowBackingValue.Unconnected, ConnectionType.Override)] public float A;
	[Input(ShowBackingValue.Unconnected, ConnectionType.Override)] public float B;
	[Input(ShowBackingValue.Unconnected, ConnectionType.Override)] public float C;
	[Input(ShowBackingValue.Unconnected, ConnectionType.Override)] public float D;
	[Input(ShowBackingValue.Unconnected, ConnectionType.Override)] public float Channel;
	public bool Fade;

	public override ModuleBase CreateModule()
	{
		return new Switcher(Channel, Fade);
	}
}
