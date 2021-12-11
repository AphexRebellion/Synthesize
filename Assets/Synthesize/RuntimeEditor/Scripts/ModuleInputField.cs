using MGB.AudioGraph.Modules;
using MGB.AudioGraph.RuntimeEditor;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class ModuleInputField : MonoBehaviour
{
	private ModuleBase mModule;
	private FieldInfo mFieldInfo;
	private Text mNameField;
	private InputField mValueField;

	void Awake()
	{
		mNameField = GetComponentInChildren<Text>();
		mValueField = GetComponentInChildren<InputField>();
	}

	public string Name
	{
		get { return mNameField.text; }
		set { mNameField.text = value; }
	}

	public float Value
	{
		get { return float.Parse(mValueField.text); }
		set { mValueField.text = value.ToString(); }
	}

	public Connector Connector { get {return GetComponentInChildren<Connector>(); } }

	public void Init(ModuleBase aModule, PropertyInfo aProp)
	{
		mModule = aModule;
		Name = aProp.Name;

		var fields = aModule.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);

		// Find associated val field if any:
		mFieldInfo = fields.FirstOrDefault(f => f.Name == aProp.Name + "Val" && f.FieldType == typeof(float));
		if (mFieldInfo != null)
			Value = (float)mFieldInfo.GetValue(aModule);
		else
			ShowValField(false);

		bool isConnected = aProp.GetValue(aModule, null) != null;
		ShowValField(!isConnected);

		mValueField.onEndEdit.AddListener(OnEndEdit);
	}

	public void Init(ModuleBase aModule, FieldInfo aFieldInfo)
	{
		mModule = aModule;
		Name = aFieldInfo.Name;
		mFieldInfo = aFieldInfo;
		GetComponentInChildren<Connector>().gameObject.SetActive(false);
		Value = (float)aFieldInfo.GetValue(aModule);
		mValueField.onEndEdit.AddListener(OnEndEdit);
	}

	private void OnEndEdit(string aText)
	{
		if (mFieldInfo != null)
			mFieldInfo.SetValue(mModule, float.Parse(aText));
	}

	public void ShowValField(bool aShow)
	{
		mValueField.gameObject.SetActive(aShow);
	}

	public void SetFieldReadonly(bool aSet)
	{
		mValueField.readOnly = aSet;
	}
}
