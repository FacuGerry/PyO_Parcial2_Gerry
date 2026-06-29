using System;
using System.Collections.Generic;
using UnityEngine;

public class ActionsMananger
{
    public event Action<List<Character>, List<Character>, List<Character>> OnDistanceCalculated; // _charsToMelee, _charsToRange, _charsToHeal

    public event Action<bool> OnEnemyAttacked; // hasAttacked?

    // Characters - RECEIVE
    private List<Character> _chars = new();

    // Characters Health Systems- RECEIVE
    private List<HealthSystem> _hs = new();

    // Characters To Attack - CREATE
    private List<Character> _charsToMelee = new();
    private List<Character> _charsToRange = new();
    private List<Character> _charsToHeal = new();

    private Character _activeChar = null;

    private bool _enemiesDead = false;

    public void InitializeActions(List<Character> chars, List<HealthSystem> hs)
    {
        _chars = chars;
        _hs = hs;
    }

    public void ToogleEnemiesDead(bool enemiesDead) => _enemiesDead = enemiesDead;

    public void ChangeActiveCharacter(Character character)
    {
        _activeChar = character;
        if (_activeChar.GetData().isPlayer)
            StartPlayerActions();
        else
            StartEnemyActions();
    }

    public void StartPlayerActions()
    {
        StopActions();

        GetCharactersInDistance(1, _charsToMelee, !_enemiesDead);
        GetCharactersInDistance(_activeChar.GetData().range, _charsToRange, !_enemiesDead);

        if (_activeChar.GetData().healingRange > 0)
            GetCharactersInDistance(_activeChar.GetData().healingRange, _charsToHeal, false);
        else
            _charsToHeal.Clear();

        _charsToHeal.Add(_activeChar);

        OnDistanceCalculated?.Invoke(_charsToMelee, _charsToRange, _charsToHeal);
    }

    public void StartEnemyActions()
    {
        GetCharactersInDistance(1, _charsToMelee, true);
        GetCharactersInDistance(_activeChar.GetData().range, _charsToRange, true);
        EnemyAttack();
    }

    public void StopActions()
    {
        _charsToMelee.Clear();
        _charsToRange.Clear();
        _charsToHeal.Clear();
    }

    private void EnemyAttack()
    {
        if (_charsToMelee.Count > 0)
        {
            int index = 0;
            if (_charsToMelee.Count != 1)
                index = UnityEngine.Random.Range(0, _charsToMelee.Count);

            _hs[_chars.IndexOf(_charsToMelee[index])].TakeDamage(_activeChar.GetData().meleeDamage);
            OnEnemyAttacked?.Invoke(true);
            return;
        }
        else
        {
            if (_charsToRange.Count > 0)
            {
                int index = 0;
                if (_charsToRange.Count != 1)
                    index = UnityEngine.Random.Range(0, _charsToRange.Count);

                _hs[_chars.IndexOf(_charsToRange[index])].TakeDamage(_activeChar.GetData().distanceDamage);
                OnEnemyAttacked?.Invoke(true);
                return;
            }
            else
                Debug.Log("No one near to attack...");
        }
        OnEnemyAttacked?.Invoke(false);
    }

    private bool GetCharactersInDistance(int maxDistance, List<Character> list, bool isLookingForEnemies)
    {
        list.Clear();

        foreach (Character item in _chars)
        {
            if (item.Equals(_activeChar) || !item.IsAlive()) continue;

            bool areBothPlayers = _activeChar.GetData().isPlayer == item.GetData().isPlayer;

            if (isLookingForEnemies && areBothPlayers) continue;
            if (!isLookingForEnemies && !areBothPlayers) continue;

            int distX = Mathf.Abs(_activeChar.GetPosition().x - item.GetPosition().x);
            int distY = Mathf.Abs(_activeChar.GetPosition().y - item.GetPosition().y);
            int distance = Mathf.Max(distX, distY);

            if (distance > 0 && distance <= maxDistance)
                list.Add(item);
        }

        return list.Count > 0;
    }

    public void AttackMelee(Character attacker, Character attacked) => _hs[_chars.IndexOf(attacked)].TakeDamage(attacker.GetData().meleeDamage);
    public void AttackRanged(Character attacker, Character attacked) => _hs[_chars.IndexOf(attacked)].TakeDamage(attacker.GetData().distanceDamage);
    public void Heal(Character healer, Character healed) => _hs[_chars.IndexOf(healed)].Heal(healer.GetData().healingPoints);
}