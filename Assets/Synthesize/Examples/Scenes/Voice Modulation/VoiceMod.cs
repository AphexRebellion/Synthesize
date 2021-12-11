using MGB.AudioGraph.Modules;
using UnityEngine;

namespace MGB.AudioGraph.Examples
{
	public class VoiceMod : MonoBehaviour
	{
		// Cached graph modules:
		private NoiseEx mFmModule;
		private Distortion mDistortionModule;
		private Sample mVoiceSampleModule;

		private UIKnob mFmFreqKnob;
		private UIKnob mFmDepthKnob;
		private UIKnob mDistortionKnob;

		void Start()
		{
			var graph = FindObjectOfType<MonoAudioGraph>();
		
			mVoiceSampleModule = graph.FindModule<Sample>("Voice Sample");

			mFmModule = graph.FindModule<NoiseEx>("Freq Mod");
			mFmFreqKnob = GameObject.Find("knobFMFreq").GetComponent<UIKnob>();
			mFmDepthKnob = GameObject.Find("knobFMDepth").GetComponent<UIKnob>();

			mDistortionModule = graph.FindModule<Distortion>("Distortion");
			mDistortionKnob = GameObject.Find("knobDistortion").GetComponent<UIKnob>();
		}
	
		void Update()
		{
			mFmModule.FreqVal = mFmFreqKnob.Value;
			mFmModule.Min = 1-mFmDepthKnob.Value;
			mFmModule.Max = 1+mFmDepthKnob.Value;

			mDistortionModule.LevelVal = 1-mDistortionKnob.Value;
		}

		public void OnClick_Play()
		{
			mVoiceSampleModule.TriggerNow();
		}
	}
}
