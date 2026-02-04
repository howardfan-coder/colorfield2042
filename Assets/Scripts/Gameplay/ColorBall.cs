using UnityEngine;
using Core.CelesteLikeMovement;

public class ColorBall : MonoBehaviour, ICollectable
{
    [Header("Color Type")]
    public ColorType colorType;

    [Header("Respawn")]
    public float respawnDelay = 0.5f;

    private SpriteRenderer spriteRenderer;
    private Collider2D col;
    private ColorManager playerColorManager;
    private EventCenter eventCenter;

    private bool isAvailable = true;

    void Awake()
    {
        eventCenter = EventCenter.Instance;
        spriteRenderer = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();

        UpdateColor();
    }

    void Start()
    {
    }

    private void Consume()
    {
        isAvailable = false;
        spriteRenderer.enabled = false;
        col.enabled = false;
        eventCenter.AddEventListener(EventType.ColorChange, _onColorChanged);
    }

    public void Respawn()
    {
        isAvailable = true;
        spriteRenderer.enabled = true;
        col.enabled = true;
        eventCenter.RemoveEventListenter(EventType.ColorChange, _onColorChanged);
    }

    private void _onColorChanged(object info)
    {
        Invoke(nameof(Respawn), respawnDelay);
    }

    public void OnCollect(PlayerController player)
    {
        if (!isAvailable) return;

        // 尝试从 PlayerRenderer (SpriteControl) 获取 ColorManager
        MonoBehaviour view = player.SpriteControl as MonoBehaviour;
        if (view != null)
        {
            playerColorManager = view.GetComponent<ColorManager>();
            if (playerColorManager != null)
            {
                playerColorManager.PickColor(colorType);
                Consume();
            }
        }
    }

    private void OnDestroy()
    {
        eventCenter.RemoveEventListenter(EventType.ColorChange, _onColorChanged);
    }

    // Called when a value is changed in the Inspector
    private void OnValidate()
    {
        UpdateColor();
    }

    private void UpdateColor()
    {
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        if (sprite != null)
        {
            sprite.color = ColorVisualMap.GetBaseColor(colorType);
        }
    }
}
