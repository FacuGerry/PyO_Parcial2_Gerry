using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiCharacterActions : MonoBehaviour
{
    public event Action<int> OnMeleeClicked;      // Character index
    public event Action<int> OnRangedClicked;     // Character index
    public event Action<int> OnHealClicked;       // Character index

    [Header("Managers")]
    [SerializeField] private GameController _controller;

    [Header("Buttons lists")]
    [SerializeField] private Button[] _btnHeal;
    [SerializeField] private Button[] _btnMelee;
    [SerializeField] private Button[] _btnRange;

    private List<Character> _chars = new();

    private void Awake()
    {
        StopButtons();

        for (int i = 0; i < _btnHeal.Length; i++)
        {
            int index = i;
            _btnHeal[i].onClick.AddListener(() => HealClicked(index));
        }

        for (int i = 0; i < _btnMelee.Length; i++)
        {
            int index = i;
            _btnMelee[i].onClick.AddListener(() => MeleeClicked(index));
        }

        for (int i = 0; i < _btnRange.Length; i++)
        {
            int index = i;
            _btnRange[i].onClick.AddListener(() => RangedClicked(index));
        }
    }

    private void OnEnable()
    {
        _controller.OnGameInitialized += InitializeCharactersList;
        _controller.OnDistanceCalculated += StartButtons;
    }

    private void OnDisable()
    {
        _controller.OnGameInitialized -= InitializeCharactersList;
        _controller.OnDistanceCalculated -= StartButtons;
    }

    private void OnDestroy()
    {
        RemoveListenersFromArray(_btnHeal);
        RemoveListenersFromArray(_btnMelee);
        RemoveListenersFromArray(_btnRange);
    }

    private void InitializeCharactersList(List<Character> list) => _chars = list;
    
    private void RemoveListenersFromArray(Button[] list)
    {
        foreach (Button btn in list)
            btn.onClick.RemoveAllListeners();
    }

    private void StartButtons(List<Character> listMelee, List<Character> listRanged, List<Character> listHeal)
    {
        StopButtons();

        if (listMelee == null || listRanged == null || listHeal == null) return;

        ToogleOnButtons(_btnMelee, listMelee);
        ToogleOnButtons(_btnRange, listRanged);
        ToogleOnButtons(_btnHeal, listHeal);
    }

    private void StopButtons()
    {
        ToogleOffButtons(_btnMelee);
        ToogleOffButtons(_btnRange);
        ToogleOffButtons(_btnHeal);
    }

    private void ToogleOnButtons(Button[] buttons, List<Character> characters)
    {
        foreach (Character character in characters)
            buttons[_chars.IndexOf(character)].gameObject.SetActive(true);
    }

    private void ToogleOffButtons(Button[] list)
    {
        foreach (Button button in list)
            button.gameObject.SetActive(false);
    }

    private void MeleeClicked(int index)
    {
        OnMeleeClicked?.Invoke(index);
        StopButtons();
    }

    private void RangedClicked(int index)
    {
        OnRangedClicked?.Invoke(index);
        StopButtons();
    }

    private void HealClicked(int index)
    {
        OnHealClicked?.Invoke(index);
        StopButtons();
    }
}