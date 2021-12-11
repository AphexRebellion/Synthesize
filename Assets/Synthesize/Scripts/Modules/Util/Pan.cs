using UnityEngine;

namespace MGB.AudioGraph.Modules
{
	/// <summary>
	/// Moves the input signal across the stereo field.
	/// </summary>
	[System.Serializable]
	public class PanModule : ModuleBase
	{
		[Range(-1, 1)]
		public float PanVal;

		[ModuleInput]
		public ModuleOutputBinder Pan { get; set; }

		public PanModule(float aPan) : base(new[] {"Left", "Right"})
		{
			PanVal = aPan;
		}

		public override void Tick()
		{
			float input = SumGenericInputs();
			PanVal = GetInputVal(Pan, PanVal);
			OutputValues[0] = input * (1 - Mathf.Max(PanVal, 0));
			OutputValues[1] = input * (1 - Mathf.Max(-PanVal, 0));
		}
	}
}
