using SymOntoClay.UnityAsset.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SymOntoClay.Unity3D
{
    public class Game : MonoBehaviour
    {
        // Start is called before the first frame update
        protected virtual void Start()
        {
            Debug.Log("Start");

            GameCore.SetSettings();

            Debug.Log("Start (2)");
        }
    }
}
