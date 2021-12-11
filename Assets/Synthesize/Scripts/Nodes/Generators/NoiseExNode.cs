using MGB.AudioGraph.Modules;
using MGB.AudioGraph.Nodes;
using UnityEngine;

[System.Serializable]
[CreateNodeMenu(KBaseMenu + KGeneratorsMenu + "Noise Ex")]
public class NoiseExNode : GeneratorNode
{
	[Tooltip("Freq of value change, per second")]
	[Input(ShowBackingValue.Unconnected, ConnectionType.Override)] public float Freq = 1.0f;
	[Tooltip("Min value that will be generated.")]
	public float Min = 0.0f;
	[Tooltip("Max value that will be generated.")]
	public float Max = 1.0f;
	[Tooltip("Rate output changes to new random value (0 = instant).")]
	public float ChangeRate = 0.0f;
	public int Seed = (int)(System.DateTime.UtcNow.Ticks & 0xffffffff);

	public override ModuleBase CreateModule()
	{
		return new NoiseEx(Seed, Min, Max, Freq, ChangeRate);
	}
}
