//#define BENCHMARK
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MGB.AudioGraph.Modules;
using MGB.AudioGraph.Nodes;
using XNode;

namespace MGB.AudioGraph
{
	[RequireComponent(typeof(AudioSource))]
	public class MonoAudioGraph : MonoBehaviour
	{
		public AudioNodeGraph GraphAsset;
		public bool Spatialised;

		#if UNITY_EDITOR
		private ModuleBase ObservedModule;
		private float[] ObservedModuleBuffer;
		private int mObserverModuleBuffIndex;
		#endif

		public Dictionary<string, ModuleBase> Modules {get; set; }

		private ModuleOutputBinder mOutputLeft;
		private ModuleOutputBinder mOutputRight;
		private Dictionary<AudioNode, ModuleBase> mNodeModules;
		private List<ModuleBase> TickModules { get; set; }
		private bool mValidGraph;

		#if BENCHMARK
		int mBmCount;
		#endif

		void Awake()
		{
			Debug.Assert(GraphAsset != null, "NULL graph asset in " + name);
			Debug.Assert(GraphAsset.nodes.Count > 0, "Graph asset has no nodes: " + name);

			var outNodes = GraphAsset.nodes.Where(n => n is OutputNode);
			Debug.Assert(outNodes.Count() == 1, "Need a single MasterOutput node in graph!");
			AudioNode outputNode = outNodes.FirstOrDefault() as AudioNode;
			if (outputNode == null)
			{
				Debug.LogError("No output node found in graph: " + GraphAsset.name);
				return;
			}

			ModuleBase.SampleRate = AudioSettings.outputSampleRate;

			var outputModule = (Output)outputNode.CreateModule();
			outputModule.Name = outputNode.name;
			var nodePos = outputNode.position;
			nodePos.y *= -1;
			outputModule.Pos = nodePos;
			GameObject obj = null;
			#if UNITY_EDITOR
			obj = CreateGameObj(gameObject, outputNode.name, outputModule);
			#endif
			mNodeModules = new Dictionary<AudioNode, ModuleBase> { { outputNode, outputModule } };
			TickModules = new List<ModuleBase>(4) { outputModule };
			BuildGraph(outputModule, outputNode, obj);
			TickModules.Reverse();  // Ensure correct tick order: leaf-first.

			mValidGraph = true;
			foreach (var kv in mNodeModules)
			{
				kv.Value.Initialise();
				if (!kv.Value.Validate())
				{
					Debug.LogError("Module validate failed: " + kv.Key.name);
					mValidGraph = false;
				}
			}

			// Setup dict for module runtime access:
			Modules = new Dictionary<string, ModuleBase>(4);
			foreach (var kv in mNodeModules)
			{
				string key = kv.Key.name;
				if (Modules.ContainsKey(key))
					Debug.LogWarning("Nodes need unique names for runtime access.  Duplicate name: " + key);
				Modules[kv.Key.name] = kv.Value;
			}
			mNodeModules = null;

			mOutputLeft = outputModule.OutputBinder("Left");
			mOutputRight = outputModule.OutputBinder("Right");

			// Unload graph asset: todo: destroy runtime obj? Check we dont delete saved asset!
			//GraphAsset = null;

			var source = GetComponent<AudioSource>();
			source.loop = true;
			source.spatialBlend = Spatialised? 1.0f : 0.0f;
		}

		public ModuleBase FindModule(string aName)
		{
			Debug.Assert(Modules.ContainsKey(aName), "Module name '" + aName + "' not found in graph. Note: nodes not connected to output will not be created at runtime.");
			return Modules[aName];
		}

		public T FindModule<T>(string aName) where T : ModuleBase
		{
			ModuleBase mb = FindModule(aName);
			Debug.AssertFormat(mb is T, "Module {0} is type {1}, not {2}", aName, mb.GetType(), typeof(T));
			return (T)mb;
		}

		/// <summary>
		/// Recursively builds a runtime module graph from the node graph.
		/// </summary>
		/// <param name="aModule">Module for given graph node.</param>
		/// <param name="aNode">Current graph node</param>
		/// <param name="aObj">GameObject representing this graph node, or null to not build GameObject hierarchy.</param>
		private void BuildGraph(ModuleBase aModule, Node aNode, GameObject aObj)
		{
			var inputModules = new Dictionary<ModuleBase, AudioNode>(2);
			foreach (var input in aNode.Inputs)
			{
				for (int c=0 ; c< input.ConnectionCount ; c++)
				{
					var inputNode = input.GetConnection(c).node as AudioNode;
					
					ModuleBase inputModule = null;
					if (!mNodeModules.TryGetValue(inputNode, out inputModule))
					{
						inputModule = inputNode.CreateModule();
						inputModule.Name = inputNode.name;
						var nodePos = inputNode.position;
						nodePos.y *= -1;
						inputModule.Pos = nodePos;
						mNodeModules[inputNode] = inputModule;

						TickModules.Add(inputModule);
					}
					
					var binder = inputModule.OutputBinder(input.GetConnection(c).fieldName);
					MapInput(aModule, input.fieldName, binder);

					if (!inputModules.ContainsKey(inputModule))
						inputModules[inputModule] = inputNode;
				}
			}

			foreach (var kv in inputModules)
			{
				ModuleBase mod = kv.Key;
				AudioNode node = kv.Value;
				GameObject obj = null;
				if (aObj != null)
					obj = CreateGameObj(aObj, node.name, mod);
				BuildGraph(mod, node, obj);
			}
		}

		// Bind input to module property if one exists with same name, else add to generic inputs list.
		private void MapInput(ModuleBase aModule, string aInputName, ModuleOutputBinder aInputBinder)
		{
			var prop = Util.GetInputProps(aModule).FirstOrDefault(p => p.Name == aInputName);
			if (prop != null)
				prop.SetValue(aModule, aInputBinder, null);
			else
				aModule.GenericInputs.Add(aInputBinder);
		}

		/// <summary>
		/// Creates an in-game object for a graph node so we can change values in the inspector.
		/// </summary>
		/// <param name="aParent"></param>
		/// <param name="aName"></param>
		/// <param name="aModule"></param>
		/// <returns></returns>
		private static GameObject CreateGameObj(GameObject aParent, string aName, ModuleBase aModule)
		{
			var obj = new GameObject(aName);
			obj.transform.SetParent(aParent.transform);
			obj.AddComponent<MonoNode>().Module = aModule;
			return obj;
		}

		/// <summary>
		/// Callback from Unity (not on the main game thread) to provide audio data.
		/// </summary>
		/// <param name="data"></param>
		/// <param name="channels"></param>
		void OnAudioFilterRead(float[] data, int channels)
		{
			if (!mValidGraph)
				return;

			#if BENCHMARK
			var sw = new System.Diagnostics.Stopwatch();
			sw.Start();
			#endif

			int numSamples = data.Length/2;
			int n=0;
			int i=0;
			while (n < numSamples)
			{
				float left = mOutputLeft.GetValue();
				float right = mOutputRight.GetValue();

				// Write to channels (interleaved):
				// Note: to get positional audio we need to loop a (value 1.0) sample on this AudioSource and mul our vals with it here!
				// See: https://forum.unity.com/threads/onaudiofilterread-sound-spatialisation.362782/
				data[i++] *= left;
				if (channels > 1)
					data[i++] *= right;

				#if UNITY_EDITOR
				if (ObservedModule != null && ObservedModuleBuffer != null)
					CaptureObservedModuleOutput();
				#endif

				for (int m = 0; m < TickModules.Count; m++)
				{
					TickModules[m].Tick();
				}
				n++;
			}

			#if BENCHMARK
			sw.Stop();
			if (++mBmCount == 500)
			{
				Debug.Log("Benchmark ticks: " + sw.ElapsedTicks/mBmCount);
				mBmCount = 0;
			}
			#endif
		}

		#if UNITY_EDITOR
		internal void SetObservedModule(ModuleBase aModule, float[] aBuffer)
		{
			ObservedModule = aModule;
			ObservedModuleBuffer = aBuffer;
			mObserverModuleBuffIndex = 0;
		}

		internal void ClearObservedModule()
		{
			ObservedModule = null;
			ObservedModuleBuffer = null;
		}

		private void CaptureObservedModuleOutput()
		{
			if (++mObserverModuleBuffIndex >= ObservedModuleBuffer.Length)
				mObserverModuleBuffIndex = 0;
			ObservedModuleBuffer[mObserverModuleBuffIndex] = ObservedModule.GetOutput();
		}
		#endif
	}
}
