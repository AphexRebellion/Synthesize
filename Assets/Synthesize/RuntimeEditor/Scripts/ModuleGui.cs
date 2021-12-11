using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MGB.AudioGraph.Modules;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MGB.AudioGraph.RuntimeEditor
{
	// GUI representing a module graph node.
	public class ModuleGui : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
	{
		public ModuleBase Module;

		public Dictionary<string, Connector> OutputConnectors {get; private set; }
		public List<ModuleInputField> InputFields {get; private set; }

		// Use this for initialization
		void Start ()
		{
			SetRoutingMode(Module.RoutingMode);
			SetChildText("Header/textName", Module.Name);
		}

		void Update ()
		{
			if (mDragging)
			{
				transform.position = Input.mousePosition - mDragOffset;
			}
		}

		private void SetRoutingMode(RoutingMode aMode)
		{
			Module.RoutingMode = aMode;
			SetChildText("Header/buttonRoutingMode/textRoutingMode", aMode.ToString().First().ToString());
		}

		private void SetChildText(string aObjName, string aText)
		{
			transform.Find(aObjName).GetComponent<Text>().text = aText;
		}

		bool mDragging;
		Vector3 mDragOffset;
		public void OnPointerDown(PointerEventData eventData)
		{
			mDragging = true;
			mDragOffset = Input.mousePosition-transform.position;
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			mDragging = false;
		}

		public void OnClickRoutingMode()
		{
			int next = (int)Module.RoutingMode + 1;
			if (next >= System.Enum.GetValues(typeof(RoutingMode)).Length)
				next = 0;
			SetRoutingMode((RoutingMode)next);
		}

		public void SetModule(ModuleBase aModule)
		{
			var parent = transform.Find("Controls");
			Module = aModule;

			InputFields = new List<ModuleInputField>(4);

			// Add inputs:
			var props = aModule.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
			foreach (var prop in props)
			{
				var ipa = prop.GetCustomAttributes(typeof(ModuleInputAttribute), true);
				if (ipa.Any())
				{
					var obj = (GameObject)Instantiate(Resources.Load("InputField"));
					obj.name = prop.Name;
					var inField = obj.GetComponent<ModuleInputField>();
					inField.Init(aModule, prop);
					obj.transform.SetParent(parent.transform);
					InputFields.Add(inField);
				}
			}

			// Add fields:
			var fields = aModule.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public).Where(f => !f.Name.EndsWith("Val"));
			foreach (var field in fields)
			{
				var attr = field.GetCustomAttributes(typeof(ModuleFieldAttribute), true);
				if (attr.Any())
				{
					var obj = (GameObject)Instantiate(Resources.Load("InputField"));
					obj.name = field.Name;
					var inField = obj.GetComponent<ModuleInputField>();
					inField.Init(aModule, field);
					obj.transform.SetParent(parent.transform);
					InputFields.Add(inField);
				}
			}

			// Add outputs:
			if (!(aModule is Output))
			{
				OutputConnectors = new Dictionary<string, Connector>(4);
				foreach (var name in aModule.OutputNames)
				{
					var obj = (GameObject)Instantiate(Resources.Load("OutputField"));
					obj.transform.Find("textFieldName").GetComponent<Text>().text = name;
					obj.name = name;
					obj.transform.SetParent(parent.transform);
					OutputConnectors[name] = obj.GetComponentInChildren<Connector>();
				}
			}
			//fitter doesnt work?! parent.GetComponent<RectTransform>().sizeDelta = new Vector2(200, 8 * 20);
		}
	}
}
