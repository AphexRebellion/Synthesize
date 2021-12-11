using MGB.AudioGraph.Modules;
using UnityEngine;

namespace MGB.AudioGraph
{
	public class WaveViewer : MonoBehaviour
	{
		public int BufferSizeKb = 4;
		[Range(0.1f, 3.0f)]
		public float YScale = 0.95f;
		public MonoNode Module;

		private MonoAudioGraph mGraph;
		private Texture2D Texture;
		private float[] mData;
		private int mIndex;
		private bool mDrawSpec;

		private int BufferSize {get {return BufferSizeKb*1024; } }

		private void OnValidate()
		{
			mGraph = GetComponent<MonoAudioGraph>();	// This fn called before Awake!

			BufferSizeKb = Mathf.Max(BufferSizeKb, 1);
			if (mData == null || mData.Length != BufferSize && BufferSize > 0)
				mData = new float[BufferSize];

			mIndex = BufferSize+1;

			if (Module != null)
				mGraph.SetObservedModule((ModuleBase)Module.Module, mData);
			else
				mGraph.ClearObservedModule();
		}

		void OnAudioFilterRead(float[] data, int channels)
		{
			if (mData == null)	// Sometimes null. Multithread problem? Need lock?
				return;

			int siz = sizeof(float);
			if (mIndex < BufferSize)
			{
				int count = Mathf.Min(data.Length, mData.Length-mIndex);
				if (count > 0)
				{
					if (Module == null)
						System.Buffer.BlockCopy(data, 0, mData, mIndex*siz, count*siz);
					mIndex += count;
				}
			}
		}

		void Update()
		{
			if (mDrawSpec)
			{
				var spectrum = new float[128];
				//Texture = new Texture2D(128, 256);
				//GetComponent<AudioSource>().GetSpectrumData(specData, 0, FFTWindow.Hamming);
				for (int i=1 ; i<spectrum.Length-1 ; i++)
				{
				//	float val = specData[x];
				//	int y = (int)(YScale * val * 100);
				//	var clr = Mathf.Abs(val) <= 1.0f ? Color.blue : Color.red;
				//	Texture.SetPixel(x, y, clr);
					//Debug.DrawLine(new Vector3(i - 1, spectrum[i] + 10, 0), new Vector3(i, spectrum[i + 1] + 10, 0), Color.red);
					//Debug.DrawLine(new Vector3(i - 1, Mathf.Log(spectrum[i - 1]) + 10, 2), new Vector3(i, Mathf.Log(spectrum[i]) + 10, 2), Color.cyan);
					//Debug.DrawLine(new Vector3(Mathf.Log(i - 1), spectrum[i - 1] - 10, 1), new Vector3(Mathf.Log(i), spectrum[i] - 10, 1), Color.green);
					//Debug.DrawLine(new Vector3(Mathf.Log(i - 1), Mathf.Log(spectrum[i - 1]), 3), new Vector3(Mathf.Log(i), Mathf.Log(spectrum[i]), 3), Color.blue);
				}
				//Texture.Apply();

				mDrawSpec = false;
			}
			else if (mIndex == BufferSize)
			{
				int halfHt = Texture.height/2;
				float inc = BufferSize/(float)Texture.width;
				for (int x=0 ; x<Texture.width ; x++)
				{
					Texture.SetPixel(x, halfHt, Color.grey);
					Texture.SetPixel(x, halfHt + (int)(YScale*halfHt), Color.yellow);
					Texture.SetPixel(x, halfHt - (int)(YScale*halfHt), Color.yellow);
					int xi = (int)(x*inc);
					float val = mData[xi];
					int y = halfHt + (int)(val*YScale*halfHt);
					var clr = Mathf.Abs(val) <= 1.0f ? Color.blue : Color.red;
					if (y >= 0 && y < Texture.height)
						Texture.SetPixel(x, y, clr);
				}
				Texture.Apply();
				mIndex = BufferSize+1;	// ugh
			}
		}

		string mWavFilename = "Test.wav";
		bool mNormalise;
		private void OnGUI()
		{
			if (UnityEditor.Selection.activeGameObject != gameObject)
				return;

			GUILayout.BeginVertical();
			{
				if (GUILayout.Button("Capture wave"))
				{
					Texture = new Texture2D(800, 256);
					mIndex = 0;
				}

				if (mData != null && mIndex == BufferSize+1)	// ugh
				{
					GUILayout.BeginHorizontal();
					GUILayout.Label("Filename:");
					mWavFilename = GUILayout.TextField(mWavFilename, GUILayout.Width(100));
					mNormalise = GUILayout.Toggle(mNormalise, "Normalise");
					if (GUILayout.Button("Save WAV"))
					{
						string filename = mWavFilename;
						if (!filename.EndsWith(".wav"))
							filename += ".wav";
						Util.SaveWav(mData, 2, AudioSettings.outputSampleRate, filename, false);
					}
					GUILayout.EndHorizontal();
				}

				//if (GUILayout.Button("Spectrum"))
				//{
				//	mDrawSpec = true;
				//}
			}
			GUILayout.EndVertical();

			if (Texture != null)
				GUI.DrawTexture(new Rect(16, 50, Texture.width, Texture.height), Texture);
		}
	}
}
