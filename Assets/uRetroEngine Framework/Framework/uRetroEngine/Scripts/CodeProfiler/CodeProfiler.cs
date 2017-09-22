using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

// Simple code profiler class for Unity projects
// @robotduck 2011
//
// usage: place on an empty gameobject in your scene
// then insert calls to CodeProfiler.Begin(id) and
// CodeProfiler.End(id) around the section you want to profile
//
// "id" should be string, unique to each code portion that you're timing
// for example, in your enemy update function, you might have:
//
//     function Update {
//         CodeProfiler.Begin("Enemy:Update");
//         <the rest of your enemy update code here>
//         CodeProfiler.End("Enemy:Update");
//     }
//
// the Begin id and the End id must match exactly.

public class CodeProfiler : MonoBehaviour
{
    private float startTime = 0;
    private float nextOutputTime = 5;
    private int numFrames = 0;
    private static Dictionary<string, ProfilerRecording> recordings = new Dictionary<string, ProfilerRecording>();
    private string displayText;
    private Rect displayRect = new Rect(10, 10, 460, 300);
    public Text text;

    private void Awake()
    {
        startTime = Time.time;
        displayText = "\n\nTaking initial readings...";
        this.text = this.GetComponent<Text>();
    }

    public static void Begin(string id)
    {
        // create a new recording if not present in the list
        if (!recordings.ContainsKey(id))
        {
            recordings[id] = new ProfilerRecording(id);
        }

        recordings[id].Start();
    }

    public static void End(string id)
    {
        recordings[id].Stop();
    }

    private void Update()
    {
        numFrames++;

        if (Time.time > nextOutputTime)
        {
            // time to display the results

            // column width for text display
            int colWidth = 20;

            // the overall frame time and frames per second:
            displayText = "\n\n";
            float totalMS = (Time.time - startTime) * 1000;
            float avgMS = (totalMS / numFrames);
            float fps = (1000 / (totalMS / numFrames));
            displayText += "Avg frame time: ";
            displayText += avgMS.ToString("0.#") + "ms, ";
            displayText += fps.ToString("0.#") + " fps \n";

            // the column titles for the individual recordings:
            displayText += "Total".PadRight(colWidth);
            displayText += "MS/frame".PadRight(colWidth);
            displayText += "Calls/fra".PadRight(colWidth);
            displayText += "MS/call".PadRight(colWidth);
            displayText += "Label";
            displayText += "\n";

            // now we loop through each individual recording
            foreach (var entry in recordings)
            {
                // Each "entry" is a key-value pair where the string ID
                // is the key, and the recording instance is the value:
                ProfilerRecording recording = entry.Value;

                // calculate the statistics for this recording:
                float recordedMS = (recording.Seconds * 1000);
                float percent = (recordedMS * 100) / totalMS;
                float msPerFrame = recordedMS / numFrames;
                float msPerCall = recordedMS / recording.Count;
                float timesPerFrame = recording.Count / (float)numFrames;

                // add the stats to the display text
                displayText += (percent.ToString("0.000") + "%").PadRight(colWidth);
                displayText += (msPerFrame.ToString("0.000") + "ms").PadRight(colWidth);
                displayText += (timesPerFrame.ToString("0.000")).PadRight(colWidth);
                displayText += (msPerCall.ToString("0.0000") + "ms").PadRight(colWidth);
                displayText += (recording.id);
                displayText += "\n";

                // and reset the recording
                recording.Reset();
            }
            //Debug.Log(displayText);
            this.text.text = displayText;

            // reset & schedule the next time to display results:
            numFrames = 0;
            startTime = Time.time;
            nextOutputTime = Time.time + 5;
        }
    }
}