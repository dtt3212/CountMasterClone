using DG.Tweening;
using System;
using UnityEngine;

namespace CountMasterClone
{
    public class DamagableController : MonoBehaviour
    {
        [SerializeField]
        private GameObject deadPopParticle;

        public event Action<GameObject, HitReason> Died;

        public void Hit(HitReason reason)
        {
            GameObject popParticleObj = Instantiate(deadPopParticle, null, false);
            popParticleObj.transform.position += transform.position;

            Destroy(gameObject);
            Died?.Invoke(gameObject, reason);
        }
    }
}