using UnityEngine;
using System.Collections;

public class ColorManager : MonoBehaviour
{
    [Header("Visual")]
    public SpriteRenderer spriteRenderer;

    public float decayDuration = 3f;

    private ColorType currentColorType = ColorType.WHITE;
    private float decayTimer;
    private EventCenter eventCenter;

    void Awake()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }
        eventCenter = EventCenter.Instance;
        eventCenter.EventTriger(EventType.ColorChange, (object)currentColorType);
        eventCenter.AddEventListener(EventType.PlayerDeath, _onPlayerDeath);
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
        if (currentColorType == ColorType.WHITE) return;

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

    private void ApplyVisualColor(float t)
    {
        Color baseColor = ColorVisualMap.GetBaseColor(currentColorType);

        // 使用HSV色彩空间
        Color.RGBToHSV(baseColor, out float h, out float s, out float v);

        // 策略调整：二段式衰减
        // 阶段1：缓慢衰减期 (t > 0.2)，颜色从 100% 缓慢降至 80%。
        //        让颜色一直保持高辨识度，同时有一点点流失感。
        // 阶段2：快速消散期 (t <= 0.2)，颜色从 80% 快速归零。
        //        仅在最后时刻迅速变回白色。
        const float FADE_THRESHOLD = 0.2f;
        const float TRANSITION_SATURATION = 0.5f; // 进入最后阶段前，保留80%的色彩

        float finalT;
        if (t > FADE_THRESHOLD)
        {
            // 归一化前半段时间 (0 ~ 1)
            float progress = (t - FADE_THRESHOLD) / (1f - FADE_THRESHOLD);
            // 线性插值：饱和度 0.8 -> 1.0
            finalT = Mathf.Lerp(TRANSITION_SATURATION, 1f, progress);
        }
        else
        {
            // 归一化后半段时间 (0 ~ 1)
            float progress = t / FADE_THRESHOLD;
            // 平滑插值：饱和度 0.0 -> 0.8
            finalT = Mathf.Lerp(0f, TRANSITION_SATURATION, Mathf.SmoothStep(0f, 1f, progress));
        }

        // 在HSV空间插值
        float newS = Mathf.Lerp(0f, s, finalT);
        float newV = Mathf.Lerp(1f, v, finalT);

        spriteRenderer.color = Color.HSVToRGB(h, newS, newV);
    }

    private void ResetColor()
    {
        currentColorType = ColorType.WHITE;
        spriteRenderer.color = Color.white;

        eventCenter.EventTriger(EventType.ColorChange, (object)currentColorType);
    }
}