
namespace MGB.AudioGraph.Modules
{
	[System.Serializable]
	public class Float : ModuleBase
	{
		[ModuleField]
		public float Value;

		public Float(float aValue)
		{
			Value = aValue;
		}

		public override void Tick()
		{
			OutputValues[0] = Value;
		}
	}
}
