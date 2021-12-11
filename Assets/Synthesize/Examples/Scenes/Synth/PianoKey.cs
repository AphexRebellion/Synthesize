using UnityEngine;
using UnityEngine.EventSystems;

namespace MGB.AudioGraph.Examples
{
	public class PianoKey : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
	{
		public int KeyCode;

		public KeyboardLayer Keyboard {get; set; }

		void Start()
		{
			Keyboard = FindObjectOfType<KeyboardLayer>();
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			Keyboard.OnKeyDown(KeyCode);
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			Keyboard.OnKeyUp(KeyCode);
		}
	}
}
