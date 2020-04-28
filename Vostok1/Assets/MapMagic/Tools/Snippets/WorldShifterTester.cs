using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Den.Tools;

public class WorldShifterTester : MonoBehaviour 
{
	public float x;
	public float z;
	public bool shift;

	public void OnDrawGizmos ()
	{
		if (!shift) return;
		shift = false;

		System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();
		timer.Start();

		//if (ThreadWorker.CurrentCoroutine == null) //avoid shifting while coroutine is running
			{ WorldShifter.ShiftObjects(x,z); WorldShifter.ShiftParticles(x,z); }

		timer.Stop();
		Debug.Log("Shifted in " + timer.ElapsedMilliseconds + "ms");
	}

}
