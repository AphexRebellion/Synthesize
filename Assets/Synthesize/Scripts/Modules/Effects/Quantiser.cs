using UnityEngine;

namespace MGB.AudioGraph.Modules
{
	/// <summary>
	/// Transforms the signal to have a number of discrete amplitudes.
	/// Use as a method of distortion.
	/// </summary>
	[System.Serializable]
	public class Quantiser : ModuleBase
	{
		public int Resolution;

		public Quantiser(int aRes)
		{
			Resolution = aRes;
		}

		public override void Tick()
		{
			if (!CheckRouting())
				return;

			float input = SumGenericInputs();
			
			if (Resolution == 0)
			{
				OutputValues[0] = input;
				return;
			}

			// Dead-zone quantiser (symmetrical about zero).
			float fRes = Mathf.Max(Resolution, 1);
			OutputValues[0] = (int)(((input * Resolution) + (Mathf.Sign(input) / fRes))) / fRes;
		}
	}
}
