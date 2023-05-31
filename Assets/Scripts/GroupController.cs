using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;

namespace CountMasterClone
{
    public class GroupController : MonoBehaviour
    {
        [SerializeField]
        protected EntityLayer targetEnemyLayer;

        [SerializeField]
        private float restoreLookForwardDuration = 0.8f;

        private bool shouldAggressiveMove = false;

        private GameObject target;
        private Vector3 lockedMoveDirection;

        public Vector3 MoveDirection => lockedMoveDirection;
        public GameObject Target => target;

        protected event Action TargetChanged;

        protected virtual void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == (int)targetEnemyLayer)
            {
                if (target == null)
                {
                    // Look at the parent
                    target = other.transform.parent.gameObject;
                    TargetChanged?.Invoke();
                }
            }
        }

        private IEnumerator ClearTargetImpl()
        {
            yield return null;

            target = null;
            TargetChanged?.Invoke();
            ResetLookats();
        }

        protected void ClearTarget()
        {
            StartCoroutine(ClearTargetImpl());
        }

        public bool AggressiveMode
        {
            get => shouldAggressiveMove;
            set
            {
                if (shouldAggressiveMove != value)
                {
                    if (value)
                    {
                        if (target == null)
                        {
                            return;
                        }

                        lockedMoveDirection = (target.transform.position - transform.position).normalized;
                    }

                    shouldAggressiveMove = value;
                }
            }
        }

        private void ResetLookats()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform childTransform = transform.GetChild(i);
                childTransform.DORotateQuaternion(Quaternion.LookRotation(Vector3.forward), restoreLookForwardDuration);
            }
        }

        private void Update()
        {
            if (Target == null)
            {
                return;
            }

            for (int i = 0; i < transform.childCount; i++)
            {
                Transform childTransform = transform.GetChild(i);

                if (this.AggressiveMode)
                {
                    childTransform.rotation = Quaternion.LookRotation(MoveDirection);
                }
                else
                {
                    Vector3 direction = (Target.transform.position - transform.position).normalized;
                    childTransform.rotation = Quaternion.LookRotation(direction);
                }
            }
        }
    }
}