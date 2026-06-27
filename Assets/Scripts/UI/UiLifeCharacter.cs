using TMPro;
using UnityEngine;

public class UiLifeCharacter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private HealthSystem _hs;

    private void OnEnable()
    {
        _hs.OnLifeUpdated += UpdateText;
    }

    private void OnDisable()
    {
        _hs.OnLifeUpdated -= UpdateText;
    }

    private void UpdateText(int life) => _text.text = string.Format($"HP: {life}");
}