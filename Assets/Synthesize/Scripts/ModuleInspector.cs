using MGB.AudioGraph.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;

namespace MGB.AudioGraph
{
	[CustomEditor(typeof(MonoNode), true)]
	public class ModuleEditor : Editor
	{
		IEnumerable<Type> mModuleTypes;

		public ModuleEditor()
		{
			mModuleTypes = Assembly.GetExecutingAssembly().GetTypes().Where(t => !t.IsAbstract && t.IsSubclassOf(typeof(ModuleBase)));
		}

		public override void OnInspectorGUI()
		{
			var mod = ((MonoNode)target).Module;
			var method = typeof(Util).GetMethod("ShowFields");
			foreach (var t in mModuleTypes)
			{
				if (mod.GetType() == t)
				{
					var genMethod = method.MakeGenericMethod(t);
					genMethod.Invoke(this, new[] { mod });
				}
			}
		}
	}
}
