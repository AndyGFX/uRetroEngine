using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// this is the ProfileRecording class which is simply included
// directly after the CodeProfiler class in the same file.
// The ProfileRecording class is basically for "internal use
// only" - you don't need to place it on a gameobject or interact
// with it in any way yourself, it's purely used by the
// CodeProfiler to do its job.
 
public class ProfilerRecording
{
    // this class accumulates time for a single recording
	
	int count = 0;
	float startTime = 0;
	float accumulatedTime = 0;
	bool started = false;
	public string id;
	
	public ProfilerRecording(string id)
	{
		this.id = id;
	}
	
	public void Start() {
		if (started) { BalanceError(); }
		count++;
		started = true;
		startTime = Time.realtimeSinceStartup; // done last
	}
	
	public void Stop() {
		float endTime = Time.realtimeSinceStartup; // done first
		if (!started) { BalanceError(); }
		started = false;
		float elapsedTime = (endTime-startTime);
		accumulatedTime += elapsedTime;
	}
	
	public void Reset() {
		accumulatedTime = 0;
		count = 0;
		started = false;
	}
	
	void BalanceError() {
        // this lets you know if you've accidentally
        // used the begin/end functions out of order
		Debug.LogError("ProfilerRecording start/stops not balanced for '"+id+"'");  
	}
	
	public float Seconds {
		get { return accumulatedTime; }
	}
	
	public int Count {
		get { return count; }
	}
 
}