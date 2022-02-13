using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SymOntoClay.UnityAsset.Samles.Interfaces
{
    public interface IPlayerCommonBus
    {
        float GetAxis(string name);
        bool GetKeyUp(KeyCode key);
        bool GetKeyDown(KeyCode key);
        bool GetButtonDown(string name);
        bool GetMouseButtonUp(int button);
        bool GetMouseButtonDown(int button);
        void SetCharacterMode();
        void AddWindow();
        void ReleaseWindow();
    }
}
