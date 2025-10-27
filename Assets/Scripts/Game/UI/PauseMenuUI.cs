using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class PauseMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private TextMeshProUGUI pauseText;

    [Inject] private GameStateMachine _stateMachine;

    private void Awake()
    {
        pauseButton.onClick.AddListener(OnPauseClicked);
        resumeButton.onClick.AddListener(OnResumeClicked);
        restartButton.onClick.AddListener(OnRestartClicked);
        quitButton.onClick.AddListener(OnQuitClicked);

        pausePanel.SetActive(false);
    }

    private void OnEnable()
    {
        _stateMachine.OnStateChanged += OnStateChanged;
    }

    private void OnDisable()
    {
        _stateMachine.OnStateChanged -= OnStateChanged;
    }

    private async void OnPauseClicked()
    {
        await _stateMachine.EnterStateAsync(GameState.Paused);
    }

    private async void OnResumeClicked()
    {
        await _stateMachine.EnterStateAsync(GameState.Playing);
    }

    private void OnRestartClicked()
    {
        _stateMachine.Restart();
    }

    private void OnQuitClicked()
    {
        _stateMachine.QuitToMenu();
    }

    private void OnStateChanged(GameState state)
    {
        pausePanel.SetActive(state == GameState.Paused || state == GameState.Victory || state == GameState.GameOver);
        resumeButton.gameObject.SetActive(state == GameState.Paused);
        pauseText.text = state switch
        {
            GameState.Paused => "PAUSED",
            GameState.GameOver => "GAME OVER",
            GameState.Victory => "VICTORY",
            _ => ""
        };
    }
}
