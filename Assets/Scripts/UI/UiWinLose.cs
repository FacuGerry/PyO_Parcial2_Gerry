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

    private void OnEnable()
    {
        _controller.OnLose += OnLose_LoadPanel;
        _controller.OnWin += OnWin_LoadPanel;
    }

    private void Start()
    {
        _btn.onClick.AddListener(ReLoadScene);
    }

    private void OnDisable()
    {
        _controller.OnLose -= OnLose_LoadPanel;
        _controller.OnWin -= OnWin_LoadPanel;
    }

    private void OnDestroy()
    {
        _btn.onClick.RemoveAllListeners();
    }

    private void OnLose_LoadPanel()
    {
        LoadPanel();
        _text.text = _loseText;
    }

    private void OnWin_LoadPanel(Character winner)
    {
        LoadPanel();
        _text.text = string.Format($"{winner.GetData().characterName} has won!!!");
    }

    private void LoadPanel() => _canvas.gameObject.SetActive(true);

    private void ReLoadScene() => SceneManager.LoadScene(_sceneName);
}