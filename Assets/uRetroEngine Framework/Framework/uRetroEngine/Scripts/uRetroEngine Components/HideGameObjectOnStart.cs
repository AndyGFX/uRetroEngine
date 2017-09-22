using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace uRetroEngine
{
    public class HideGameObjectOnStart : MonoBehaviour
    {
        // Use this for initialization
        private void Awake()
        {
            this.gameObject.SetActive(false);
        }
    }
}