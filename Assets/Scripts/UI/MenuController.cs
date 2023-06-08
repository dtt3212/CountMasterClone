using DG.Tweening;
using Radrat.Tweening;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

namespace CountMasterClone
{
    public class MenuController : MonoBehaviour
    {
        [SerializeField]
        private GameObject helpDialoguePrefab;

        [SerializeField]
        private ShopController shopController;

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

        private VisualElement realRoot;

        private void Awake()
        {
            EventSystem.SetUITookitEventSystemOverride(null, false, false);
            DOTween.Init();
        }

        private void Start()
        {
            VisualElement root = document.rootVisualElement;

            Button helpBtn = root.Q<Button>("action_help");
            helpBtn.clicked += OnHelpPressed;

            Button shopBtn = root.Q<Button>("action_store");
            shopBtn.clicked += OnShopPressed;

            Label instructionLb = root.Q<Label>("lb_instruction_short");

            DOTween.Sequence()
                .Append(instructionLb.style.DOOpacity(1.0f, flashDuration))
                .AppendInterval(flashCooldown)
                .OnComplete(() => instructionLb.style.opacity = 0.0f)
                .SetLoops(-1);

            realRoot = root.Q<VisualElement>("ve_root");
            realRoot.RegisterCallback<PointerUpEvent>(x =>
            {
                Hide(() => playerController.Kickup());
            });

            shopController.Closed += Show;
        }

        private void Hide(Action onHideDone)
        {
            realRoot.style.DOOpacity(0.0f, uiFadeDuration)
                .OnComplete(() =>
                {
                    realRoot.style.display = DisplayStyle.None;
                    onHideDone?.Invoke();
                });
        }

        private void Show()
        {
            realRoot.style.display = DisplayStyle.Flex;
            realRoot.style.DOOpacity(1.0f, uiFadeDuration);
        }

        private void OnHelpPressed()
        {
            Instantiate(helpDialoguePrefab, null, false);
        }

        private void OnShopPressed()
        {
            Hide(() => shopController.Show());
        }
    }
}