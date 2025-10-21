using UnityEngine;
using Zenject;

public class testMono : MonoBehaviour
{
    [Inject] private WaveManager _waveManager;

    private void Start()
    {
        StartWave();
    }

    private async void StartWave()
    {
        await _waveManager.StartWaveAsync();
    }

}
