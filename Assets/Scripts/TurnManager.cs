using System;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public event Action<Character> OnTurnChanged;

    [SerializeField] private List<Character> _characters;
    private int _index = 0;

    private void Start()
    {
        
    }

    private void ChangeTurn()
    {
        _index++;
        if (_index >= _characters.Count)
            _index = 0;

        OnTurnChanged?.Invoke(GetCurrentCharacter());
    }

    public Character GetCurrentCharacter() => _characters[_index];
}