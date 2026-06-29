using System;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    public event Action<int> OnLifeUpdated; // _currentHealth
    public event Action<CharacterDataSO> OnCharacterDie; // _data

    [SerializeField] private CharacterDataSO _data;
    private int _currentHealth;

    private void Start()
    {
        _currentHealth = _data.maxLife;
        OnLifeUpdated?.Invoke(_currentHealth);
    }

    public void TakeDamage(int damage)
    {
        if (_currentHealth <= 0) return;

        _currentHealth -= damage;

        if (_currentHealth <= 0)
        {
            _currentHealth = 0;

            OnLifeUpdated?.Invoke(_currentHealth);
            OnCharacterDie?.Invoke(_data);

            Debug.Log($"{_data.characterName} died");

            gameObject.SetActive(false);
        }
        else
            OnLifeUpdated?.Invoke(_currentHealth);
    }

    public void Heal(int heal)
    {
        _currentHealth += heal;
        if (_currentHealth >= _data.maxLife)
            _currentHealth = _data.maxLife;

        OnLifeUpdated?.Invoke(_currentHealth);
        Debug.Log($"Healed {_data.characterName}");
    }
}