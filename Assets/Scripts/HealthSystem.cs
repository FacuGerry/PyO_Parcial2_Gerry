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
    }

    public void TakeDamage(int damage)
    {
        _currentHealth -= damage;

        if (_currentHealth <= 0)
        {
            _currentHealth = 0;

            OnLifeUpdated?.Invoke(_currentHealth);
            OnCharacterDie?.Invoke(_data);

            gameObject.SetActive(false);
        }
        else
            OnLifeUpdated?.Invoke(_currentHealth);
    }
}