using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class HealthBar : MonoBehaviour
{
    [Inject] private GameStateMachine _gameState;
    
    [SerializeField]private Slider healthSlider;

    private IDamageable _damageable;

    private void Awake()
    {
        _damageable = GetComponentInParent<IDamageable>();
    }

    private async void Start()
    {
        if (healthSlider != null && _damageable != null)
        {
            await UniTask.WaitUntil(() => _gameState.CurrentState == GameState.Playing);
            SetStartValues();
            UpdateHealthUI();
            _damageable.OnHealthChange += UpdateHealthUI;
        }
    }

    private void SetStartValues()
    {
        healthSlider.maxValue = _damageable.Health;
        healthSlider.value = _damageable.Health;
    }

    private void UpdateHealthUI()
    {
        healthSlider.value = _damageable.Health;
    }

    private void OnDestroy()
    {
        if (_damageable != null)
            _damageable.OnHealthChange -= UpdateHealthUI;
    }
}
