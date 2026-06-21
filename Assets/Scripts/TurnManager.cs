using System;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager
{
    public event Action<Character> OnTurnChanged;

    private List<Character> _characters;
    private int _index = 0;

    public TurnManager(List<Character> list)
    {
        _characters = list;
        Debug.Log("Turn of " + GetCurrentCharacter());
        OnTurnChanged?.Invoke(GetCurrentCharacter());
    }

    public void ChangeTurn()
    {
        _index++;
        if (_index >= _characters.Count)
            _index = 0;

        Debug.Log("Turn of " + GetCurrentCharacter());
        OnTurnChanged?.Invoke(GetCurrentCharacter());
    }

    public Character GetCurrentCharacter() => _characters[_index];
}