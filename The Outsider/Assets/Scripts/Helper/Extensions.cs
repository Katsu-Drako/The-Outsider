using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extensions
{
	/// <summary>
	/// Scales values greater than 0.5 upwards and lower than 0.5 downwards.
	/// </summary>
	/// <param name="x">Input</param>
	/// <returns>Normalized value</returns>
	public static float Sigmoid(float x) {
		return (Mathf.Sin((x * Mathf.PI) - (Mathf.PI / 2f)) + 1) / 2f;
	}
	public static float Sigmoid(float x, int repetitions = 1) {
        float result = x;
        for (int i = 0; i < repetitions; i++) {
            result = Sigmoid(result);
        }
		return result;
    }
}
