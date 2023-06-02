using System.Collections.Generic;
using UnityEngine;

namespace CountMasterClone
{
    public class PlatformSpawner : MonoBehaviour
    {
        [SerializeField]
        private GameObject basePlatformPrefab;

        [SerializeField]
        private GameObject[] treeBatchPrefabs;

        [SerializeField]
        private GameObject[] staticHostilePrefabs;

        [SerializeField]
        private GameObject doubleGatePrefab;

        [SerializeField]
        private GameObject enemyPrefab;

        [SerializeField]
        private GameObject playerGO;

        [SerializeField]
        private GameObject finishLinePrefab;

        [SerializeField]
        private GameObject stairPrefab;

        [SerializeField]
        [Range(0.05f, 1.0f)]
        private float treeSpawnDensity = 0.7f;

        [SerializeField]
        private float distancePerTree = 3.0f;

        [SerializeField]
        private float spawnAreaInsideGatesPadding = 1.0f;

        [SerializeField]
        [Range(0.0f, 1.0f)]
        private float enemySpawnChance = 0.65f;

        private int level = 30;
        private GameObject rootGO;

        private int TargetMaxClone
        {
            get
            {
                if (level <= 30)
                {
                    return 220;
                }
                else if (level <= 50)
                {
                    return 660;
                }
                else
                {
                    return 1100;
                }
            }
        }

        private float TargetMaxMultiplier
        {
            get
            {
                if (level <= 30)
                {
                    return 5.0f;
                }
                else if (level <= 50)
                {
                    return 8.0f;
                }
                else
                {
                    return 10.0f;
                }
            }
        }

        private float TargetPlatformScale
        {
            get
            {
                if (level <= 30)
                {
                    return 6.0f;
                }
                else if (level <= 50)
                {
                    return 7.0f;
                }
                else
                {
                    return 8.0f;
                }
            }
        }

        private void SpawnTreeBatches(BasePlatformInfo platformInfo)
        {
            float approximateDistancePerTreeLeftSide = distancePerTree / treeSpawnDensity;
            float approximateDistancePerTreeRightSide = distancePerTree / treeSpawnDensity;

            GameObject emptyTreeHolder = new GameObject();
            emptyTreeHolder.transform.parent = rootGO.transform;
            emptyTreeHolder.name = "Trees";

            Vector3[] starts = new Vector3[] { platformInfo.TreeLeftSideStart, platformInfo.TreeRightSideStart };
            Vector3[] ends = new Vector3[] { platformInfo.TreeLeftSideEnd, platformInfo.TreeRightSideEnd };
            float[] dists = new float[] { approximateDistancePerTreeLeftSide, approximateDistancePerTreeRightSide };

            for (int i = 0; i < dists.Length; i++)
            {
                Vector3 spawnPos = starts[i]  + Vector3.forward * Random.Range(0.0f, dists[i] / 2);

                while (spawnPos.z < ends[i].z)
                {
                    GameObject batchPrefab = treeBatchPrefabs[Random.Range(0, treeBatchPrefabs.Length)];
                    GameObject treeBatch = Instantiate(batchPrefab, emptyTreeHolder.transform, false);
                    treeBatch.transform.position = spawnPos + Vector3.right * Random.Range(-2.0f, 2.0f);

                    spawnPos += Vector3.forward * Random.Range(dists[i] - 0.5f, dists[i] + 0.5f);
                }
            }
        }

        private void SpawnGatesAndHostiles(BasePlatformInfo info)
        {
            int totalGates = 3;
            int maxMultiplier = 3;
            int maxAddUnit = 10;
            int firstTimeMinAddUnit = 0;
            int mulSpawnRate = 0;
            int hostileOrEnemiesMaxBetweenGates = 1;
            int minEnemyCount = 0;

            if (level <= 10)
            {
                totalGates = 3;
                maxMultiplier = 3;
                maxAddUnit = 18;
                firstTimeMinAddUnit = 3;
                mulSpawnRate = 45;
                hostileOrEnemiesMaxBetweenGates = 1;
                minEnemyCount = 5;
            }
            else if (level <= 30)
            {
                totalGates = 4;
                maxMultiplier = 4;
                maxAddUnit = 20;
                firstTimeMinAddUnit = 4;
                mulSpawnRate = 40;
                hostileOrEnemiesMaxBetweenGates = 2;
                minEnemyCount = 10;
            }
            else if (level <= 50)
            {
                totalGates = 5;
                maxMultiplier = 5;
                maxAddUnit = 25;
                firstTimeMinAddUnit = 5;
                mulSpawnRate = 35;
                hostileOrEnemiesMaxBetweenGates = 3;
                minEnemyCount = 20;
            }
            else
            {
                totalGates = 6;
                maxMultiplier = 6;
                maxAddUnit = 35;
                firstTimeMinAddUnit = 6;
                mulSpawnRate = 35;
                hostileOrEnemiesMaxBetweenGates = 4;
                minEnemyCount = 20;
            }

            float distVertical = info.EndSpawn.z - info.StartSpawn.z;
            float averageBetweenGates = distVertical / (totalGates + 1);

            List<Vector3> gates = new();
            Vector3 startPositionGate = info.StartSpawn;

            List<GateGroupController> gatesObj = new();

            GameObject gateHolder = new GameObject();
            gateHolder.transform.parent = rootGO.transform;
            gateHolder.name = "Gates";

            int averageEnemyToBePerGate = TargetMaxClone / totalGates;

            for (int i = 0; i < totalGates; i++)
            {
                GameObject gate = Instantiate(doubleGatePrefab, gateHolder.transform, false);
                startPositionGate += Vector3.forward * (averageBetweenGates - Random.Range(-2.0f, 2.0f));
                gate.transform.position = new Vector3(startPositionGate.x, gate.transform.position.y, startPositionGate.z);

                GateGroupController groupController = gate.GetComponent<GateGroupController>();
                groupController.Initialize(maxAddUnit, maxMultiplier, (i == 0), (i == 0) ? firstTimeMinAddUnit : -1, mulSpawnRate);

                gates.Add(startPositionGate);
                gatesObj.Add(groupController);
            }

            int minPreviousCount = 1;

            for (int i = 0; i < totalGates; i++)
            {
                Vector3 startSpawn = gates[i] + Vector3.forward * spawnAreaInsideGatesPadding;
                Vector3 endSpawn = ((i == totalGates - 1) ? info.EndSpawn : gates[i + 1]) - Vector3.forward * spawnAreaInsideGatesPadding;

                int spawnHostileCount = Random.Range(1, hostileOrEnemiesMaxBetweenGates + 1);
                float distanceZBetweenHostiles = (endSpawn.z - startSpawn.z) / (spawnHostileCount + 1);

                int maxCloneAfterGate = gatesObj[i].GetMaxPossibleCloneAfterPassing(minPreviousCount);

                for (int j = 0; j < spawnHostileCount; j++)
                {
                    bool isEnemy = (Random.Range(0, 101) >= (enemySpawnChance * 100)) ? true : false;
                    
                    if (isEnemy)
                    {
                        GameObject enemy = Instantiate(enemyPrefab, rootGO.transform, false);
                        enemy.transform.position = startSpawn;

                        EnemiesManager manager = enemy.GetComponentInChildren<EnemiesManager>();

                        if (maxCloneAfterGate > averageEnemyToBePerGate)
                        {
                            // Forcefully reduce, but dont make the penalty too hard
                            int finalChoice = Mathf.Min((int)((maxCloneAfterGate - averageEnemyToBePerGate) * 0.7f), 99);
                            manager.Initialize(Mathf.Max(minEnemyCount, finalChoice));

                            maxCloneAfterGate -= finalChoice;
                        }
                        else
                        {
                            // Doing small penalty
                            int intentionReduce = maxCloneAfterGate * Random.Range(1, 20) / 20;
                            int finalChoice = Random.Range(Mathf.Min(minEnemyCount, intentionReduce), Mathf.Max(intentionReduce, minEnemyCount));

                            manager.Initialize(finalChoice);

                            maxCloneAfterGate -= finalChoice;
                        }
                    }
                    else
                    {
                        GameObject hostile = Instantiate(staticHostilePrefabs[Random.Range(0, staticHostilePrefabs.Length)], rootGO.transform, false);
                        hostile.transform.position = new Vector3(hostile.transform.position.x, hostile.transform.position.y + startSpawn.y, startSpawn.z);

                        maxCloneAfterGate -= 10;
                    }

                    startSpawn += Vector3.forward * distanceZBetweenHostiles;
                }

                minPreviousCount = Mathf.Max(0, maxCloneAfterGate);
            }
        }

        private void SpawnDestination(BasePlatformInfo platformInfo)
        {
            GameObject finishLine = Instantiate(finishLinePrefab, rootGO.transform, false);
            finishLine.transform.position = platformInfo.EndSpawn;

            FinishLineInfo info = finishLine.GetComponent<FinishLineInfo>();
            info.Initialize(FinishDestinationType.Staircase);

            GameObject staircase = Instantiate(stairPrefab, rootGO.transform, false);
            staircase.transform.position = platformInfo.DestPlatformPoint;

            MultiplierStairsDestController stepManager = staircase.GetComponent<MultiplierStairsDestController>();
            stepManager.Initialize(TargetMaxMultiplier);
        }

        public void Start()
        {
            rootGO = new GameObject();
            rootGO.transform.parent = null;
            rootGO.name = "World";

            GameObject basePlatform = Instantiate(basePlatformPrefab, rootGO.transform, true);
            basePlatform.transform.localScale += Vector3.right * TargetPlatformScale;

            // Spawn decoratives
            BasePlatformInfo platformInfo = basePlatform.GetComponent<BasePlatformInfo>();
            playerGO.transform.position = platformInfo.PlayerSpawnPoint;

            SpawnGatesAndHostiles(platformInfo);
            SpawnDestination(platformInfo);
            SpawnTreeBatches(platformInfo);
        }
    }
}