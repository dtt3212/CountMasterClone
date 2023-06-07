using DG.Tweening;
using Radrat.Tweening;
using UnityEngine;
using UnityEngine.UIElements;

namespace CountMasterClone
{
    public class MenuController : MonoBehaviour
    {
        [SerializeField]
        private GameObject helpDialoguePrefab;

        [SerializeField]
        private UIDocument document;

        [SerializeField]
        private float flashDuration = 0.6f;

        [SerializeField]
        private float flashCooldown = 1.5f;

        [SerializeField]
        private float uiFadeDuration = 0.3f;

        [SerializeField]
        private PlayerController playerController;

        private void Awake()
        {
            DOTween.Init();
        }

        private void Start()
        {
            VisualElement root = document.rootVisualElement;

            Button helpBtn = root.Q<Button>("action_help");
            helpBtn.clicked += OnHelpPressed;

            Label instructionLb = root.Q<Label>("lb_instruction_short");

            DOTween.Sequence()
                .Append(instructionLb.style.DOOpacity(1.0f, flashDuration))
                .AppendInterval(flashCooldown)
                .OnComplete(() => instructionLb.style.opacity = 0.0f)
                .SetLoops(-1);

            VisualElement realRoot = root.Q<VisualElement>("ve_root");
            realRoot.RegisterCallback<PointerUpEvent>(x =>
            {
                realRoot.style.DOOpacity(0.0f, uiFadeDuration)
                    .OnComplete(() =>
                    {
                        realRoot.style.display = DisplayStyle.None;
                        playerController.Kickup();
                    });
            });
        }

        private void OnHelpPressed()
        {
            Instantiate(helpDialoguePrefab, null, false);
        }
    }
}