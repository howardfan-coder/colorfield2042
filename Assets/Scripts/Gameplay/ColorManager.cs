using UnityEngine;

public class ColorManager : MonoBehaviour
{
    [Header("Visual")]
    public SpriteRenderer spriteRenderer;

    public float decayDuration = 3f;

    private ColorType currentColorType = ColorType.WHITE;
    private float decayTimer;
    private EventCenter eventCenter;

    // 新增：运行时使用的材质（clone 出来）
    private Material runtimeMat;

    // 快消失时闪烁参数
    [Range(0.05f, 0.5f)]
    public float flashThreshold = 0.3f;   // 剩余比例小于这个开始特殊表现
    public float flashSpeed = 8f;         // 闪烁/变化频率

    void Awake()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        if (spriteRenderer != null)
        {
            // clone 一份材质，避免修改到 sharedMaterial
            runtimeMat = Instantiate(spriteRenderer.sharedMaterial);
            spriteRenderer.material = runtimeMat;
        }
        else
        {
            Debug.LogError("[ColorManager] SpriteRenderer not found on player.");
        }

        eventCenter = EventCenter.Instance;
        eventCenter.EventTriger(EventType.ColorChange, (object)currentColorType);
        eventCenter.AddEventListener(EventType.PlayerDeath, _onPlayerDeath);

        ResetColor();
    }

    void Update()
    {
        UpdateVisualColor();
    }

    public void PickColor(ColorType colorType)
    {
        currentColorType = colorType;
        decayTimer = decayDuration;

        ApplyVisualColor(1f);
        eventCenter.EventTriger(EventType.ColorChange, (object)currentColorType);
    }

    private void UpdateVisualColor()
    {
        if (currentColorType == ColorType.WHITE)
            return;

        decayTimer -= Time.deltaTime;
        float t = Mathf.Clamp01(decayTimer / decayDuration);

        ApplyVisualColor(t);

        if (t <= 0f)
        {
            ResetColor();
        }
    }

    private void _onPlayerDeath(object info)
    {
        ResetColor();
    }

    /// <summary>
    /// t: 0~1, 1=刚拿到颜色，0=马上消失
    /// </summary>
    private void ApplyVisualColor(float t)
    {
        if (runtimeMat == null)
            return;

        Color baseColor = ColorVisualMap.GetBaseColor(currentColorType);

        // 使用 HSV 色彩空间
        Color.RGBToHSV(baseColor, out float h, out float s, out float v);

        // 策略调整：二段式衰减
        // 阶段1：缓慢衰减期 (t > 0.2)，颜色从 100% 缓慢降至 80%。
        // 阶段2：快速消散期 (t <= 0.2)，颜色从 80% 快速归零。
        const float FADE_THRESHOLD = 0.2f;
        const float TRANSITION_SATURATION = 0.5f; // 进入最后阶段前，保留约 50% 的饱和度

        float finalT;
        if (t > FADE_THRESHOLD)
        {
            // 归一化前半段时间 (0 ~ 1)
            float progress = (t - FADE_THRESHOLD) / (1f - FADE_THRESHOLD);
            // 线性插值：饱和度 0.5 -> 1.0
            finalT = Mathf.Lerp(TRANSITION_SATURATION, 1f, progress);
        }
        else
        {
            // 归一化后半段时间 (0 ~ 1)
            float progress = t / FADE_THRESHOLD;
            // 平滑插值：饱和度 0.0 -> 0.5
            finalT = Mathf.Lerp(0f, TRANSITION_SATURATION, Mathf.SmoothStep(0f, 1f, progress));
        }

        // 在 HSV 空间插值
        float newS = Mathf.Lerp(0f, s, finalT);
        float newV = Mathf.Lerp(1f, v, finalT);
        Color finalColor = Color.HSVToRGB(h, newS, newV);

        // 把最终颜色传给 shader（假设 shader 用 _BaseColor / _Color）
        // 如果你是自定义的 Player Shader，可以改成你自己定义的属性名
        runtimeMat.SetColor("_BaseColor", finalColor);
        // 如果用的是默认 Sprite Shader，也可以用：
        // runtimeMat.SetColor("_Color", finalColor);

        // ===== 快消失时的特殊视觉表现 =====
        // t 越小，越接近 0，说明越接近消失
        if (t < flashThreshold)
        {
            float normalized = 1f - (t / flashThreshold); // 0~1
            float flash = Mathf.PingPong(Time.time * flashSpeed, 1f) * normalized;
            runtimeMat.SetFloat("_FlashStrength", flash);
        }
        else
        {
            runtimeMat.SetFloat("_FlashStrength", 0f);
        }
    }

    private void ResetColor()
    {
        currentColorType = ColorType.WHITE;
        decayTimer = 0f;

        if (runtimeMat != null)
        {
            runtimeMat.SetColor("_BaseColor", Color.white);
            runtimeMat.SetFloat("_FlashStrength", 0f);
        }

        eventCenter.EventTriger(EventType.ColorChange, (object)currentColorType);
    }
}
