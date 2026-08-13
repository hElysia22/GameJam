using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

/// <summary>
/// 风力时间枢纽：玩家进入区域按F切换时间流速档位，冷却后自动恢复正常流速
/// </summary>
public class WindRotateHub : MonoBehaviour
{
    [Header("绑定控制的时间区域")]
    public WindTimeZone targetZone;
    public AreaEffector2D areaEffector;

    [Header("时间档位数组：凝滞 / 正常")]
    public float[] timeScaleGears = { 0.01f, 1f };
    private int _currentGearIndex = 1; // 默认正常流速档位

    [Header("粒子系统")]
    public ParticleSystem ps;
    private ParticleSystem.NoiseModule _noiseModule;

    [Header("凝滞冷却时长")]
    public float freezeCoolDown = 5f;
    private float _coolDownTimer;
    private bool _isFreezeState = false; // 是否处于凝滞状态

    [Header("机关交互提示")]
    public TMP_Text hubTipText;
    public string tipText = "按 F 减缓时间";
    public Vector3 tipOffset = new Vector3(0, 1.1f, 0);

    // 玩家是否在触发框内
    private bool _playerInArea;

    private void Start()
    {
        if (ps != null)
        {
            _noiseModule = ps.noise;
            _noiseModule.enabled = true;
        }
        else
        {
            Debug.LogWarning("未绑定粒子系统 ps！", this);
        }

        // 启动时隐藏提示
        if (hubTipText != null)
            hubTipText.gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            _playerInArea = true;
            Debug.Log("玩家进入风力时间区域");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            _playerInArea = false;
            Debug.Log("玩家离开风力时间区域");
        }
    }

    private void Update()
    {
        // 冷却倒计时
        if (_isFreezeState)
        {
            _coolDownTimer -= Time.deltaTime;
            if (_coolDownTimer <= 0f)
            {
                RecoverNormalTime();
            }
        }

        // 更新机关旁提示显隐与位置
        RefreshHubTip();
    }

    /// <summary>
    /// 机关悬浮提示
    /// </summary>
    private void RefreshHubTip()
    {
        if (hubTipText == null) return;

        bool canShowTip = _playerInArea && !_isFreezeState;
        hubTipText.gameObject.SetActive(canShowTip);

        if (!canShowTip) return;

        // 固定在机关上方偏移位置
        hubTipText.transform.position = transform.position + tipOffset;

        hubTipText.text = tipText;
    }

    /// <summary>
    /// 交互按键按下回调
    /// </summary>
    public void OnInteractPress(InputAction.CallbackContext ctx)
    {
        if (ctx.phase != InputActionPhase.Performed)
            return;

        if (!_playerInArea)
        {
            Debug.Log("玩家不在交互区域，无法操作");
            return;
        }
        if (_isFreezeState)
        {
            Debug.Log("凝滞冷却中，暂时无法切换");
            return;
        }

        SwitchTimeGear();
        EnterFreezeState();
    }

    /// <summary>
    /// 切换时间流速档位
    /// </summary>
    public void SwitchTimeGear()
    {
        _currentGearIndex = (_currentGearIndex + 1) % timeScaleGears.Length;
        float targetScale = timeScaleGears[_currentGearIndex];

        if (targetZone != null)
        {
            targetZone.SetTimeScale(targetScale);
        }
        else
        {
            Debug.LogWarning("未绑定WindTimeZone区域组件！", this);
        }
        Debug.Log($"切换时间档位，区域流速 = {targetScale}");
    }

    /// <summary>
    /// 进入凝滞状态：关闭粒子噪声、开启冷却
    /// </summary>
    private void EnterFreezeState()
    {
        _isFreezeState = true;
        _coolDownTimer = freezeCoolDown;
        _noiseModule.enabled = false;
        areaEffector.forceMagnitude = 0.1f;
    }

    /// <summary>
    /// 冷却结束，恢复正常时间与粒子
    /// </summary>
    private void RecoverNormalTime()
    {
        _isFreezeState = false;
        // 强制切回正常档位索引
        _currentGearIndex = 1;
        areaEffector.forceMagnitude = -15f;
        if (targetZone != null)
            targetZone.SetTimeScale(timeScaleGears[1]);

         _noiseModule.enabled = true;

        Debug.Log("凝滞冷却结束，恢复正常时间流速");
    }
}
