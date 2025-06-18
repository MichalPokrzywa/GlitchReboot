using DG.Tweening;
using UnityEngine;

public class TabletInteractor : MonoBehaviour
{
    public GameObject tabletTerminal;
    public TabletTerminal terminal;
    public FirstPersonController firstPersonController;
    public MarkerPointsSpawner markerSpawner;
    public Interactor interactor;

    public Vector3 tabletPosition;
    public Vector3 tabletRotation;
    private Vector3 basePosition;
    private Vector3 baseRotation;
    private Sequence showSequence;
    private Sequence hideSequence;
    public bool isOn = false;

    void Awake()
    {
        tabletTerminal.SetActive(false);
        showSequence = DOTween.Sequence();
        hideSequence = DOTween.Sequence();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        basePosition = tabletTerminal.transform.localPosition;
        baseRotation = tabletTerminal.transform.localRotation.eulerAngles;
        //tabletTerminal.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && !interactor.IsHoldingObject() && terminal.assignedTerminal != null)
        {
            if (!isOn)
            {
                if(showSequence != null && !showSequence.IsActive())
                {
                    PanelManager.Instance.ShowTipOnce(TipsPanel.eTipType.TerminalLanguageChange);
                    ShowTablet();
                    interactor.HideLastUI();
                }
            }
            else
            {
                if (hideSequence != null && !hideSequence.IsActive())
                    HideTablet();
            }
        }

        if (Input.GetKeyDown(KeyCode.E) && isOn)
        {
            terminal.ChangeTextType();
        }
    }

    private void HideTablet()
    {
        hideSequence = DOTween.Sequence();
        hideSequence
            .Append(tabletTerminal.transform.DOLocalMove(basePosition, 0.5f))
            .Join(tabletTerminal.transform.DOLocalRotate(baseRotation, 0.5f))
            .OnComplete(() =>
            {
                firstPersonController.StartMovement();
                firstPersonController.lockCursor = true;
                markerSpawner.active = true;
                interactor.canInteract = true;
                tabletTerminal.SetActive(false);
                isOn = false;
            });

        hideSequence.Play();
    }

    private void ShowTablet()
    {
        showSequence = DOTween.Sequence();
        showSequence
            .OnStart(() => {
                firstPersonController.StopMovement();
                firstPersonController.Zoom(false);
                firstPersonController.lockCursor = false;
                markerSpawner.active = false;
                interactor.canInteract = false;
                tabletTerminal.SetActive(true);
            })
            .OnComplete((() => isOn = true))
            .Append(tabletTerminal.transform.DOLocalMove(tabletPosition, 0.5f))
            .Join(tabletTerminal.transform.DOLocalRotate(tabletRotation, 0.5f));
        showSequence.Play();

    }
}
