using UnityEngine;

namespace MGB.AudioGraph.Modules
{
	[System.Serializable]
	public class Math : ModuleBase
	{
		[System.Serializable]
		public enum MathOp
		{
			Add,
			Mul,
			Sub,
			Max,
			Min
		}

		public Math(MathOp aOp)
		{
			Operation = aOp;
		}

		public MathOp Operation;

		public override void Tick()
		{
			if (!CheckRouting())
				return;

			float val = GenericInputs[0].GetValue();
			for (int i = 1; i < GenericInputs.Count; i++)
			{
				float iv = GenericInputs[i].GetValue();
				val = ApplyOp(val, iv);
			}

			OutputValues[0] = val;
		}

		private float ApplyOp(float a, float b)
		{
			switch (Operation)
			{
				case MathOp.Add: return a + b;
				case MathOp.Mul: return a * b;
				case MathOp.Sub: return a - b;
				case MathOp.Max: return Mathf.Max(a, b);
				case MathOp.Min: return Mathf.Min(a, b);

				default:
					Debug.LogError("Unhandled Combiner operation: " + Operation);
					break;
			}

			return a;
		}
	}
}

