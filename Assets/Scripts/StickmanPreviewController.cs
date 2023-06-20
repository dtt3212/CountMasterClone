using DG.Tweening;
using UnityEngine;

namespace CountMasterClone
{
    public class StickmanPreviewController : MonoBehaviour
    {
        [SerializeField]
        private Transform scroller;

        [SerializeField]
        private Camera previewCamera;

        public float ScrollInViewportUnits
        {
            get { return scrollInViewportUnits; }
            set
            {
                if (ignoreScroll)
                {
                    return;
                }
                scrollInViewportUnits = Mathf.Clamp(value, -0.70f, 0.70f);
            }
        }

        private StickmanDatabase database;
        public int CurrentStickman => currentStickman;

        private int currentStickman;
        private float distance;
        private float scrollInViewportUnits;
        private bool ignoreScroll = false;

        public event System.Action<int> ViewChanged;

        public void Initialize(StickmanDatabase database)
        {
            distance = previewCamera.ViewportToWorldPoint(new Vector3(1, 0, -20.0f)).x - transform.position.x;
            this.database = database;

            Instantiate(database.stickmans[0].previewPrefab, scroller, false);
        }

        private void Awake()
        {
            DOTween.Init();
        }

        public void Reset()
        {
            scrollInViewportUnits = 0;
            ignoreScroll = false;

            ViewChanged?.Invoke(currentStickman);
        }

        private void Start()
        {
            Reset();
        }

        private int GetNextViewableStickman()
        {
            int viewable = currentStickman;

            if (Mathf.Abs(ScrollInViewportUnits) >= 0.25f)
            {
                if (ScrollInViewportUnits < 0.0f)
                {
                    if (viewable < database.stickmans.Length - 1)
                    {
                        viewable++;
                    }
                    else
                    {
                        scrollInViewportUnits = Mathf.Max(-0.45f, scrollInViewportUnits);
                    }
                }
                else
                {
                    if (viewable > 0)
                    {
                        viewable--;
                    }
                    else
                    {
                        scrollInViewportUnits = Mathf.Min(0.45f, scrollInViewportUnits);
                    }
                }

                if (scroller.childCount <= viewable)
                {
                    for (int i = scroller.childCount; i <= viewable; i++)
                    {
                        GameObject friend = Instantiate(database.stickmans[i].previewPrefab, scroller, false);
                        friend.transform.localPosition = -Vector3.right * distance * i;
                    }
                }
            }

            return viewable;
        }

        private void Update()
        {
            if (!ignoreScroll)
            {
                // This is to clamp scroll when nearly OOB and also spawn potential viewable stickmans
                GetNextViewableStickman();
                scroller.localPosition = -Vector3.right * ((ScrollInViewportUnits - currentStickman) * distance);
            }
        }

        public void EatScroll()
        {
            currentStickman = GetNextViewableStickman();

            ScrollInViewportUnits = 0.0f;
            ignoreScroll = true;

            scroller.DOLocalMove(currentStickman * Vector3.right * distance, 0.2f)
                .OnComplete(() => 
                {
                    ignoreScroll = false;
                    ViewChanged?.Invoke(currentStickman);
                });
        }
    }
}