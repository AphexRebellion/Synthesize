using UnityEngine;
using UnityEngine.UI;

namespace MGB.AudioGraph.Examples
{
	public class SynthGui : MonoBehaviour
	{
		public KeyboardLayer Keyboard;
		public int PitchBendRange = 100;
		public int ModWheelRange = 100;
		public Text OctaveText;
		public bool Slide;

		private void Start()
		{
			UpdateOctaveText();
		}

		public void OnPitchBend(float aVal)
		{
			Keyboard.PitchBend = aVal * PitchBendRange;
		}

		public void OnModWheel(float aVal)
		{
			Keyboard.ModWheel = aVal * ModWheelRange;
		}

		public void OnClickOctaveDown()
		{
			Keyboard.DecOctave();
			UpdateOctaveText();
		}

		public void OnClickOctaveUp()
		{
			Keyboard.IncOctave();
			UpdateOctaveText();
		}

		private void UpdateOctaveText()
		{
			OctaveText.text = Keyboard.Octave.ToString();
		}
	}
}
