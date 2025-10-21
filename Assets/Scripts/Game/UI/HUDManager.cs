using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using Zenject;

public class HUDManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI resourcesText;
    [SerializeField] private TextMeshProUGUI waveText;
    [SerializeField] private TextMeshProUGUI enemiesText;
    [SerializeField] private TextMeshProUGUI timerText;

    [Inject] private EconomyManager _economyManager;
    [Inject] private WaveManager _waveManager;
    
    private Coroutine _timerCoroutine;

    private void OnEnable()
    {
        _economyManager.OnResourcesChanged += UpdateResourcesUI;
        _waveManager.OnWaveStarted += UpdateWaveUI;
        _waveManager.OnWaveEnded += UpdateWaveUI;
        _waveManager.OnEnemiesCountChanged += UpdateEnemiesUI;
        _waveManager.OnWavePauseStarted += StartCountdown;
        
        UpdateResourcesUI(_economyManager.Resources);
        UpdateWaveUI(_waveManager.CurrentWave,_waveManager.WavesCount);
        UpdateEnemiesUI(_waveManager.EnemiesAlive);
    }

    private void OnDisable()
    {
        _economyManager.OnResourcesChanged -= UpdateResourcesUI;
        _waveManager.OnWaveStarted -= UpdateWaveUI;
        _waveManager.OnWaveEnded -= UpdateWaveUI;
        _waveManager.OnEnemiesCountChanged -= UpdateEnemiesUI;
        _waveManager.OnWavePauseStarted -= StartCountdown;
    }

    private void UpdateResourcesUI(int resources)
    {
        resourcesText.text = $"{resources}";
    }

    private void UpdateWaveUI(int wave,int wavesCount)
    {
        waveText.text = $"Wave: {wave}/{wavesCount}";
    }

    private void UpdateEnemiesUI(int enemies)
    {
        enemiesText.text = $"{enemies}";
    }
    
    private async void StartCountdown(float duration)
    {
        float remaining = duration;
        timerText.gameObject.SetActive(true);
        while (remaining > 0)
        {
            timerText.text = $"Next wave in: {Mathf.CeilToInt(remaining)}";
            await UniTask.NextFrame();
            remaining -= Time.deltaTime;
        }
        timerText.text = "";
        timerText.gameObject.SetActive(false);
    }
}
