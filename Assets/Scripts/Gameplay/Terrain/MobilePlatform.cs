using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobilePlatform : MonoBehaviour
{
    public Transform startPoint;
    public Transform endPoint;
    public float speed;
    public EventCenter eventCenter;
    public SpriteRenderer sprite;
    public BoxCollider2D collider;
    public ColorType colorType;
    public float tolerance = 0.01f;

    private Vector2 _target;
    private Vector2 _start;
    private Vector2 _end;
    private void Awake()
    {
        eventCenter = EventCenter.Instance;
        transform.position = startPoint.position;
    }
    // Start is called before the first frame update
    void Start()
    {
        sprite = gameObject.GetComponent<SpriteRenderer>();
        collider = gameObject.GetComponent<BoxCollider2D>();
        eventCenter.AddEventListener(EventType.ColorChange, OnColorChanged);
        _start = new Vector2(startPoint.position.x, startPoint.position.y);
        _end = new Vector2(endPoint.position.x, endPoint.position.y);
        _target = _end;
        sprite.color = ColorVisualMap.GetBaseColor(colorType);
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 pos = new Vector2(transform.position.x, transform.position.y);
        Vector2 next = Vector2.MoveTowards(pos, _target, speed * Time.deltaTime);
        transform.position = new Vector3(next.x, next.y, transform.position.z);

        if (Vector2.Distance(next, _target) <= tolerance)
        {
            _target = (_target == _end) ? _start : _end;
        }
    }

    void OnColorChanged(object info)
    {
        ColorType type = (ColorType)Enum.ToObject(typeof(ColorType), info);
        ChangespriteState(type);
    }

    public virtual void ChangespriteState(ColorType type)
    {
        if (colorType == type && colorType != ColorType.WHITE)
        {
            sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 1);
            collider.enabled = true;
        }
        else
        {
            sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 0.25f);
            collider.enabled = false;
        }
    }
}
