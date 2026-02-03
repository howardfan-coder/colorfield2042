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

    private void ApplyVisualColor(float t)
    {
        Color baseColor = ColorVisualMap.GetBaseColor(currentColorType);
        spriteRenderer.color = Color.Lerp(Color.white, baseColor, t);
    }

    private void ResetColor()
    {
        currentColorType = ColorType.WHITE;
        spriteRenderer.color = Color.white;

        eventCenter.EventTriger(EventType.ColorChange, (object)currentColorType);
    }
}