using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class TooltipManager : MonoBehaviour
{
    [SerializeField] private Canvas tooltipCanvas;
    [SerializeField] private RectTransform tooltipPanel;
    [SerializeField] private TextMeshProUGUI tooltipText;
    [SerializeField] private LineRenderer radiusVisualizer;
    [SerializeField] private LayerMask towerLayer;

    private ITooltip _currentProvider;
    private CancellationTokenSource _cts;

    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
        if (tooltipPanel != null) tooltipPanel.gameObject.SetActive(false);
        if (radiusVisualizer != null) radiusVisualizer.gameObject.SetActive(false);
        _cts = new CancellationTokenSource();
        _ = TooltipLoopAsync();
    }

    private async UniTask TooltipLoopAsync()
    {
        while (!_cts.IsCancellationRequested)
        {
            if (Input.GetKey(KeyCode.LeftAlt))
            {
                Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, towerLayer))
                {
                    _currentProvider = hit.collider.GetComponent<ITooltip>();
                    if (_currentProvider != null)
                    {
                        ShowTooltip(_currentProvider);
                    }
                }
                else
                {
                    HideTooltip();
                }
            }
            else
            {
                HideTooltip();
            }
            await UniTask.Yield(cancellationToken: _cts.Token);
        }
    }

    private void ShowTooltip(ITooltip provider)
    {
        if (provider.GetTooltipText() != "")
        {
            tooltipText.text = provider.GetTooltipText();
            tooltipPanel.gameObject.SetActive(true);

            tooltipPanel.position = Input.mousePosition + new Vector3(20f, 20f, 0f);
        }


        if (provider.GetAttackRadius() != 0)
        {
            float radius = provider.GetAttackRadius();
            Vector3 position = provider.GetPosition();
            DrawRadiusCircle(position, radius);
        }
    }

    private void HideTooltip()
    {
        tooltipPanel.gameObject.SetActive(false);
        radiusVisualizer.gameObject.SetActive(false);
        _currentProvider = null;
    }

    private void DrawRadiusCircle(Vector3 center, float radius)
    {
        radiusVisualizer.gameObject.SetActive(true);
        radiusVisualizer.positionCount = 50;
        float angleStep = 360f / 50;
        for (int i = 0; i < 50; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            Vector3 point = center + new Vector3(Mathf.Cos(angle) * radius, 0.5f, Mathf.Sin(angle) * radius);
            radiusVisualizer.SetPosition(i, point);
        }
    }

    private void OnDestroy()
    {
        _cts?.Cancel();
    }
}