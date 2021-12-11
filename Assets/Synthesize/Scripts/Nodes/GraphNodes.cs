using MGB.AudioGraph.Modules;
using UnityEngine;
using XNode;

// Nodes for audio node graph.
namespace MGB.AudioGraph.Nodes
{
	[System.Serializable]
	public abstract class AudioNode : Node
	{
		public abstract ModuleBase CreateModule();

		public const string KBaseMenu = "AudioGraph/";
		public const string KGeneratorsMenu = "Generators/";
		public const string KEffectsMenu = "Effects/";
		public const string KFiltersMenu = "Filters/";
		public const string KUtilMenu = "Util/";
	}

	[System.Serializable]
	[NodeTint(192,255,192)]
	public abstract class GeneratorNode : AudioNode
	{
		[Output] public float Output;
	}

	[System.Serializable]
	[NodeTint(255,180,180)]
	public abstract class EffectsNode : AudioNode
	{
		[Output] public float Output;
		[Input] public float Input;
	}

	[System.Serializable]
	[NodeTint(192,192,255)]
	public abstract class FilterNode : AudioNode
	{
		[Output] public float Output;
		[Input] public float Input;
	}
}
