using UnityEditor;
using MGB.AudioGraph;
using UnityEngine;
using MGB.AudioGraph.RuntimeEditor;

[CustomEditor(typeof(MonoAudioGraph))]
public class MonoAudioGraphEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		var rte = FindObjectOfType<RuntimeEditor>();	// todo: create editor if none found?
		if (rte != null && GUILayout.Button("Edit Graph"))
		{
			rte.EditGraph((MonoAudioGraph)target);
		}
	}
}
