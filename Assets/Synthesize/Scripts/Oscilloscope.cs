using MGB.AudioGraph.Modules;
using UnityEngine;
using UnityEngine.UI;

namespace MGB.AudioGraph
{
	public class Oscilloscope : MonoBehaviour
	{
		public RawImage UiImage;

		public float Frequency = 440.0f;
		public float XScale = 1.0f;
		public float YScale = 1.0f;
		public ResetMode TrigMode;
		[Tooltip("Level used for rising/falling retrigger")]
		public float TriggerLevel;
		[Tooltip("Min samples before rise/fall retrigger.")]
		public int SampleThreshold = 440;
		public MonoNode Module;

		private Texture2D Texture;
		private int mChannels;
		private float[] mData = new float[64*1024];
		private int mDataIndex;
		private int[] mTexDat;
		private int mSample;

		private void Awake()
		{
			if (UiImage != null)
			{
				Texture = new Texture2D((int)UiImage.GetComponent<RectTransform>().sizeDelta.x, (int)UiImage.GetComponent<RectTransform>().sizeDelta.y);
				UiImage.texture = Texture;
			}
			else
			{
				Texture = new Texture2D(800, 256);
			}

			// Set white:
			var fillColorArray =  Texture.GetPixels();
			for(var i = 0; i < fillColorArray.Length; ++i)
				fillColorArray[i] = Color.white;
			Texture.SetPixels( fillColorArray );
			Texture.Apply();

			mTexDat = new int[Texture.width];

			Reset();
		}

		private void OnValidate()
		{
			var graph = GetComponent<MonoAudioGraph>();	// This fn called before Awake!

			if (Module != null)
				graph.SetObservedModule((ModuleBase)Module.Module, mData);
			else
				graph.ClearObservedModule();
		}

		// Observed: 2k chunks, usually called about 4 to 8 times per update call.
		void OnAudioFilterRead(float[] data, int channels)
		{
			mChannels = channels;
			int siz = sizeof(float);
			int count = Mathf.Min(data.Length, mData.Length-mDataIndex-1);
			if (count > 0)
			{
				if (Module == null)
					System.Buffer.BlockCopy(data, 0, mData, mDataIndex*siz, count*siz);
				mDataIndex += count;
			}
		}

		private void OnGUI()
		{
			if (Texture != null && UiImage == null)
				GUI.DrawTexture(new Rect(8, 8, Texture.width, Texture.height), Texture);
		}

		void Update()
		{
			if (mDataIndex > 0)	// We got data...
			{
				float lastVal = 0;
				int halfHt = Texture.height/2;
				//float dsIndex = 0; // << todo: use me for xScale
				for (int d=0 ; d<mDataIndex-3; d+=mChannels)
				{
					if (mSample < mTexDat.Length)
						Texture.SetPixel(mSample, mTexDat[mSample], Color.white);	// Erase old val.
					Texture.SetPixel(mSample, halfHt, Color.grey);  // Draw zero y axis.

					float val = mData[d];
					int y = halfHt + (int)(val*YScale*halfHt);
					if (mSample < Texture.width)
					{
						mTexDat[mSample] = y;
						Texture.SetPixel(mSample, y, Color.blue);
					}

					switch (TrigMode)
					{
						case ResetMode.End:
							if (mSample >= Texture.width)
								Reset();
							break;

						case ResetMode.Freq:
							if (mSample >= AudioSettings.outputSampleRate/Frequency)
								Reset();
							break;

						case ResetMode.Rising:
							if (mSample > SampleThreshold)
							{
								if (val >= TriggerLevel && lastVal < TriggerLevel)
									Reset();
							}
							break;

						case ResetMode.Falling:
							if (mSample > SampleThreshold)
							{
								if (lastVal > TriggerLevel && val <= TriggerLevel)
									Reset();
							}
							break;
					}

					mSample++;
					lastVal = val;
				}
				Texture.Apply();
				mDataIndex = 0;
			}
		}

		private void Reset()
		{
			// Erase rest of prev signal:
			int halfHt = Texture.height/2;
			for (int s = mSample; s < Texture.width; s++)
				Texture.SetPixel(s, mTexDat[s], Color.white);

			mSample = 0;
		}
	}

	public enum ResetMode
	{
		End,		// Resets after reaches display end.
		Freq,		// Resets according to freq val.
		Rising,		// Reset on rising above trigger level.
		Falling,	// Reset on falling below trigger level.
	}
}
