using MGB.AudioGraph.Modules;
using MGB.AudioGraph.Nodes;
using UnityEngine;

[System.Serializable]
[CreateNodeMenu(KBaseMenu + KEffectsMenu + "Quantiser")]
public class QuantiserNode : EffectsNode
{
	[Tooltip("Number of discrete levels to round to.")]
	[Input(ShowBackingValue.Unconnected, ConnectionType.Override)] public float Resolution = 16;

	public override ModuleBase CreateModule()
	{
		return new Quantiser((int)Resolution);
	}
}
