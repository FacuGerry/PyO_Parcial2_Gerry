using System;
using UnityEngine;
using UnityEngine.UI;

public class UiButtonsMovement : MonoBehaviour
{
    public static event Action<MovementTypes> OnMovementClicked;

    [SerializeField] private Button _btnUp;
    [SerializeField] private Button _btnDown;
    [SerializeField] private Button _btnLeft;
    [SerializeField] private Button _btnRight;

    private void Start()
    {
        _btnUp.onClick.AddListener(UpClicked);
        _btnDown.onClick.AddListener(DownClicked);
        _btnLeft.onClick.AddListener(LeftClicked);
        _btnRight.onClick.AddListener(RightClicked);
    }

    private void OnDestroy()
    {
        _btnUp.onClick.RemoveAllListeners();
        _btnDown.onClick.RemoveAllListeners();
        _btnLeft.onClick.RemoveAllListeners();
        _btnRight.onClick.RemoveAllListeners();
    }

    private void UpClicked() { OnMovementClicked?.Invoke(MovementTypes.UP); }

    private void DownClicked() { OnMovementClicked?.Invoke(MovementTypes.DOWN); }

    private void LeftClicked() { OnMovementClicked?.Invoke(MovementTypes.LEFT); }

    private void RightClicked() { OnMovementClicked?.Invoke(MovementTypes.RIGHT); }
}