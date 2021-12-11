using MGB.AudioGraph;
using MGB.AudioGraph.Modules;
using UnityEngine;

public class HelicopterExample : MonoBehaviour
{
	Float mSpeedCtrl;
	Float mLoadCtrl;
	float mSpeedVal;
	float mLoadVal;

	void Start()
	{
		var graph = FindObjectOfType<MonoAudioGraph>();

		mSpeedCtrl = graph.Modules["Ctrl_Speed"] as Float;
		mSpeedVal = mSpeedCtrl.Value;

		mLoadCtrl = graph.Modules["Ctrl_Load"] as Float;
		mLoadVal = mLoadCtrl.Value;
	}

	void Update()
	{
		mSpeedCtrl.Value = mSpeedVal;
		mLoadCtrl.Value = mLoadVal;
	}

	private void OnGUI()
	{
		GUILayout.BeginArea(new Rect(8, 8, 200, 80));
			GUILayout.BeginHorizontal();
				GUILayout.Label("Speed");
				mSpeedVal = GUILayout.HorizontalSlider(mSpeedVal, 0, 1);
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
				GUILayout.Label("Load");
				mLoadVal = GUILayout.HorizontalSlider(mLoadVal, 0, 1);
			GUILayout.EndHorizontal();
			//if (GUILayout.Button("time"))
			//{
			//	Time.timeScale = 0.5f;
			//	GetComponent<AudioSource>().pitch = Time.timeScale;
			//}
		GUILayout.EndArea();
	}
}
