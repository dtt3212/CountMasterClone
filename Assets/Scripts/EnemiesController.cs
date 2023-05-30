using System.Collections;
using UnityEngine;

namespace CountMasterClone
{
    public class EnemiesController : MonoBehaviour
    {
        public const int LayerNumber = 9;

        [SerializeField]
        private GameObject cageCircle;

        [SerializeField]
        private float cageRadius = 3.0f;

        [SerializeField]
        private float fightEncounterDistance = 0.8f;

        private GameObject target;
        private bool shouldAggressiveMove = false;

        private IEnumerator aggressiveMoveCheckCoroutine;

        public void Initialize(int count)
        {
            ClonableGroupController groupController = GetComponent<ClonableGroupController>();
            groupController?.Clone(count);
        }

        private void Awake()
        {
            cageCircle.transform.localScale = new Vector3(cageRadius * 2, 1, cageRadius * 2);
        }

        private void Start()
        {
            Initialize(Random.Range(10, 21));
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == PlayerController.LayerNumber)
            {
                if (target == null)
                {
                    // Look at the parent
                    target = other.transform.parent.gameObject;
                    aggressiveMoveCheckCoroutine = CheckStartMoving();

                    StartCoroutine(aggressiveMoveCheckCoroutine);
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.layer == PlayerController.LayerNumber)
            {
                if (target != null)
                {
                    // No one will know...
                    target = null;
                    StopCoroutine(aggressiveMoveCheckCoroutine);
                }
            }
        }

        private IEnumerator CheckStartMoving()
        {
            WaitForSeconds checkAgain = new WaitForSeconds(0.15f);

            while (target != null)
            {
                Collider[] colliders = Physics.OverlapSphere(transform.position, cageRadius + fightEncounterDistance, 1 << PlayerController.LayerNumber, queryTriggerInteraction: QueryTriggerInteraction.Collide);
                if ((colliders != null) && (colliders.Length != 0))
                {
                    shouldAggressiveMove = true;
                    break;
                }

                yield return checkAgain;
            }

            yield break;
        }

        private void Update()
        {
            if (target != null)
            {
                for (int i = 0; i < transform.childCount; i++)
                {
                    Transform childTransform = transform.GetChild(i);
                    Vector3 direction = (target.transform.position - transform.position).normalized;

                    childTransform.rotation = Quaternion.LookRotation(direction);

                    if (shouldAggressiveMove)
                    {
                        childTransform.localPosition = new Vector3(Mathf.Clamp(childTransform.localPosition.x + direction.x * Time.deltaTime * 3.0f, -cageRadius + 0.3f, cageRadius - 0.3f),
                            0.0f, Mathf.Clamp(childTransform.localPosition.z + direction.z * Time.deltaTime * 3.0f, -cageRadius + 0.3f, cageRadius - 0.3f));
                    }
                }
            }
        }
    }
}