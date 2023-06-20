using Cysharp.Threading.Tasks;
using UnityEngine;

using UniRx;

namespace CountMasterClone
{
    public class PlayerGroupController : GroupController
    {
        [SerializeField]
        private float attackSpeed = 4.0f;

        [SerializeField]
        private GameObject notificationPrefab;

        [SerializeField]
        private ValuableState valuableState;

        [SerializeField]
        private AudioSource hitAudioSource;

        [SerializeField]
        private AudioSource popAudioSource;

        [SerializeField]
        private AudioSource cashInAudioSource;

        [SerializeField]
        private AudioSource oneShotAudioSource;

        [SerializeField]
        private AudioClip deadByHostileSFX;

        private PlayerGroupManager clonableGroupManager;

        private FinishDestinationType destinationType = FinishDestinationType.None;

        public bool ReachedFinish => destinationType != FinishDestinationType.None;
        public FinishDestinationType FinishDestinationType => destinationType;

        public event System.Action<bool> GameEnded;

        private bool stairing = false;
        private bool treasuring = false;
        private bool towering = false;
        private bool endGamed = false;
        private bool losing = false;
        private int deathPlayingCount = 0;

        private int coinCollected;

        public void TotalReset()
        {
            transform.localPosition = Vector3.zero;
            destinationType = FinishDestinationType.None;
            stairing = false;
            treasuring = false;
            endGamed = false;
            losing = false;
            towering = false;

            clonableGroupManager.MassacreAndReset();
        }

        private async UniTaskVoid TransitionToEndgame()
        {
            if (endGamed)
            {
                return;
            }

            endGamed = true;

            valuableState.money += coinCollected;
            valuableState.Save();

            if (!losing && coinCollected != 0)
            {
                cashInAudioSource.Play();
                
                GameObject notification = Instantiate(notificationPrefab, transform.position + new Vector3(0, 8, 5), Quaternion.identity);
                EarnedNotificationController notificationController = notification.GetComponent<EarnedNotificationController>();

                notificationController.Display($"+${coinCollected}=${valuableState.money}");
                await UniTask.Delay((int)(4.5f * 1000));
            }
            else
            {
                await UniTask.Delay((int)(2.0f * 1000));
            }

            GameEnded?.Invoke(!losing);
        }

        public bool ReachedEndgame
        {
            get => endGamed;
            set
            {
                TransitionToEndgame().Forget();
            }
        }

        private void Awake()
        {
            clonableGroupManager = GetComponent<PlayerGroupManager>();
            clonableGroupManager.CountLabelEnabled = false;

            clonableGroupManager.Disbanded += () =>
            {
                hitAudioSource.Stop();
                losing = true;
                ReachedEndgame = true;
                deathPlayingCount = 0;
            };

            valuableState.activeStickman.Subscribe(newActive =>
            {
                clonableGroupManager.MassacreAndReset();
            });

            clonableGroupManager.Attacking += () =>
            {
                if (!hitAudioSource.isPlaying)
                {
                    hitAudioSource.Play();
                }
            };

            clonableGroupManager.MeetStaticHostile += () =>
            {
                if (deathPlayingCount++ < 5)
                {
                    oneShotAudioSource.PlayOneShot(deadByHostileSFX);
                }
            };
        }

        private void Start()
        {
            TargetChanged += OnTargetChanged;
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
                        addNumber = clonableGroupManager.CloneCount * (gate.Value - 1);
                        break;
                    }
            }

            if (addNumber == 0)
            {
                return;
            }

            clonableGroupManager.Clone(addNumber);
            popAudioSource.Play();
        }

        private void OnTargetChanged()
        {
            if (Target != null)
            {
                GroupManager targetGroupManager = Target.GetComponent<GroupManager>();
                if (targetGroupManager != null)
                {
                    targetGroupManager.Disbanded += () =>
                    {
                        AggressiveMode = false;
                        deathPlayingCount = 0;
                        hitAudioSource.Stop();

                        ClearTarget();
                    };
                }
            }
        }

        protected override void OnTriggerEnter(Collider other)
        {
            base.OnTriggerEnter(other);
            OnTriggerEnterAsync(other).Forget();
        }

        protected async UniTaskVoid OnTriggerEnterAsync(Collider other)
        {
            switch ((EntityLayer)other.gameObject.layer)
            {
                case EntityLayer.Gate:
                    SpawnNewClones(other.gameObject.GetComponent<GateController>());
                    break;

                case EntityLayer.EnemiesCage:
                    // The first clone enter the cage will make the group go into this aggressive mode
                    AggressiveMode = true;
                    break;

                case EntityLayer.Finish:
                    {
                        if (!towering)
                        {
                            towering = true;

                            FinishLineInfo info = other.gameObject.GetComponent<FinishLineInfo>();
                            if (info != null)
                            {
                                destinationType = info.DestinationType;
                            }

                            hitAudioSource.Play();
                            await clonableGroupManager.BuildTower();
                            hitAudioSource.Stop();
                        }
                        break;
                    }

                case EntityLayer.Stair:
                    if (!stairing)
                    {
                        stairing = true;
                        
                        
                        System.Tuple<float, bool> coinMultiplierAndKeepGoing = await clonableGroupManager.StepOnStair(other.gameObject.GetComponent<MultiplierStairsDestController>());
                        float coinBase = other.GetComponent<ObjectCoinValue>().value;

                        coinCollected += (int)(coinBase * coinMultiplierAndKeepGoing.Item1);

                        Debug.Log("Collected " + coinCollected + " coins");

                        if (!coinMultiplierAndKeepGoing.Item2)
                        {
                            ReachedEndgame = true;
                        }
                    }

                    break;

                case EntityLayer.Treasure:
                    if (!treasuring)
                    {
                        treasuring = true;
                        ReachedEndgame = true;

                        coinCollected += other.GetComponent<ObjectCoinValue>().value;
                    }

                    break;
            }
        }

        private void Update()
        {
            if (ReachedEndgame)
            {
                return;
            }

            if (AggressiveMode)
            {
                transform.localPosition += MoveDirection * attackSpeed * Time.deltaTime;
            }
        }

        public void Kickup()
        {
            clonableGroupManager.CountLabelEnabled = true;
        }
    }
}
