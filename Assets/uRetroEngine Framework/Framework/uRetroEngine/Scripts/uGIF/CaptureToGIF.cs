using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;

using System.Threading;

namespace uRetroEngine
{
    public class CaptureToGIF : MonoBehaviour
    {
        public float frameRate = 15;
        public bool capture;
        public int downscale = 1;
        public float captureTime = 10;
        public bool useBilinearScaling = true;
        public string filename = "screenshot";
        public GameObject recIcon;
        public bool enableRecIcon = true;
        private Thread thread = null;

        [System.NonSerialized]
        public byte[] bytes = null;

        private void Start()
        {
            period = 1f / frameRate;
            colorBuffer = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
            if (this.recIcon == null) this.enableRecIcon = false;
            this.recIcon.SetActive(false);
            startTime = Time.time;
        }

        public void Encode()
        {
            bytes = null;
            this.thread = new Thread(_Encode);

            thread.Start();
            StartCoroutine(WaitForBytes());
        }

        private void OnDestroy()
        {
            if (this.thread != null)
                this.thread.Interrupt();
        }

        private IEnumerator WaitForBytes()
        {
            if (this.enableRecIcon) this.recIcon.SetActive(false);
            while (bytes == null) yield return null;
            string path = uRetroSystem.GetRoot() + "/" + uRetroConfig.cartridgesFolder + "/" + uRetroConfig.cartridgeName + "/" + uRetroConfig.cartridgeName + "_" + this.filename + ".gif";
            System.IO.File.WriteAllBytes(path, bytes);
            bytes = null;
            uRetroConsole.Show();
            uRetroConsole.Print("GIF screenshot '" + path + "' saved.");
        }

        public void _Encode()
        {
            capture = false;

            var ge = new GIFEncoder();
            ge.useGlobalColorTable = true;
            ge.repeat = 0;
            ge.FPS = frameRate;
            ge.transparent = new Color32(255, 0, 255, 255);
            ge.dispose = 1;

            var stream = new MemoryStream();
            ge.Start(stream);
            foreach (var f in frames)
            {
                if (downscale != 1)
                {
                    if (useBilinearScaling)
                    {
                        f.ResizeBilinear(f.width / downscale, f.height / downscale);
                    }
                    else {
                        f.Resize(downscale);
                    }
                }
                f.Flip();
                ge.AddFrame(f);
            }
            ge.Finish();
            bytes = stream.GetBuffer();
            stream.Close();
        }

        private void OnPostRender()
        {
            if (capture)
            {
                T += Time.deltaTime;
                if (T >= period)
                {
                    T = 0;
                    colorBuffer.ReadPixels(new Rect(0, 0, colorBuffer.width, colorBuffer.height), 0, 0, false);
                    frames.Add(new Image(colorBuffer));
                    if (this.enableRecIcon) this.recIcon.SetActive(true);
                }
                if (Time.time > (startTime + captureTime))
                {
                    capture = false;
                    Encode();
                }
            }
        }

        private List<Image> frames = new List<Image>();
        private Texture2D colorBuffer;
        private float period;
        private float T = 0;
        private float startTime = 0;
    }
}