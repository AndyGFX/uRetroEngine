using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace uRetroEngine
{
    public static class uRetroSound
    {
        public static Dictionary<string, string> synthsData = new Dictionary<string, string>();
        private static Dictionary<string, SfxrSynth> synths = new Dictionary<string, SfxrSynth>();

        public static void Add(string name, string definition, bool cache = false)
        {
            synthsData.Add(name, definition);
            SfxrSynth sfx = new SfxrSynth();
            sfx.parameters.SetSettingsString(definition);
            if (cache) sfx.CacheSound();
            synths.Add(name, sfx);
        }

        public static void Remove(string name)
        {
            synthsData.Remove(name);
            synths.Remove(name);
        }

        public static void Play(string name)
        {
            synths[name].Play();
        }

        public static void Stop(string name)
        {
            synths[name].Stop();
        }
    }
}