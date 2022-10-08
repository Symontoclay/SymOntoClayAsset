using SymOntoClay.UnityAsset.Samles.Environment;
using SymOntoClay.UnityAsset.Samles.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.SymOntoClay.Helpers
{
    public class InstancesRegistry
    {
        #region static_members
        private static object _sLockObj = new object();
        private static InstancesRegistry _instance;

        public static InstancesRegistry GetRegistry()
        {
            lock (_sLockObj)
            {
                if (_instance == null)
                {
                    _instance = new InstancesRegistry();
                }

                return _instance;
            }
        }
        #endregion


    }
}
