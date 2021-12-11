using MGB.AudioGraph.Modules;
using MGB.AudioGraph.Nodes;
using UnityEngine;

[System.Serializable]
[CreateNodeMenu(KBaseMenu + KFiltersMenu + "Eq3Band")]
public class Eq3BandNode : FilterNode
{
	[Tooltip("Low frequency gain")]
	[Range(0,5)]
	[Input(ShowBackingValue.Unconnected, ConnectionType.Override)] public float Low=1;

	[Tooltip("Med frequency gain")]
	[Range(0, 5)]
	[Input(ShowBackingValue.Unconnected, ConnectionType.Override)] public float Med=1;

	[Tooltip("High frequency gain")]
	[Range(0, 5)]
	[Input(ShowBackingValue.Unconnected, ConnectionType.Override)] public float High=1;

	public override ModuleBase CreateModule()
	{
		return new Eq3Band(Low, Med, High);
	}
}
