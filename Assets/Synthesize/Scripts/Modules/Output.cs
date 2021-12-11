using UnityEngine;

namespace MGB.AudioGraph.Modules
{
	[System.Serializable]
	public class Output : ModuleBase
	{
		public float MasterAmpVal = 1.0f;
		public float DcOffset;

		// Note: Mono input in via GenericInputs.
		[ModuleInput]
		public ModuleOutputBinder Left { get; set; }
		[ModuleInput]
		public ModuleOutputBinder Right { get; set; }
		[ModuleInput]
		public ModuleOutputBinder MasterAmp { get; set; }

		public Output(float aMasterAmp, float aDcOffset) : base (new[] {"Left", "Right"})
		{
			MasterAmpVal = aMasterAmp;
			DcOffset = aDcOffset;
		}

		public override void Tick()
		{
			MasterAmpVal = GetInputVal(MasterAmp, MasterAmpVal);

			float left = SumGenericInputs();
			float right = left;

			if (Left != null)
				left += Left.GetValue();

			if (Right != null)
				right += Right.GetValue();

			OutputValues[0] = left * MasterAmpVal + DcOffset;
			OutputValues[1] = right * MasterAmpVal + DcOffset;
		}

		public override bool Validate()
		{
			if (GenericInputs.Count == 0 && Left == null && Right == null)
			{
				Debug.LogError("Master Output node has no inputs!");
				return false;
			}

			return true;
		}
	}
}
