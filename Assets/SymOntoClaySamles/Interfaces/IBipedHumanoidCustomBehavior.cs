﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SymOntoClay.UnityAsset.Samles.Interfaces
{
    public interface IBipedHumanoidCustomBehavior
    {
        GameObject RightHandWP { get; }
        GameObject LeftHandWP { get; }
    }
}