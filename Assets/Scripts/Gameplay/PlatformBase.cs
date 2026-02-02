using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;

public class Platform : MonoBehaviour
{
    private Tilemap tilemap;
    private TilemapCollider2D tilemapCollider2D;
    private EventCenter eventCenter;
    public ColorType colorType;
    private void Awake()
    {
        eventCenter = EventCenter.Instance;
    }
    // Start is called before the first frame update
    void Start()
    {
        tilemap = gameObject.GetComponent<Tilemap>();
        tilemapCollider2D = gameObject.GetComponent<TilemapCollider2D>();
        eventCenter.AddEventListener(EventType.ColorChange, _onColorChanged);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void _onColorChanged(object info)
    {
        ColorType colorType = (ColorType)Enum.ToObject(typeof(ColorType), info);
        if (this.colorType == colorType)
        {
            tilemap.color = new Color(tilemap.color.r, tilemap.color.g, tilemap.color.b, 100);
        }
        else
        {
            tilemap.color = new Color(tilemap.color.r, tilemap.color.g, tilemap.color.b, 255);
        }
    }
}
