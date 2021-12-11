using MGB.AudioGraph.Modules;
using MGB.AudioGraph.Nodes;
using UnityEngine;

[System.Serializable]
[CreateNodeMenu(KBaseMenu + "Master Output")]
public class OutputNode : AudioNode
{
	[Input] public float Mono;
	[Input(ShowBackingValue.Unconnected, ConnectionType.Override)] public float Left;
	[Input(ShowBackingValue.Unconnected, ConnectionType.Override)] public float Right;
	[Input(ShowBackingValue.Unconnected, ConnectionType.Override)] public float MasterAmp = 1.0f;
	public float DcOffset;

	public override ModuleBase CreateModule()
	{
		return new Output(MasterAmp, DcOffset);
	}
}
