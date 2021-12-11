using MGB.AudioGraph.Modules;
using UnityEngine;

namespace MGB.AudioGraph.Examples
{
	public class KeyboardLayer : MonoBehaviour
	{
		public MonoAudioGraph Graph;

		public int Octave;

		const int KMaxOctave = 8;

		private Float mFreqModule;
		private Float mKeyGate;
		private Float mPitchBend;
		private Float mModWheel;

		void Start()
		{
			mFreqModule = Graph.Modules["Input_Freq"] as Float;
			mKeyGate = Graph.Modules["Input_KeyGate"] as Float;
			mPitchBend = Graph.Modules["Input_PitchBend"] as Float;
			//mModWheel = Graph.Modules["Input_ModWheel"] as Float;

			Octave = 3;
		}

		public void DecOctave()
		{
			if (Octave > 1)
				Octave--;
		}

		public void IncOctave()
		{
			if (Octave < KMaxOctave)
				Octave++;
		}

		private float CalcKeyFreq(int aKey)
		{
			const float KTwelfthRoot2 = 1.0594631f;
			return 440 * Mathf.Pow(KTwelfthRoot2, aKey-49);
		}

		public void OnKeyDown(int aKeyCode)
		{
			int key = (Octave-1)*12 + aKeyCode + 4;
			float freq = CalcKeyFreq(key);
			mFreqModule.Value = freq;
			mKeyGate.Value = 1;
		}

		public void OnKeyUp(int aKeyCode)
		{
			mKeyGate.Value = 0;
		}

		public float PitchBend
		{
			get { return mPitchBend.Value; }
			set { mPitchBend.Value = value; }
		}

		public float ModWheel
		{
			get { return mModWheel.Value; }
			set { mModWheel.Value = value; }
		}

		private void Update()
		{
			var keys = new[] { KeyCode.A, KeyCode.W, KeyCode.S, KeyCode.E, KeyCode.D, KeyCode.F, KeyCode.T, KeyCode.G, KeyCode.Y, KeyCode.H, KeyCode.U, KeyCode.J, KeyCode.K, KeyCode.O, KeyCode.L, KeyCode.P, KeyCode.Semicolon, KeyCode.BackQuote};

			for (int k=0; k<keys.Length; k++)
			{
				if (Input.GetKeyDown(keys[k]))
					OnKeyDown(k);
				else if (Input.GetKeyUp(keys[k]))
					OnKeyUp(k);
			}
		}
		//Coroutine mPulseCR;
		//private void Pulse(Float aTrigger)
		//{
		//	if (mPulseCR != null)
		//		StopCoroutine(mPulseCR);
		//	mPulseCR = StartCoroutine(PulseFloat(aTrigger, 0.1f));
		//}
		//
		//private IEnumerator PulseFloat(Float aTrigger, float aTime)
		//{
		//	aTrigger.Value = 1.0f;
		//	yield return new WaitForSeconds(aTime);
		//	aTrigger.Value = 0.0f;
		//}
	}
}
