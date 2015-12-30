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
	public GameObject HelpPanel;

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
		HelpLabel.text =
			"1) Select \"Initial lamp status\". Mark the LIT LAMPS** with the lamp buttons.\n" +
			"\n2) Select another location checkbox. Press the corresponding button in BF4.\n" +
			"\n3) Mark which lamps CHANGED state compared to the \"Initial lamp status\"\n" +
			"\n4) Go back and unpress the button in BF4 (so you always compare to initial state)\n" +
			"\n5) Repeat steps 2,3 and 4 for all locations\n" +
			"\n6) Mark which lamps are CURRENTLY LIT (**) under \"Current lamp status\".\n" +
			"\n7) Press \"Solve\"\n" +
			"\n?) No answer found? Repeat steps 2 and 3 to be sure you have marked the lamps right at all locations and repeat steps 6 and 7\n ";
			
		//SetTestData();
		LocationChanged(true);
		HelpPanel.SetActive(true);
		initialized = true;
	}

	private void SetTestData() {
		/*Map[Location.StartingSituation] = 1 << 10 | 1 << 16;
		Map[Location.Garden] = 1 << 2 | 1 << 3 | 1 << 7 | 1 << 10 | 1 << 14 | 1 << 16 | 1 << 18;
		Map[Location.Tree] = 1 << 3 | 1 << 7 | 1 << 8 | 1 << 10 | 1 << 11 | 1 << 13 | 1 << 14 | 1 << 16;
		Map[Location.Pagoda] = 1 << 0 | 1 << 1 | 1 << 2 | 1 << 9 | 1 << 10 | 1 << 15 | 1 << 16 | 1 << 18;
		Map[Location.Furnace] = 1 << 0 | 1 << 5 | 1 << 6 | 1 << 8 | 1 << 9 | 1 << 10 | 1 << 11 | 1 << 13 | 1<<16 | 1<<19;
		Map[Location.Pier] = 1 << 8 | 1 << 11 | 1 << 13;
		Map[Location.Waterfall] = 1 << 4 | 1 << 12 | 1 << 17;
		Map[Location.Rock] = 1 << 4 | 1 << 5 | 1 << 6 | 1 << 10 | 1 << 12 | 1 << 16 | 1 << 17 | 1 << 19;*/

		// 2nd test case from commenter on reddit.  Not sure of data quality
		
		Map[Location.StartingSituation] = 1 << 0 | 1 << 8 | 1 << 13;
		Map[Location.Garden] = 1 << 1 | 1 << 4 | 1 << 5 | 1 << 7 | 1 << 15 | 1 << 19;
		Map[Location.Tree] = 1 << 1 | 1 << 2 | 1 << 3 | 1 << 5 | 1 << 9 | 1 << 19;
		Map[Location.Pagoda] = 1 << 1 | 1 << 5 | 1 << 16 | 1 << 17 | 1 << 18 | 1 << 19;
		Map[Location.Furnace] = 1 << 0 | 1 << 2 | 1 << 3 | 1 << 8 | 1 << 11 | 1 << 13 | 1 << 14;
		Map[Location.Pier] = 1 << 4 | 1 << 6 | 1 << 7 | 1 << 10 | 1 << 12 | 1 << 15;
		Map[Location.Waterfall] = 1 << 6 | 1 << 9 | 1 << 10 | 1 << 12;
		Map[Location.Rock] = 1 << 0 | 1 << 8 | 1 << 13 | 1 << 16 | 1 << 17 | 1 << 18;

		Map[Location.Currently] = Map[Location.StartingSituation];
	}

	public void LocationChanged(bool b) {
		HelpPanel.SetActive(false);
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
		HelpPanel.SetActive(false);
	}

	public void HelpPressed() {
		HelpPanel.SetActive(true);
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
