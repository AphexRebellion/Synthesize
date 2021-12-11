using MGB.AudioGraph.Modules;
using MGB.AudioGraph.Nodes;

[System.Serializable]
[CreateNodeMenu(KBaseMenu + KGeneratorsMenu + "Float")]
public class FloatNode : GeneratorNode
{
	public float Value;

	public override ModuleBase CreateModule()
	{
		return new Float(Value);
	}
}
