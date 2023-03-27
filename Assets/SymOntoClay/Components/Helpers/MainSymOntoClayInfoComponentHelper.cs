using Assets.SymOntoClay.Interfaces;
using SymOntoClay.UnityAsset.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using UnityEngine;

namespace Assets.SymOntoClay.Components.Helpers
{
    public static class MainSymOntoClayInfoComponentHelper
    {
        public static void CheckAndFixId(IMainSymOntoClayInfo target)
        {
#if UNITY_EDITOR
            Debug.Log($"MainSymOntoClayInfoComponentHelper CheckAndFixId ('{target.Name}') UniqueIdRegistry.ContainsId(Id)({target.Id}) = {UniqueIdRegistry.ContainsId(target.Id)}");
#endif

            if (UniqueIdRegistry.ContainsId(target.Id))
            {
                var oldId = target.Id;

                target.Id = $"#id{Guid.NewGuid().ToString("D").Replace("-", string.Empty)}";
            }
            UniqueIdRegistry.AddId(target.Id);

            if (string.IsNullOrWhiteSpace(target.IdForFacts) && !string.IsNullOrWhiteSpace(target.Id))
            {
                if (target.Id.StartsWith("#`"))
                {
                    target.IdForFacts = target.Id;
                }
                else
                {
                    target.IdForFacts = $"{target.Id.Insert(1, "`")}`";
                }
            }
        }

        public static void Validate(IMainSymOntoClayInfo target)
        {
            if (string.IsNullOrWhiteSpace(target.Id))
            {
                target.Id = GetIdByName(target);
            }
            else
            {
                if (target.Id == GetIdByName(target.OldName))
                {
                    target.Id = GetIdByName(target);
                }
            }

            if (target.Id.StartsWith("#`"))
            {
                target.IdForFacts = target.Id;
            }
            else
            {
                target.IdForFacts = $"{target.Id.Insert(1, "`")}`";
            }

            target.OldName = target.Name;
        }

        private static string GetIdByName(IMainSymOntoClayInfo target)
        {
            return GetIdByName(target.Name);
        }

        private static string GetIdByName(string nameStr)
        {
            return $"#{nameStr}";
        }
    }
}
