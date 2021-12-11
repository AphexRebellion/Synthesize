using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MGB.AudioGraph.Examples
{
	public class UIKnob : MonoBehaviour, IPointerDownHandler
	{
		public float MinVal = 0.0f;
		public float MaxVal = 1.0f;
		public bool ShowValueText = true;
		public string ValueFormat = "0.00";
		public Image DialImage;
		public Text ValueText;

		private const int KMaxValDragDistPixels = 100;

		void Start()
		{
			ValueText.gameObject.SetActive(ShowValueText);
			UpdateVisuals();
		}
	
		void Update()
		{
			if (mDragging && Input.GetMouseButtonUp(0))
			{
				mDragging = false;
			}

			if (mDragging)
			{
				float delta = Input.mousePosition.y - mMouseDragOrigin.y;
				delta = Mathf.Clamp(delta, -KMaxValDragDistPixels, KMaxValDragDistPixels);
				float valRange = MaxVal - MinVal;
				Value = mDragStartVal + delta * (valRange/(float)KMaxValDragDistPixels);
			}
		}

		private float mValue;
		public float Value
		{
			get { return mValue; }

			set
			{
				mValue = Mathf.Clamp(value, MinVal, MaxVal);
				UpdateVisuals();
			}
		}

		bool mDragging;
		private float mDragStartVal;
		Vector3 mMouseDragOrigin;
		public void OnPointerDown(PointerEventData eventData) 
		{
			mDragging = true;
			mMouseDragOrigin = Input.mousePosition;
			mDragStartVal = Value;
		}

		private void UpdateVisuals()
		{
			DialImage.fillAmount = (Value-MinVal)/(MaxVal-MinVal);

			if (ShowValueText)
				ValueText.text = Value.ToString(ValueFormat);
		}
	}
}
