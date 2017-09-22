using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace uRetroEngine
{
    /// <summary>
    /// SFXR sound engine
    /// </summary>
    public static class uRetroSound
    {
        public static Dictionary<string, string> synthsData = new Dictionary<string, string>();
        private static Dictionary<string, SfxrSynth> synths = new Dictionary<string, SfxrSynth>();

        /// <summary>
        /// Add sound defintion
        /// </summary>
        /// <param name="name">sounf fx name</param>
        /// <param name="definition">sfxr definition</param>
        /// <param name="cache">use cache</param>
        public static void Add(string name, string definition, bool cache = false)
        {
            synthsData.Add(name, definition);
            SfxrSynth sfx = new SfxrSynth();
            sfx.parameters.SetSettingsString(definition);
            if (cache) sfx.CacheSound();
            synths.Add(name, sfx);
        }

        /// <summary>
        /// Remove sfxr sound
        /// </summary>
        /// <param name="name">sound fx name</param>
        public static void Remove(string name)
        {
            synthsData.Remove(name);
            synths.Remove(name);
        }

        /// <summary>
        /// Play sfxr sound
        /// </summary>
        /// <param name="name">sound fx name</param>
        public static void Play(string name)
        {
            synths[name].Play();
        }

        /// <summary>
        /// Stop palying
        /// </summary>
        /// <param name="name">sound fx name</param>
        public static void Stop(string name)
        {
            synths[name].Stop();
        }
    }
}