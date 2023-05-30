using System;
using UnityEngine;

namespace CountMasterClone
{
    public class DamagableController : MonoBehaviour
    {
        public event Action Died;

        public void Hit()
        {
            Destroy(gameObject);
            Died?.Invoke();
        }
    }
}