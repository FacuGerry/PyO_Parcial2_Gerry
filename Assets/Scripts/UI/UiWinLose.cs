using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UiWinLose : MonoBehaviour
{
    [SerializeField] private GameController _controller;
    [SerializeField] private GameObject _canvas;
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private Button _btn;

    [Header("Texts")]
    [SerializeField] private string _sceneName;
    [SerializeField] private string _loseText;

    private bool _hasToogled = false;

    private void OnEnable()
    {
        _controller.OnPlayersLose += OnLose_LoadPanel;
        _controller.OnWin += OnWin_LoadPanel;
    }

    private void Start()
    {
        _btn.onClick.AddListener(ReLoadScene);
    }

    private void OnDisable()
    {
        _controller.OnPlayersLose -= OnLose_LoadPanel;
        _controller.OnWin -= OnWin_LoadPanel;
    }

    private void OnDestroy()
    {
        _btn.onClick.RemoveAllListeners();
    }

    private void OnLose_LoadPanel()
    {
        if (_hasToogled) return;
        LoadPanel();
        _text.text = _loseText;
        _hasToogled = true;
    }

    private void OnWin_LoadPanel(Character winner)
    {
        if (_hasToogled) return;
        LoadPanel();
        _text.text = string.Format($"{winner.GetData().characterName} has won!!!");
        _hasToogled = true;
    }

    private void LoadPanel() => _canvas.gameObject.SetActive(true);

    private void ReLoadScene() => SceneManager.LoadScene(_sceneName);
}