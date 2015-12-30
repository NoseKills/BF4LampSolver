using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LocationToggle : MonoBehaviour {
	void Start () {
		GetComponent<Toggle>().onValueChanged.AddListener(OnChanged);
	}

	private void OnChanged(bool val) {
		if (val) {
			FindObjectOfType<Bf4Solver>().LocationChanged(val);
		}
	}
}
