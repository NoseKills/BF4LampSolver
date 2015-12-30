using UnityEngine;
using System.Collections.Generic;

public class Bf4SolutionFinder {

	public const int LampCount = 20;

	public const int AllLampsMask = (1 << LampCount) - 1;
	private static Bf4Solution solution;

	public Bf4Solution Solve(Dictionary<Location, int> map) {

		var startingState = map[Location.Currently];
		solution = new Bf4Solution(startingState);

		var toggleCombos = GetToggles(map);
		var alwaysOns = GetAlwaysOns(toggleCombos);
		var end = 1 << (toggleCombos.Length + 1);
		var bestLitLampCount = 0;
		var bestButtonMask = 0;
		var bestLampResult = 0;
		for (var pressedButtonsMask = 1; pressedButtonsMask < end; pressedButtonsMask++) {
			var changedLamps = 0;
			for (var i = 0; i < toggleCombos.Length; i++) {
				var bit = 1 << i;
				if ((pressedButtonsMask & bit) == bit) {
					changedLamps ^= toggleCombos[i];
				}
			}

			var result = (changedLamps ^ startingState) | alwaysOns;

			
			if (result == AllLampsMask) {
				solution.SetSolution(pressedButtonsMask, changedLamps);
				break;
			}
			var bitsLit = CountBits(result);
			if (bitsLit > bestLitLampCount) {
				bestLitLampCount = bitsLit;
				bestLampResult = result;
				bestButtonMask = pressedButtonsMask;
			}
		}

		if (!solution.IsSolved()) {
			solution.SetIncorrectSolution(bestButtonMask, bestLampResult);
		}
		return solution;
	}

	private int CountBits(int mask) {
		var bits = 0;
		for (var i = 0; i < LampCount; i++) {
			if ((mask & (1 << i)) != 0) ++bits;
		}

		return bits;
	}

	private int[] GetToggles(Dictionary<Location, int> map) {
		return new[] {
			map[Location.Garden],
			map[Location.Rock],
			map[Location.Waterfall],
			map[Location.Pier],
			map[Location.Furnace],
			map[Location.Pagoda],
			map[Location.Tree]
		};
	}

	private int GetAlwaysOns(int[] toggles) {
		var once = 0;
		var twice = 0;
		foreach (var mask in toggles) {
			twice |= mask & once;
			once |= mask;
		}

		if (once < AllLampsMask) {
			var nonTogglables = ~once & AllLampsMask;
			solution.SetNonTogglables(nonTogglables);
		}

		solution.SetAlwaysOns(~twice & AllLampsMask);
		return ~twice & AllLampsMask;
	}
}