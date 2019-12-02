using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GH.Data
{
    [CreateAssetMenu(fileName = "weapon_specs", menuName = "GH/Weapon Specs")]
    public class WeaponSpecsData : ScriptableObject
    {
        public int ID;

        public string NameID;

        public float Damage;

        public float ShieldDamageModifier;

        public float HullDamageModifer;

        public float MinRange;

        public float MaxRange;

        public float OptimalRange;

        public float ChargeTime;

        public float ProjectileSpeed;

        public int SalvoCount;        

        public float SplashRadius;

        public float Lifetime;

        private void Awake()
        {
            ID = GetHashCode();
        }
    }
}
