using MGB.AudioGraph.Modules;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace MGB.AudioGraph
{
	public static class Util
	{
		[AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
		public sealed class AudioCallAttribute : PropertyAttribute { }

		public static IEnumerable<PropertyInfo> GetInputProps(ModuleBase aModule)
		{
			return aModule.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(p => p.PropertyType == typeof(ModuleOutputBinder));
		}

		public static void ShowFields<T>(T aObj) where T : class
		{
			var fields = aObj.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
			foreach (var field in fields)
			{
				DrawField(aObj, field);
			}

			var voidMethods = aObj.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance).Where(m => m.GetParameters().Length == 0);
			foreach (var vm in voidMethods)
			{
				var aca = vm.GetCustomAttributes(typeof(AudioCallAttribute), true);
				if (aca.Any())
				{
					if (GUILayout.Button(vm.Name))
						vm.Invoke(aObj, null);
				}
			}
		}

		// Draws editor gui for given field info, respecting Range attribs.
		public static void DrawField<T>(T aObj, FieldInfo aField) where T : class
		{
			RangeAttribute rngAtt = null;
			var rng = aField.GetCustomAttributes(typeof(RangeAttribute), true);
			if (rng.Length > 0)
				rngAtt = ((RangeAttribute)rng[0]);

			if (aField.FieldType == typeof(float))
			{
				if (rngAtt != null)
					aField.SetValue(aObj, EditorGUILayout.Slider(aField.Name, (float)aField.GetValue(aObj), rngAtt.min, rngAtt.max));
				else
					aField.SetValue(aObj, EditorGUILayout.FloatField(aField.Name, (float)aField.GetValue(aObj)));
			}
			else if (aField.FieldType == typeof(bool))
			{
				aField.SetValue(aObj, EditorGUILayout.Toggle(aField.Name, (bool)aField.GetValue(aObj)));
			}
			else if (aField.FieldType == typeof(int))
			{
				if (rngAtt != null)
					aField.SetValue(aObj, EditorGUILayout.IntSlider(aField.Name, (int)aField.GetValue(aObj), (int)rngAtt.min, (int)rngAtt.max));
				else
					aField.SetValue(aObj, EditorGUILayout.IntField(aField.Name, (int)aField.GetValue(aObj)));
			}
			else if (aField.FieldType == typeof(AnimationCurve))
			{
				aField.SetValue(aObj, EditorGUILayout.CurveField(aField.Name, (AnimationCurve)aField.GetValue(aObj)));
			}
			else if (aField.FieldType.IsSubclassOf(typeof(Enum)))
			{
				aField.SetValue(aObj, EditorGUILayout.EnumPopup(label: aField.Name, selected: (Enum)aField.GetValue(aObj)));
			}
		}

		public static void SaveWav(float[] aData, int aChannels, int aSampleRate, string aFilename, bool aNormalise)
		{
			int numsamples = aData.Length / aChannels;
			int bytesPerSample = 2; // in bytes

			float normMul = 1.0f;
			if (aNormalise)
			{
				float min=0, max=0;
				for (int i=0 ; i<aData.Length ; i++)
				{
					float d = aData[i];
					if (d < min)
						min = d;
					else if (d > max)
						max = d;
				}
				normMul = 1/(Mathf.Max(Mathf.Abs(max), Mathf.Abs(min)));
			}

			using (var f = new FileStream(aFilename, FileMode.Create))
			{
				BinaryWriter wr = new BinaryWriter(f);

				wr.Write("RIFF".ToArray());
				wr.Write(36 + numsamples * aChannels * bytesPerSample);	// file length-8
				wr.Write("WAVEfmt ".ToArray());
				wr.Write(16);					// SubChunk1 length.
				wr.Write((ushort)1);			// Format: 1 = uncompressed PCM
				wr.Write((ushort)aChannels);
				wr.Write((uint)aSampleRate);
				wr.Write((uint)(aSampleRate * aChannels * bytesPerSample));	// byte rate
				wr.Write((ushort)(aChannels * bytesPerSample));		// block align
				wr.Write((ushort)(8 * bytesPerSample));	// bits per sample
				wr.Write("data".ToArray());
				wr.Write(numsamples * aChannels * bytesPerSample);	// SubChunk2 length

				for (int s=0 ; s<aData.Length ; s++)
				{
					float d = aData[s];
					if (aNormalise)
						d *= normMul;
					else
						d = Mathf.Clamp(d, -1, 1);
					wr.Write((short)(short.MaxValue * d));
				}
			}
		}
	}

	public static class ExtensionMethods
	{
		public static float ClampLower(this float aVal, float aLow) { return Mathf.Max(aVal, aLow); }   
		public static float ClampUpper(this float aVal, float aUpper) { return Mathf.Min(aVal, aUpper); }   
		public static float Clamp01(this float aVal) { return Mathf.Clamp01(aVal); }
		public static float Clamp(this float aVal, float aUpper, float aLower) { return Mathf.Clamp(aVal, aUpper, aLower); }
	}
}
