using System.Collections.Generic;
using UnityEngine;
using AudioGraph.Filters;
using System;

// Modules generate the audio data at runtime, reflecting node graph structure.
//
// Notes:
//
// - For each sample, GetOutput is called (possibly multiple times between ticks) on the graph, then Tick is called.
//   Change of internal state and value calcs are done in the Tick method.  Set OutputValues here for each output.
//   Map value output indices to names in GetOutputIndex().
//
// - Module inputs are auto-bound to module properties of the node input field name, via a ModuleOutputBinder in MonoAudioGraph.MapInput()
//   So any module inputs should have a ModuleOutputBinder property of the same name as the node input field.
//

namespace MGB.AudioGraph.Modules
{
	public enum WaveType
	{
		Sine,
		Square,
		Pulse,
		Saw,
		Triangle,
	}

	public enum Polarity
	{
		Bipolar,
		Positive,
		Negative
	}

	public enum RetriggerType
	{
		Interrupt,	// Triggering will always restart.
		OneShot,	// Will only restart after finished.
	}

	public enum FilterMode
	{
		LowPass,
		HighPass,
		BandPass
	}

	public enum RoutingMode
	{
		Active,			// Normal active state.
		Bypass,			// Signal bypasses node.
		Disabled,		// Graph evaluation stops here with a zero output. Internal state stil updated in Tick.
		Frozen,			// As Disabled, but internal state not updated in Tick.
	}

	/// <summary>
	/// Base runtime graph node.
	/// </summary>
	[System.Serializable]
	public abstract class ModuleBase
	{
		[ModuleInput]
		public List<ModuleOutputBinder> GenericInputs {get; set; }	// Generic (non-named) inputs.
		[System.NonSerialized]
		protected float[] OutputValues;					// Cached in Tick fn for each output.

		// Gui data:
		public string Name {get; set; }
		public Vector3 Pos {get; set; }
		// End Gui data

		public string[] OutputNames {get; private set;}

		public static int SampleRate {get; set;}

		public const float KLevelSmoothDelta = 0.002f;	// For amp level change smoothing.

		public RoutingMode RoutingMode;

		public ModuleBase(string[] aOutputNames = null)
		{
			GenericInputs = new List<ModuleOutputBinder>(2);
			OutputNames = aOutputNames != null ? aOutputNames : new[] {"Output"};
			OutputValues = new float[OutputNames.Length];
		}

		/// <summary>
		/// Returns a binding adapter for the given output name.
		/// </summary>
		/// <param name="aOutputName">Name of output to bind to.  See GetOutputIndex()</param>
		/// <returns></returns>
		public ModuleOutputBinder OutputBinder(string aOutputName)
		{
			return new ModuleOutputBinder(this, aOutputName);
		}

		// Helper to get input binder val if not null, else returns default val.
		public static float GetInputVal(ModuleOutputBinder aBinder, float aDefaultVal)
		{
			if (aBinder != null)
				return aBinder.GetValue();
			return aDefaultVal;
		}

		/// <summary>
		/// Perform any module initialisation here.
		/// Called after runtime graph has been created and module inputs bound, before Validate is called and graph is run.
		/// </summary>
		public virtual void Initialise()
		{
		}

		/// <summary>
		/// Checks if this module is in a valid state.
		/// Called after runtime graph is created and wired up and Initialise called.
		/// </summary>
		/// <returns>Whether valid.</returns>
		public virtual bool Validate()
		{
			return true;
		}

		/// <summary>
		/// Looks up the output index for the given output name.
		/// </summary>
		/// <param name="aName">Name of output to return index of.</param>
		/// <returns>Index of output corresponding to given name.</returns>
		public int GetOutputIndex(string aName)
		{
			for (int n=0 ; n<OutputNames.Length ; n++)
			{
				if (aName == OutputNames[n])
					return n;
			}
			return -1;
		}

		/// <summary>
		/// Returns the signal value for the given output index.
		/// </summary>
		/// <param name="aOutputIndex"></param>
		/// <returns>The signal value.</returns>
		public float GetOutput(int aOutputIndex = 0)
		{
			return OutputValues[aOutputIndex];
		}

		/// <summary>
		/// All processing calculations for an audio sample are carried out here and cached in OutputValues for each output.
		/// </summary>
		public abstract void Tick();

		/// <summary>
		/// Sums all non-named inputs.
		/// </summary>
		/// <returns>The inputs sum.</returns>
		protected float SumGenericInputs()
		{
			float val = 0.0f;
			for (int i=0 ; i<GenericInputs.Count ; i++)
			{
				val += GenericInputs[i].GetValue();
			}
			return val;
		}

		/// <summary>
		/// Basic routing check template.
		/// Checks default routing when no action is required for Disabled mode (i.e. the Tick fn has no internal state to progress).
		/// </summary>
		/// <returns>Whether to carry out further processing.</returns>
		protected bool CheckRouting()
		{
			if (RoutingMode == RoutingMode.Frozen || RoutingMode == RoutingMode.Disabled)
			{
				OutputValues[0] = 0;
				return false;
			}
			else if (RoutingMode == RoutingMode.Bypass)
			{
				OutputValues[0] = SumGenericInputs();
				return false;
			}

			return true;
		}

		public void Calc(float[] aOutput, int aChannels)
		{
			for (int i=0; i<aOutput.Length; i+= aChannels)
			{
				Tick();
				aOutput[i] = OutputValues[0];
				if (aChannels > 1)
					aOutput[i + 1] = OutputValues[0];
			}
		}
	}

	// Adapter for binding to a specific module output.
	public class ModuleOutputBinder
	{
		public ModuleOutputBinder(ModuleBase aModule, string aOutputName)
		{
			Module = aModule;
			OutputIndex = aModule.GetOutputIndex(aOutputName);
		}

		public ModuleBase Module {get; private set; }
		public int OutputIndex {get; private set; }

		public float GetValue()
		{
			return Module.GetOutput(OutputIndex);
		}
	}


	[System.Serializable]
	public class AudioModuleEvent : UnityEngine.Events.UnityEvent<ModuleBase>
	{
	}


	[System.Serializable]
	public abstract class GeneratorModule : ModuleBase
	{
		public GeneratorModule(float aFreq, float aAmp, float aFreqMod) : base()
		{
			FreqVal = aFreq;
			AmpVal = aAmp;
			FreqModVal = aFreqMod;
		}

		public float FreqVal;
		public float AmpVal;
		public float FreqModVal;

		[ModuleInput]
		public ModuleOutputBinder Amp {get; set; }
		[ModuleInput]
		public ModuleOutputBinder Freq {get; set; }
		[ModuleInput]
		public ModuleOutputBinder FreqMod {get; set; }
	}

	[AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
	public sealed class ModuleInputAttribute : PropertyAttribute { }

	[AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
	public sealed class ModuleFieldAttribute : PropertyAttribute { }

	public static class SoundUtils
	{
		public static float WetDryCombine(float aDry, float aWet, float aMix)
		{
			Debug.Assert(aMix >= 0 && aMix <=1, "WetDry combine: mix not 0..1");
			return Mathf.Lerp(aDry, aWet, aMix); // todo: may want 0dB-preserving mix?  See: https://www.kvraudio.com/forum/viewtopic.php?f=6&t=426306
		}
	}
}
