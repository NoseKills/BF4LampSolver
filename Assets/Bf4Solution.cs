using System;
using UnityEngine;
using System.Collections.Generic;

public class Bf4Solution {

	private string solution;
	private readonly List<string> infos = new List<string>();
	private readonly List<string> problems = new List<string>();

	public Bf4Solution(int currentState) {
		infos.Add("Lamps lit to begin with: " + GetOnBits(currentState));
	}

	public void SetSolution(int solutionButtons, int changedLamps) {
		solution = "Buttons to Press:\n <color=#00FFFFFF>" + GetLocations(solutionButtons) + "</color>";
		infos.Add("Lamps that will toggle as result:\n" + GetOnBits(changedLamps));
	}

	public void SetIncorrectSolution(int bestCombo, int result) {
		infos.Add("Pressing:\n" + GetLocations(bestCombo) + "\nwould still leave " + GetOnBits(~result & Bf4SolutionFinder.AllLampsMask) + " unlit (best attempt)");
	}

	public void AddInfo(string i) {
		infos.Add(i);
	}

	public bool IsSolved() {
		return solution != null;
	}

	public static string GetOnBits(int c) {
		var s = "";
		for (var i = 0; i < Bf4SolutionFinder.LampCount; i++) {
			if ((c & (1 << i)) != 0) {
				s += i + ", ";
			}
		}
		return s.TrimEnd(',', ' ');
	}

	public static string GetLocations(int c) {
		var s = "";
		for (var i = 0; i < Bf4Solver.buttonLocationCount; i++) {
			if ((c & (1 << i)) != 0) {
				s += (Location)(i+1) + ", ";
			}
		}
		return s.TrimEnd(',', ' ');
	}

	public static string GetPaddedInt(int mask, int fill) {
		return Convert.ToString(mask, 2).PadLeft(fill, '0');
	}

	public void SetNonTogglables(int nonTogglables) {
		infos.Add("Lamps not affected by any button:\n" + GetOnBits(nonTogglables));
	}

	public void SetAlwaysOns(int alwaysOns) {
		infos.Add("Lamps permanently lit based on your data:\n" + GetOnBits(alwaysOns) + " ");
	}

	public override string ToString() {
		var result = solution ?? "<color=#FF7070FF>No solution found. Please double check your lamp data</color>";

		foreach (var info in infos) {
			result += "\n\n" + info;
		}

		return result;
	}
}
