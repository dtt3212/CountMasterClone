using DG.Tweening;
using Radrat.Tweening;
using UnityEngine;
using UnityEngine.UIElements;

namespace CountMasterClone
{
    public class DialogueController: MonoBehaviour
    {
        [SerializeField]
        private UIDocument document;

        [SerializeField]
        private float animationDuration = 0.7f;

        private Label contentLabel;
        private VisualElement root;

        protected string Content
        {
            get => contentLabel.text;
            set => contentLabel.text = value;
        }

        protected virtual void Awake()
        {
            DOTween.Init();

            root = document.rootVisualElement;
            root.transform.scale = Vector3.zero;
            
            Button confirmBtn = root.Q<Button>("action_confirm");
            contentLabel = root.Q<Label>("lb_message");

            confirmBtn.clicked += OnConfirmPressed;
        }

        private void Start()
        {
            root.transform.DOScale(Vector3.one, animationDuration)
                .SetEase(Ease.InOutBack);
        }

        private void OnConfirmPressed()
        {
            root.transform.DOScale(Vector3.zero, animationDuration)
                .SetEase(Ease.InOutBack)
                .OnComplete(() =>
                {
                    Destroy(gameObject);
                });
        }
    }
}