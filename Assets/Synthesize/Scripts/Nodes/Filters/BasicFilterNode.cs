using MGB.AudioGraph.Modules;
using MGB.AudioGraph.Nodes;
using UnityEngine;

[System.Serializable]
[CreateNodeMenu(KBaseMenu + KFiltersMenu + "Basic Filter")]
public class BasicFilterNode : FilterNode
{
	[Tooltip("Frequency to apply filter at.")]
	[Input(ShowBackingValue.Unconnected, ConnectionType.Override)] public float CutOff = 1000;
	[Tooltip("Size of frequency peak at cutoff.")]
	[Input(ShowBackingValue.Unconnected, ConnectionType.Override)] public float Resonance = 0.25f;
	public MGB.AudioGraph.Modules.FilterMode Mode;

	public override ModuleBase CreateModule()
	{
		return new BasicFilter(CutOff, Resonance, Mode);
	}
}
