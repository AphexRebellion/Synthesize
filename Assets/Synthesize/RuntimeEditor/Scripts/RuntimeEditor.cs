using MGB.AudioGraph.Modules;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MGB.AudioGraph.RuntimeEditor
{
	// todo: Drop endlink on editor = show create module menu?

	public class RuntimeEditor : MonoBehaviour
	{
		private ModuleLink mLink;

		private GameObject mModulePanel;
		private MonoAudioGraph mGraph;
		private List<ModuleGui> mModules = new List<ModuleGui>(8);
		private List<ModuleLink> mLinks = new List<ModuleLink>(4);
		private Connector mStartConnector;

		void Start()
		{
			mModulePanel = transform.Find("EditorPanel/panelModules").gameObject;

			mLink = ((GameObject)Instantiate(Resources.Load("Link"))).GetComponent<ModuleLink>();
			mLink.transform.SetParent(transform);
			mLink.gameObject.SetActive(false);
		}

		public void EditGraph(MonoAudioGraph aGraph)
		{
			Clear();

			Vector3 avgPos = Vector3.zero;
			mGraph = aGraph;
			mModules = new List<ModuleGui>(8);
			foreach (var mod in aGraph.Modules.Values)
			{
				var mg = CreateModuleGui(mod, mModulePanel.transform);
				avgPos += mg.transform.position;
				mModules.Add(mg);
			}
			
			avgPos /= mModules.Count;
			// dunt werk?! mModulePanel.transform.position = avgPos;

			// Create links:
			foreach (var mg in mModules)
			{
				var props = Util.GetInputProps(mg.Module);
				foreach (var inf in mg.InputFields)
				{
					if (inf.Name == "GenericInputs")
					{
						foreach (var gi in mg.Module.GenericInputs)
						{
							AddFieldLink(inf, gi);
						}
					}
					else
					{
						var prop = props.FirstOrDefault(p => p.Name == inf.Name);
						var binder = (ModuleOutputBinder)prop.GetValue(mg.Module, null);
						if (binder != null)
						{
							AddFieldLink(inf, binder);
						}
					}
				}
			}

			mLink.transform.SetAsLastSibling();
		}

		private void AddFieldLink(ModuleInputField inf, ModuleOutputBinder binder)
		{
			Connector endConn = inf.Connector;
			var inMod = binder.Module;
			string opName = inMod.OutputNames[binder.OutputIndex];
			ModuleGui mgi = mModules.FirstOrDefault(m => m.Module == inMod);
			Connector startConn = mgi.OutputConnectors[opName];
			CreateLink(startConn, endConn);
		}

		private void Clear()
		{
			mGraph = null;
			mModulePanel.transform.position = Vector3.zero;
			mScale = 1;

			foreach (var m in mModules)
				Destroy(m.gameObject);
			
			foreach (var l in mLinks)
				Destroy(l.gameObject);

			mModules.Clear();
			mLinks.Clear();
		}

		private ModuleGui CreateModuleGui(ModuleBase aModule, Transform aParent)
		{
			var mgr = Resources.Load("Module");
			var mg = (GameObject)Instantiate(mgr);
			var gui = mg.GetComponent<ModuleGui>();
			gui.SetModule(aModule);
			mg.name = aModule.Name;
			mg.transform.SetParent(aParent, false);
			mg.transform.position = aModule.Pos;
			return gui;
		}

		float mScale = 1.0f;
		Vector3 mLastMousePos;
		void Update()
		{
			if (mStartConnector != null)
			{
				mLink.SetEndPos(Input.mousePosition);

				if (Input.GetMouseButtonUp(0))
				{
					var conn = GetGuiComponentUnderMouse<Connector>();
					if (conn != null)
					{
						EndLink(conn);
					}
					else
					{
						mStartConnector = null;
						mLink.StartConnector = null;
						mLink.gameObject.SetActive(false);
					}
				}
			}

			if (Input.GetMouseButton(2))
			{
				mModulePanel.transform.position += Input.mousePosition - mLastMousePos;
			}
			mLastMousePos = Input.mousePosition;

			mScale += Input.mouseScrollDelta.y * 0.1f;
			mScale = Mathf.Clamp(mScale, 0.5f, 2.0f);
			mModulePanel.transform.localScale = Vector3.one * mScale;
		}

		private T GetGuiComponentUnderMouse<T>() where T : Component
		{
			PointerEventData pointerData = new PointerEventData (EventSystem.current) { position = Input.mousePosition };
			var list = new List<RaycastResult>();
			EventSystem.current.RaycastAll(pointerData, list);
			foreach (var c in list)
			{
				if (c.gameObject.GetComponent<T>() != null)
					return c.gameObject.GetComponent<T>();
			}
			return null;
		}

		public void StartLink(Connector aStart)
		{
			Debug.Assert(mModules.Contains(aStart.ModuleGui), "Start module not found!");
			mStartConnector = aStart;
			mLink.SetStartConnector(aStart);
			mLink.SetEndPos(Input.mousePosition);
			mLink.gameObject.SetActive(true);
		}

		public void EndLink(Connector aEndConnector)
		{
			Debug.Assert(mStartConnector != null , "Null Start link!");

			if (mStartConnector != aEndConnector)
			{
				Debug.Assert(mModules.Contains(aEndConnector.ModuleGui), "End module not found!");
				CreateLink(mStartConnector, aEndConnector);
				mLink.gameObject.SetActive(false);
			}

			mStartConnector = null;
		}

		public void CreateLink(Connector aStart, Connector aEnd)
		{
			var link = Instantiate(mLink.gameObject).GetComponent<ModuleLink>();
			link.transform.SetParent(transform);
			link.StartConnector = aStart;
			link.EndConnector = aEnd;
			link.gameObject.SetActive(true);
			mLinks.Add(link);
		}
	}
}
