/*MIT License

Copyright (c) 2020 - 2021 Sergiy Tolkachov

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.*/

using UnityEngine;
using System.Collections;
using SymOntoClay.Scriptables;
using System.IO;
using SymOntoClay.UnityAsset.Core;

namespace SymOntoClay
{
    [AddComponentMenu("SymOntoClay/Thing")]
    public class Thing : BaseSymOntoClayGameObject
    {
        private object GetHostListener()
        {
            var hostListener = GetComponent<IUHostListener>();

            if (hostListener == null)
            {
                return this;
            }

            return hostListener;
        }

        void Awake()
        {
#if DEBUG
            //Debug.Log("Thing Awake");
#endif

            var fullFileName = Path.Combine(Application.dataPath, SobjFile.FullName);

#if DEBUG
            //Debug.Log($"Thing Awake fullFileName = {fullFileName}");
#endif

            var settings = new GameObjectSettings();
            settings.Id = Id;
            settings.InstanceId = gameObject.GetInstanceID();

            settings.HostFile = fullFileName;

            settings.HostListener = GetHostListener();

#if DEBUG
            //Debug.Log($"Thing Awake settings = {settings}");
#endif

            _thing = WorldFactory.WorldInstance.GetGameObject(settings);

            SetSelfWorldComponent(_thing);
        }

        private IGameObject _thing;

        void Stop()
        {
            _thing.Dispose();
        }
    }
}
