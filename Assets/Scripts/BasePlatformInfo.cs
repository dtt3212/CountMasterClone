using UnityEngine;

namespace CountMasterClone
{
    public class BasePlatformInfo : MonoBehaviour
    {
        [SerializeField]
        private Transform startSpawn;

        [SerializeField]
        private Transform endSpawn;

        [SerializeField]
        private Transform treeLeftSideStart;

        [SerializeField]
        private Transform treeLeftSideEnd;

        [SerializeField]
        private Transform treeRightSideStart;

        [SerializeField]
        private Transform treeRightSideEnd;

        [SerializeField]
        private Transform playerSpawnPoint;

        [SerializeField]
        private Transform destPlatformPoint;

        [SerializeField]
        private Transform leftPoint;

        [SerializeField]
        private Transform rightPoint;

        public Vector3 StartSpawn => startSpawn.position;
        public Vector3 EndSpawn => endSpawn.position;

        public Vector3 TreeLeftSideStart => treeLeftSideStart.position;
        public Vector3 TreeLeftSideEnd => treeLeftSideEnd.position;

        public Vector3 TreeRightSideStart => treeRightSideStart.position;
        public Vector3 TreeRightSideEnd => treeRightSideEnd.position;

        public Vector3 PlayerSpawnPoint => playerSpawnPoint.position;
        public Vector3 DestPlatformPoint => destPlatformPoint.position;

        public Vector3 LeftPoint => leftPoint.position;
        public Vector3 RightPoint => rightPoint.position;
    }
}