using MGB.AudioGraph.Modules;
using MGB.AudioGraph.Nodes;
using UnityEngine;

[System.Serializable]
[CreateNodeMenu(KBaseMenu + KUtilMenu + "Math")]
public class MathNode : AudioNode
{
	[Output] public float Output;
	[Input] public float Input;

	public Math.MathOp Operation;

	public override ModuleBase CreateModule()
	{
		return new Math(Operation);
	}
}
