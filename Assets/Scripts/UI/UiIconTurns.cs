using System.Collections.Generic;
using UnityEngine;

public class UiIconTurns : MonoBehaviour
{
    [SerializeField] private RectTransform _icon;
    [SerializeField] private List<RectTransform> _positions;
    [SerializeField] private GameController _controller;

    private void OnEnable()
    {
        _controller.OnTurnChanged += OnTurnChanged_ChangeIconPosition;
    }

    private void OnDisable()
    {
        _controller.OnTurnChanged -= OnTurnChanged_ChangeIconPosition;
    }

    private void OnTurnChanged_ChangeIconPosition(int index) => _icon.position = _positions[index].position;
}