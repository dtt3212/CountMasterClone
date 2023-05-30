using System.Collections;
using UnityEngine;

namespace CountMasterClone
{
    public class MovableEnemyGroupController : GroupController
    {
        [SerializeField]
        private float fightEncounterDistance = 0.8f;

        [SerializeField]
        private float moveSpeed = 4.0f;

        private EnemiesManager enemiesManager;
        private IEnumerator aggressiveMoveCheckCoroutine;

        private void Start()
        {
            enemiesManager = GetComponent<EnemiesManager>();
            TargetChanged += OnTargetChanged;
        }

        private void OnTargetChanged()
        {
            if (gameObject != null)
            {
                aggressiveMoveCheckCoroutine = CheckStartMoving();
                StartCoroutine(aggressiveMoveCheckCoroutine);
            }
            else
            {
                StopCoroutine(aggressiveMoveCheckCoroutine);
                AggressiveMode = false;
            }
        }

        private IEnumerator CheckStartMoving()
        {
            WaitForSeconds checkAgain = new WaitForSeconds(0.15f);

            while (Target != null)
            {
                Collider[] colliders = Physics.OverlapSphere(transform.position, fightEncounterDistance, 1 << (int)targetEnemyLayer, queryTriggerInteraction: QueryTriggerInteraction.Collide);
                if ((colliders != null) && (colliders.Length != 0))
                {
                    AggressiveMode = true;
                    break;
                }

                yield return checkAgain;
            }

            yield break;
        }

        private void Update()
        {
            if (AggressiveMode)
            {
                float cageRadius = enemiesManager.CageRadius;

                for (int i = 0; i < transform.childCount; i++)
                {
                    Transform childTransform = transform.GetChild(i);

                    childTransform.localPosition = new Vector3(Mathf.Clamp(childTransform.localPosition.x + MoveDirection.x * moveSpeed * Time.deltaTime * 3.0f, -cageRadius + 0.3f, cageRadius - 0.3f),
                        0.0f, Mathf.Clamp(childTransform.localPosition.z + MoveDirection.z * moveSpeed * Time.deltaTime * 3.0f, -cageRadius + 0.3f, cageRadius - 0.3f));
                }
            }
        }
    }
}