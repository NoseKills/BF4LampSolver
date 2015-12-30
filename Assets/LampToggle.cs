using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LampToggle : MonoBehaviour {
	void Start () {
		transform.FindChild("Background").FindChild("Checkmark").GetComponent<Image>().color = Color.cyan;
		GetComponent<Toggle>().onValueChanged.AddListener(OnChanged);
	}

	private void OnChanged(bool val) {
		FindObjectOfType<Bf4Solver>().LampsChanged(val);
	}
}
