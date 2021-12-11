using UnityEngine;
using UnityEngine.EventSystems;

namespace MGB.AudioGraph.RuntimeEditor
{
	public class Connector : MonoBehaviour, IPointerDownHandler
	{
		public ModuleGui ModuleGui {get; set; }
		public string FieldName {get; set; }

		private RuntimeEditor mEditor;

		void Start()
		{
			mEditor = FindObjectOfType<RuntimeEditor>();
			ModuleGui = GetComponentInParent<ModuleGui>();
			FieldName = transform.parent.name;
		}

		// Overrides ModuleGui pointer handling.
		void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
		{
			mEditor.StartLink(this);
		}
	}
}
