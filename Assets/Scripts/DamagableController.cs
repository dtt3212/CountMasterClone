using DG.Tweening;
using System;
using UnityEngine;

namespace CountMasterClone
{
    public class DamagableController : MonoBehaviour
    {
        public event Action<GameObject, HitReason> Died;

        public void Hit(HitReason reason)
        {
            Destroy(gameObject);
            Died?.Invoke(gameObject, reason);
        }
    }
}