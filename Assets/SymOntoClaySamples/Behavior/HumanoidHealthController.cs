using SymOntoClay.UnityAsset.Samples.Interfaces;
using UnityEngine;

namespace SymOntoClay.UnityAsset.Samples.Behavior
{
    [AddComponentMenu("SymOntoClay Samples/HumanoidHealthController")]
    public class HumanoidHealthController: MonoBehaviour, IHumanoidHealthCustomBehavior, ITargetOfDamageCustomBehavior
    {
        private IDieCustomBehavior _dieProvider;

        public int Health = 10;

        void Start()
        {
            _dieProvider = GetComponent<IDieCustomBehavior>();
        }

        public void SetHit(RaycastHit shootHit, int damagePerShot)
        {
            Health -= damagePerShot;

            if(Health < 0)
            {
                Health = 0;
                _dieProvider.Die();
            }
        }
    }
}
