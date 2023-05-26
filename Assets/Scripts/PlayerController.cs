using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using System.Collections.Generic;

namespace CountMasterClone
{
    public class PlayerController : MonoBehaviour
    {
        public const int LayerNumber = 6;

        [SerializeField]
        private float verticalMoveSpeedPerSec = 0.5f;

        [SerializeField]
        private float horizontalMoveSpeedPerSec = 10.0f;

        [SerializeField]
        private float cameraDistance = 10.0f;

        [SerializeField]
        private Camera gameCamera;

        [SerializeField]
        private GameObject clonePrefab;

        [SerializeField]
        private GameObject stickmanContainer;

        [SerializeField]
        private float distanceBetweenSpawn = 0.7f;

        [SerializeField]
        private float unevenDistance = 0.2f;

        [SerializeField]
        [Range(0.0f, 1.0f)]
        private float unevenIntensity = 0.4f;

        [SerializeField]
        private int perColumnCloneEstimate = 7;

        [SerializeField]
        private bool spawnInCircleLayers = false;

        [SerializeField]
        [Range(0.0f, 10.0f)]
        private float moveToNewPositonDuration = 0.3f;

        private bool gateHit = false;

        public int CloneCount => stickmanContainer.transform.childCount;

        private void Awake()
        {
            DOTween.Init();
        }

        private void OnMove(InputValue value)
        {
            Vector2 pos = value.Get<Vector2>();
            Vector3 realWorldPos = gameCamera.ScreenToWorldPoint(new Vector3(pos.x, pos.y, cameraDistance));

            Vector3 direction = (realWorldPos - transform.position);
            direction.Scale(Vector3.right);

            transform.position += direction.normalized * horizontalMoveSpeedPerSec * Time.deltaTime; 
        }

        private void SpawnNewClones(GateController gate)
        {
            if (gate == null)
            {
                return;
            }
 
            int addNumber = 0;

            switch (gate.GateType)
            {
                case GateType.Add:
                    {
                        addNumber = gate.Value;
                        break;
                    }

                case GateType.Multiplication:
                    {
                        addNumber = CloneCount * (gate.Value - 1);
                        break;
                    }
            }

            if (addNumber == 0)
            {
                return;
            }

            for (int i = 0; i < addNumber; i++)
            {
                Instantiate(clonePrefab, stickmanContainer.transform, false);
            }

            RepositionStickmans();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == GateController.LayerNumber)
            {
                if (gateHit == false)
                {
                    SpawnNewClones(other.gameObject.GetComponent<GateController>());
                }
            }
        }

        private void RepositionStickmans()
        {
            Transform containerTransform = stickmanContainer.transform;
            float shrunkenDistance = (containerTransform.childCount >= 100) ? distanceBetweenSpawn / Mathf.Log(containerTransform.childCount, 110) : distanceBetweenSpawn;

            if (spawnInCircleLayers)
            {
                int currentLayer = 1;
                int currentObjectInLayer = 0;
                float anglePerObject = 360.0f;

                for (int i = 0; i < containerTransform.childCount; i++)
                {
                    Transform stickmanTransfrom = containerTransform.GetChild(i);

                    float x = shrunkenDistance * (currentLayer - 1) * Mathf.Cos(anglePerObject * currentObjectInLayer);
                    float z = shrunkenDistance * (currentLayer - 1) * Mathf.Sin(anglePerObject * currentObjectInLayer);

                    stickmanTransfrom.DOLocalMove(new Vector3(x, 0, z), moveToNewPositonDuration)
                        .SetEase(Ease.OutBack);

                    if (++currentObjectInLayer >= currentLayer * currentLayer)
                    {
                        currentLayer++;
                        currentObjectInLayer = 0;

                        anglePerObject = Mathf.PI * 2 / (currentLayer * currentLayer);
                    }
                }
            }
            else
            {
                List<int> colCounts = new List<int>();
                int totalCalced = 0;

                while (totalCalced < containerTransform.childCount)
                {
                    // The first outer column should have much more small clone density, so we gonna check that too!
                    // As we go deeper in, we will raise the minimum a bit
                    // If there's too little clone, laid them out in a big estimation will not look too pretty, instead we should shrink that
                    // estimation down
                    int cloneRange = Mathf.Max((containerTransform.childCount < perColumnCloneEstimate * 3 ? Mathf.Max(containerTransform.childCount / 3 - 2, 1) : perColumnCloneEstimate) - Mathf.Max(4 - colCounts.Count, 0), 1);
                    int columnCloneCount = 0;

                    if (colCounts.Count <= 3)
                    {
                        columnCloneCount = Mathf.Min(cloneRange, containerTransform.childCount - totalCalced);
                    }
                    else
                    {
                        columnCloneCount = Mathf.Min(Random.Range(cloneRange, cloneRange + 3), containerTransform.childCount - totalCalced);
                    }

                    colCounts.Add(columnCloneCount);
                    totalCalced += columnCloneCount;
                }

                int startingColumn = -(colCounts.Count / 2);
                int currentStickman = 0;

                for (int i = 0; i < colCounts.Count; i++)
                {
                    int column = startingColumn + i;
                    int cloneColumnCount = colCounts[i];
                    int startingRow = -(cloneColumnCount / 2);

                    for (int j = 0; j < cloneColumnCount; j++)
                    {
                        float x = shrunkenDistance * column + (Mathf.Cos(startingRow + j) * unevenDistance * unevenIntensity);
                        float z = shrunkenDistance * (startingRow + j) + (Mathf.Sin(column) * unevenDistance * unevenIntensity);

                        Transform stickmanTransfrom = containerTransform.GetChild(currentStickman++);

                        stickmanTransfrom.DOLocalMove(new Vector3(x, 0, z), moveToNewPositonDuration)
                            .SetEase(Ease.OutBack);
                    }
                }
            }
        }

        private void Start()
        {
        }

        private void Update()
        {
            transform.position += new Vector3(0, 0, verticalMoveSpeedPerSec * Time.deltaTime);
        }
    }
}
