using DG.Tweening;
using UnityEngine;

namespace CountMasterClone
{
    public class EnemiesManager: GroupManager
    {
        [SerializeField]
        private GameObject cageCircle;

        [SerializeField]
        private float cageRadius = 3.0f;

        [SerializeField]
        [Range(0.2f, 3.0f)]
        private float destroyDuration = 0.5f;

        public float CageRadius => cageRadius;

        protected override void OnDisbanding()
        {
            SphereCollider collider = cageCircle.GetComponent<SphereCollider>();
            if (collider != null)
            {
                collider.enabled = false;
            }

            cageCircle.transform.DOScale(Vector3.zero, destroyDuration)
                .SetEase(Ease.InOutBack)
                .OnComplete(() => base.OnDisbanding());
        }

        public void Initialize(Camera gameCamera, int count)
        {
            Initialize(gameCamera);
            Clone(count);
        }

        private void Awake()
        {
            cageCircle.transform.localScale = new Vector3(cageRadius * 2, 1, cageRadius * 2);
        }

        private void Start()
        {
        }
    }
}