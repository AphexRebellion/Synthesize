using MGB.AudioGraph.Modules;
using MGB.AudioGraph.Nodes;
using UnityEngine;

[System.Serializable]
[CreateNodeMenu(KBaseMenu + KGeneratorsMenu + "Noise")]
public class NoiseNode : GeneratorNode
{
	[Input(ShowBackingValue.Unconnected, ConnectionType.Override)] public float Amp = 1.0f;
	public Polarity Polarity;
	public int Seed = (int)(System.DateTime.UtcNow.Ticks & 0xffffffff);

	public override ModuleBase CreateModule()
	{
		return new Noise(Seed, Polarity, Amp);
	}
}
