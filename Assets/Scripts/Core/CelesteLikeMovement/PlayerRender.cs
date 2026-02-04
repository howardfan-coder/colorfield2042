// using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.CelesteLikeMovement
{

    /// <summary>
    /// 这里是Unity下实现玩家表现接口
    /// </summary>
    public class PlayerRenderer : MonoBehaviour, ISpriteControl
    {
        [SerializeField]
        public SpriteRenderer spriteRenderer;

        [SerializeField]
        public ParticleSystem vfxDashFlux;
        [SerializeField]
        public ParticleSystem vfxWallSlide;

        [SerializeField]
        public TrailRenderer hair;

        [SerializeField]
        public SpriteRenderer hairSprite01;
        [SerializeField]
        public SpriteRenderer hairSprite02;

        private Vector2 scale;
        private Vector2 currSpriteScale;

        public Vector3 SpritePosition { get => this.spriteRenderer.transform.position; }

        [Header("Sprite Animation")]
        [SerializeField] private SpriteAnimationClip[] clips;
        private readonly Dictionary<string, SpriteAnimationClip> clipMap = new();
        private string currentClip;
        private int currentFrame;
        private float frameTimer;
        private SpriteAnimationClip activeClip;

        public void Reload()
        {
            clipMap.Clear();
            foreach (var clip in clips)
            {
                if (clip == null || string.IsNullOrEmpty(clip.name) || clip.frames == null || clip.frames.Length == 0)
                    continue;
                clipMap[clip.name] = clip;
            }
            if (clipMap.Count > 0)
            {
                var first = clips[0];
                PlayClip(first.name, true);
            }
        }

        public void Render(float deltaTime)
        {
            float tempScaleX = Mathf.MoveTowards(scale.x, currSpriteScale.x, 1.75f * deltaTime);
            float tempScaleY = Mathf.MoveTowards(scale.y, currSpriteScale.y, 1.75f * deltaTime);
            this.scale = new Vector2(tempScaleX, tempScaleY);
            this.spriteRenderer.transform.localScale = scale;

            UpdateAnimation(deltaTime);
        }

        public void SetSpriteIndex(int index)
        {
            //TODO Change sprite based on index
        }


        public void Trail(int face)
        {
            // SceneEffectManager.Instance.Add(this.spriteRenderer, face, Color.white);
        }

        public void Scale(Vector2 scale)
        {
            this.scale = scale;
        }

        public void SetSpriteScale(Vector2 scale)
        {
            this.currSpriteScale = scale;
        }

        public void DashFlux()
        {

        }

        public void Slash(bool enable)
        {
        }

        public void WallSlide(Color color, Vector2 dir)
        {
            this.vfxWallSlide.transform.rotation = Quaternion.FromToRotation(Vector2.up, dir);
            var main = this.vfxWallSlide.main;
            main.startColor = color;
            this.vfxWallSlide.Emit(1);
        }

        public void DashFlux(Vector2 dir, bool play)
        {
            if (play)
            {
                this.vfxDashFlux.transform.rotation = Quaternion.FromToRotation(Vector2.up, dir);
                this.vfxDashFlux.Play();
            }
            else
            {
                this.vfxDashFlux.transform.parent = this.transform;
                this.vfxDashFlux.Stop();
            }
        }

        public void SetHairColor(Color color)
        {
            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new GradientColorKey[] { new GradientColorKey(color, 0.0f), new GradientColorKey(Color.black, 1.0f) },
                new GradientAlphaKey[] { new GradientAlphaKey(1, 0.0f), new GradientAlphaKey(1, 0.6f), new GradientAlphaKey(0, 1.0f) }
            );
            
            //TODO
            // this.hair.colorGradient = gradient;
            // this.hairSprite01.color = color;
            // this.hairSprite02.color = color;
        }

        public void PlayClip(string name, bool restart = false)
        {
            if (!clipMap.TryGetValue(name, out var clip))
                return;

            if (!restart && currentClip == name && activeClip != null)
                return;

            currentClip = name;
            activeClip = clip;
            currentFrame = 0;
            frameTimer = 0f;
            spriteRenderer.sprite = activeClip.frames[0];
        }

        private void UpdateAnimation(float deltaTime)
        {
            if (activeClip == null || activeClip.frames == null || activeClip.frames.Length == 0)
                return;

            frameTimer += deltaTime;
            float frameDuration = 1f / Mathf.Max(1f, activeClip.fps);
            while (frameTimer >= frameDuration)
            {
                frameTimer -= frameDuration;
                currentFrame++;
                if (currentFrame >= activeClip.frames.Length)
                {
                    if (activeClip.loop)
                        currentFrame = 0;
                    else
                    {
                        currentFrame = activeClip.frames.Length - 1;
                        frameTimer = 0f;
                    }
                }
                spriteRenderer.sprite = activeClip.frames[currentFrame];
            }
        }
    }

    [System.Serializable]
    public class SpriteAnimationClip
    {
        public string name;
        public Sprite[] frames;
        public float fps = 8f;
        public bool loop = true;
    }
 }
