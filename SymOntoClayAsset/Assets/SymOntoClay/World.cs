using SymOntoClay.UnityAsset.Core;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace SymOntoClay.Unity3D
{
    public class World : MonoBehaviour
    {
        protected virtual void Awake()
        {
            Debug.Log($"Awake {Directory.GetCurrentDirectory()}");
        }

        // Start is called before the first frame update
        protected virtual void Start()
        {
            Debug.Log("Start");

            //WorldCore.SetSettings();

            Debug.Log("Start (2)");
        }
    }
}
