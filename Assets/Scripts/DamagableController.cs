using System;
using UnityEngine;

namespace CountMasterClone
{
    public class DamagableController : MonoBehaviour
    {
        public const int LayerNumber = 10;

        public event Action Died;

        public void InstantKill()
        {
            Destroy(gameObject);
            Died?.Invoke();
        }
    }
}