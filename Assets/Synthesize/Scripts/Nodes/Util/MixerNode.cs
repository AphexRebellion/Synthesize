using MGB.AudioGraph.Modules;
using MGB.AudioGraph.Nodes;
using UnityEngine;

[System.Serializable]
[CreateNodeMenu(KBaseMenu + KUtilMenu + "Mixer")]
public class MixerNode : AudioNode
{
	[Output] public float Output;

	[Input(ShowBackingValue.Unconnected, ConnectionType.Override)] public float A;
	[Input(ShowBackingValue.Unconnected, ConnectionType.Override)] public float B;
	[Input(ShowBackingValue.Unconnected, ConnectionType.Override)] public float C;
	[Input(ShowBackingValue.Unconnected, ConnectionType.Override)] public float D;
	[Input(ShowBackingValue.Unconnected, ConnectionType.Override)] public float LevelA;
	[Input(ShowBackingValue.Unconnected, ConnectionType.Override)] public float LevelB;
	[Input(ShowBackingValue.Unconnected, ConnectionType.Override)] public float LevelC;
	[Input(ShowBackingValue.Unconnected, ConnectionType.Override)] public float LevelD;

	public override ModuleBase CreateModule()
	{
		return new Mixer(LevelA, LevelB, LevelC, LevelD);
	}
}
