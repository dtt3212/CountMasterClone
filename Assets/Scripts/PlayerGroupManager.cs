using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

namespace CountMasterClone
{
    public class PlayerGroupManager : GroupManager
    {
        private List<List<Transform>> towerLayers;

        [SerializeField]
        private float cloneRepositionInTowerDuration = 0.5f;

        [SerializeField]
        private float towerLayerHeight = 0.5f;

        [SerializeField]
        private float timeBetweenLayerBuildup = 0.8f;

        private bool isBuilding = false;

        public void StartBuildingTower()
        {
            if (isBuilding)
            {
                return;
            }

            isBuilding = true;
            StartCoroutine(BuildTower());
        }

        public IEnumerator BuildTower()
        {
            WaitForSeconds waitBuildup = new WaitForSeconds(timeBetweenLayerBuildup);

            towerLayers = new();

            int left = transform.childCount;

            // Index 0 will say how many 1-clone layer there is
            // Index 1 will say how many 2-clone layer there is
            List<int> layerIndiciesCount = new();
            while (left > 0)
            {
                int maxLayerCurrent = 1;
                while (true)
                {
                    if ((maxLayerCurrent + 2) * (maxLayerCurrent + 1) / 2 > left)
                    {
                        break;
                    }

                    maxLayerCurrent++;
                }

                for (int i = 0; i < maxLayerCurrent; i++)
                {
                    if (layerIndiciesCount.Count < i + 1)
                    {
                        layerIndiciesCount.Add(1);
                    }
                    else
                    {
                        layerIndiciesCount[i]++;
                    }
                }

                left -= (maxLayerCurrent + 1) * maxLayerCurrent / 2;
            }

            Queue<Vector2> searching = new();
            List<Vector2> closed = new();
            
            searching.Enqueue(new Vector2(childMap.GetLength(0) / 2, childMap.GetLength(1) / 2));
            closed.Add(searching.Peek());

            for (int layerCountIndex = 0; layerCountIndex < layerIndiciesCount.Count; layerCountIndex++)
            {
                for (int repeatCount = 0; repeatCount < layerIndiciesCount[layerCountIndex]; repeatCount++)
                {
                    int toTake = layerCountIndex + 1;

                    List<Transform> layer = new();

                    while (toTake > 0)
                    {
                        Vector2 positionTake = searching.Dequeue();
                        Transform childTransformCurrent = childMap[(int)positionTake.x, (int)positionTake.y];

                        if (childTransformCurrent != null)
                        {
                            layer.Add(childTransformCurrent);
                            toTake--;
                        }

                        for (int i = -1; i <= 1; i++)
                        {
                            for (int j = -1; j <= 1; j++)
                            {
                                Vector2 checkPosition = positionTake + new Vector2(i, j);
                                if ((checkPosition.x < 0) || (checkPosition.y < 0) || (checkPosition.x >= childMap.GetLength(0)) || (checkPosition.y >= childMap.GetLength(1)))
                                {
                                    continue;
                                }

                                if (!closed.Contains(checkPosition))
                                {
                                    searching.Enqueue(checkPosition);
                                    closed.Add(checkPosition);
                                }
                            }
                        }
                    }

                    towerLayers.Add(layer);

                    float currentY = 0;

                    for (int i = towerLayers.Count - 1; i >= 0; i--)
                    {
                        int currentLayerCount = towerLayers[i].Count;
                        float middleIndex = currentLayerCount / 2 - ((currentLayerCount % 2 == 0) ? 0.5f : 0);

                        for (int j = 0; j < currentLayerCount; j++)
                        {
                            float x = (j - middleIndex) * distanceBetweenSpawn;

                            towerLayers[i][j].transform.DOComplete();
                            towerLayers[i][j].transform.DOLocalMove(new Vector3(x, currentY, 0), cloneRepositionInTowerDuration);
                        }

                        currentY += towerLayerHeight;
                    }

                    yield return waitBuildup;
                }
            }
        }
    }
}