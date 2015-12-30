using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public enum Location {
	StartingSituation = 0,
	Garden = 1,
	Rock = 2,
	Waterfall = 3,
	Pier = 4,
	Furnace = 5,
	Pagoda = 6,
	Tree = 7,
	Currently = 8
}

public class Bf4Solver : MonoBehaviour {
	public Toggle[] Locations;
	public Toggle[] LampButtons;
	public Text SolutionLabel;
	public Text HelpLabel;


	private static readonly int locationCount = Enum.GetNames(typeof(Location)).Length;
	public static readonly int buttonLocationCount = locationCount - 2;

	private Location SelectedLocation = Location.StartingSituation;
	private Dictionary<Location, int> Map;
	private bool initialized;

	private void Start() {
		Map = new Dictionary<Location, int>();
		
		for (var i = 0; i < locationCount; i++) {
			var l = (Location) i;
			Map.Add(l, PlayerPrefs.GetInt(l.ToString(), 0));
		}
		LocationChanged(true);
		initialized = true;
	}

	public void LocationChanged(bool b) {
		var i = 0;
		foreach (var location in Locations) {
			if (location.isOn) {
				SelectedLocation = (Location) i;
				break;
			}
			i++;
		}
		SetLampsFromLocation();
		HelpPressed();
	}

	public void LampsChanged(bool t) {
		SaveLampStatuses();
	}

	public void SolvePressed() {
		SolutionLabel.text = new Bf4SolutionFinder().Solve(Map).ToString();
		SolutionLabel.enabled = true;
		HelpLabel.enabled = false;
	}

	public void HelpPressed() {
		SolutionLabel.enabled = false;
		HelpLabel.enabled = true;
	}

	private void SaveLampStatuses() {
		Map[SelectedLocation] = ButtonsToInt();
		if (!initialized) return;

		foreach (var kvp in Map) {
			//Debug.Log(kvp.Key + " - "+ Bf4Solution.GetPaddedInt(kvp.Value, 20));
			PlayerPrefs.SetInt(kvp.Key.ToString(), kvp.Value);
		}
	}

	private void SetLampsFromLocation() {
		IntToButtons(Map[SelectedLocation]);
	}

	private int ButtonsToInt() {
		var lampMask = 0;
		var i = 0;
		foreach (var lampButton in LampButtons) {
			if (lampButton.isOn) {
				lampMask |= 1 << i;
			}
			++i;
		}

		return lampMask;
	}

	private void IntToButtons(int lampMask) {
		var i = 0;
		foreach (var lampButton in LampButtons) {
			var lampBit = 1 << i++;
			lampButton.isOn = (lampMask & lampBit) > 0;
		}
	}
}
