using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;

public class TilemapBase : MonoBehaviour
{
    public EventCenter eventCenter;

    public Tilemap tilemap;
    public TilemapCollider2D tilemapCollider2D;
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
        eventCenter.AddEventListener(EventType.ColorChange, OnColorChanged);
    }

    // Update is called once per frame
    void Update()
    {
    }

    void OnColorChanged(object info)
    {
        ColorType type = (ColorType)Enum.ToObject(typeof(ColorType), info);
        ChangeTileMapState(type);
    }

    public virtual void ChangeTileMapState(ColorType type) 
    {
        if (colorType == type && colorType != ColorType.WHITE)
        {
            tilemap.color = new Color(tilemap.color.r, tilemap.color.g, tilemap.color.b, 1);
            tilemapCollider2D.enabled = true;
        }
        else
        {
            tilemap.color = new Color(tilemap.color.r, tilemap.color.g, tilemap.color.b, 0.25);
            tilemapCollider2D.enabled = false;
        }
    }
}
