using UnityEngine;

namespace MGB.AudioGraph.RuntimeEditor
{
	public class ModuleLink : MonoBehaviour
	{
		public Connector StartConnector {get; set; }
		public Connector EndConnector {get; set; }

		private RectTransform mTrans;

		// Use this for initialization
		void Start ()
		{
			mTrans = GetComponent<RectTransform>();	// Goes null -- why?!
		}

		public void SetStartConnector(Connector aConn)
		{
			StartConnector = aConn;
			mTrans = GetComponent<RectTransform>();
			mTrans.position = StartConnector.transform.position;
		}

		// Update is called once per frame
		void Update ()
		{
			mTrans = GetComponent<RectTransform>();
			mTrans.position = StartConnector.transform.position;
			if (EndConnector != null)
				SetEndPos(EndConnector.transform.position);
		}

		public void SetEndPos(Vector3 aEndPos)
		{
			mTrans = GetComponent<RectTransform>();
			mTrans.sizeDelta = new Vector2(Vector2.Distance(mTrans.position, aEndPos), mTrans.sizeDelta.y);
			var delta = aEndPos - mTrans.position;
			mTrans.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan2(delta.y, delta.x));
		}
	}
}
