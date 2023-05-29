using UnityEngine;

namespace CountMasterClone
{
    public class EnemiesController : MonoBehaviour
    {
        public const int LayerNumber = 9;

        private GameObject lockedEnemy;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == DamagableController.LayerNumber)
            {
                // Look at the parent
                lockedEnemy = other.transform.parent.gameObject;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.layer == DamagableController.LayerNumber)
            {
                // No one will know...
                lockedEnemy = null;
            }
        }

        private void Update()
        {
            if (lockedEnemy != null)
            {
                for (int i = 0; i < transform.childCount; i++)
                {
                    Transform childTransform = transform.GetChild(i);
                    childTransform.rotation = Quaternion.LookRotation((lockedEnemy.transform.position - transform.position).normalized);
                }
            }
        }
    }
}