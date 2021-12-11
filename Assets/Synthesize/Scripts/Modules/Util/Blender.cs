using UnityEngine;

namespace MGB.AudioGraph.Modules
{
	/// <summary>
	/// Blends between two inputs.
	/// Used to create a mix of two signals.
	/// </summary>
	[System.Serializable]
	public class Blender : ModuleBase
	{
		public float Input1Val;
		public float Input2Val;
		[Range(0.0f, 1.0f)]
		public float MixVal;

		public Blender(float aInput1, float aInput2, float aMix)
		{
			Input1Val = aInput1;
			Input2Val = aInput2;
			MixVal = aMix;
		}

		[ModuleInput]
		public ModuleOutputBinder Input1 { get; set; }
		[ModuleInput]
		public ModuleOutputBinder Input2 { get; set; }
		[ModuleInput]
		public ModuleOutputBinder Mix { get; set; }

		public override void Tick()
		{
			Input1Val = GetInputVal(Input1, Input1Val);
			Input2Val = GetInputVal(Input2, Input2Val);
			MixVal = GetInputVal(Mix, MixVal);
			OutputValues[0] = Mathf.Lerp(Input1Val, Input2Val, MixVal);
		}
	}
}
