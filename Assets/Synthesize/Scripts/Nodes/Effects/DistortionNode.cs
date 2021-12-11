using MGB.AudioGraph.Modules;
using MGB.AudioGraph.Nodes;
using UnityEngine;

[System.Serializable]
[CreateNodeMenu(KBaseMenu + KEffectsMenu + "Distortion")]
public class DistortionNode : EffectsNode
{
	[Tooltip("Amplitude above which signal is clipped.")]
	[Input(ShowBackingValue.Unconnected, ConnectionType.Override)] public float Level = 0.75f;

	public override ModuleBase CreateModule()
	{
		return new Distortion(Level);
	}
}
