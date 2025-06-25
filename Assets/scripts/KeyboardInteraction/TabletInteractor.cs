using DG.Tweening;
using UnityEngine;

public class TabletInteractor : MonoBehaviour
{
    [SerializeField] GameObject tabletTerminal;
    [SerializeField] TabletTerminal terminal;
    [SerializeField] FirstPersonController firstPersonController;
    [SerializeField] MarkerPointsSpawner markerSpawner;
    [SerializeField] Interactor interactor;
    [SerializeField] Vector3 tabletPosition;
    [SerializeField] Vector3 tabletRotation;

    Vector3 basePosition;
    Vector3 baseRotation;
    Sequence showSequence;
    Sequence hideSequence;

    bool isOn = false;

    void Awake()
    {
        tabletTerminal.SetActive(false);
        showSequence = DOTween.Sequence();
        hideSequence = DOTween.Sequence();
    }

    void Start()
    {
        basePosition = tabletTerminal.transform.localPosition;
        baseRotation = tabletTerminal.transform.localRotation.eulerAngles;
        //tabletTerminal.SetActive(false);
    }

    void Update()
    {
        if (InputManager.Instance.IsInteractWithTabletPressed() && !interactor.IsHoldingObject() && terminal.assignedTerminal != null)
        {
            if (!isOn)
            {
                if(showSequence != null && !showSequence.IsActive())
                {
                    PanelManager.Instance.ShowTipsOnce(TipsPanel.eTipType.TabletLanguage);
                    ShowTablet();
                }
            }
            else
            {
                if (hideSequence != null && !hideSequence.IsActive())
                    HideTablet();
            }
        }

        if (InputManager.Instance.IsInteractPressed() && isOn)
        {
            terminal.ChangeTextType();
        }
    }

    void HideTablet()
    {
        hideSequence = DOTween.Sequence();
        hideSequence
            .Append(tabletTerminal.transform.DOLocalMove(basePosition, 0.5f))
            .Join(tabletTerminal.transform.DOLocalRotate(baseRotation, 0.5f))
            .OnComplete(() =>
            {
                firstPersonController.StartMovement();
                InputManager.Instance.CursorVisibilityState(InputManager.CursorVisibilityRequestSource.TABLET, null);
                markerSpawner.active = true;
                interactor.canInteract = true;
                tabletTerminal.SetActive(false);
                isOn = false;
            });

        hideSequence.Play();
    }

    void ShowTablet()
    {
        showSequence = DOTween.Sequence();
        showSequence
            .OnStart(() => {
                firstPersonController.StopMovement();
                InputManager.Instance.CursorVisibilityState(InputManager.CursorVisibilityRequestSource.TABLET, true);
                markerSpawner.active = false;
                interactor.canInteract = false;
                interactor.HideLastUI();
                tabletTerminal.SetActive(true);
            })
            .OnComplete((() => isOn = true))
            .Append(tabletTerminal.transform.DOLocalMove(tabletPosition, 0.5f))
            .Join(tabletTerminal.transform.DOLocalRotate(tabletRotation, 0.5f));
        showSequence.Play();

    }
}
