using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GH.Data
{
    [CreateAssetMenu(fileName = "ai_profile", menuName = "GH/AI Profile")]
    public class AIProfileData : ScriptableObject
    {
        public int ID;

        public string NameID;
        
        [Tooltip("How likely to ignore danger.")]        
        [Range(0f, 1f)]
        public float Aggression;

        [Tooltip("How likely to avoid danger.")]        
        [Range(0f, 1f)]
        public float Preservation;

        [Tooltip("How likely to re-evaluate targets.")]        
        [Range(0f, 1f)]
        public float Opportunism;

        [Tooltip("How likely to follow orders.")]        
        [Range(0f, 1f)]
        public float Autonomy;

        [Tooltip("How likely to pursue smaller or larger enemies.")]        
        [Range(-1f, 1f)]
        public float ScaleBias;

        private void Awake()
        {
            ID = GetHashCode();
        }
    }    
}
