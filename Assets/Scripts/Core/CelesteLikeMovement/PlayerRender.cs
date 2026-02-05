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

        [Header("Effects")]
        [SerializeField]
        public ParticleSystem vfxRunDust; // 跑步/落地/起跳共用的尘土粒子

        private Vector2 scale;
        private Vector2 currSpriteScale;

        public Vector3 SpritePosition { get => this.spriteRenderer.transform.position; }

        [Header("Sprite Animation")]
        [SerializeField] private SpriteAnimationClip[] clips;
        private readonly Dictionary<string, SpriteAnimationClip> clipMap = new();
        private string currentClip;
        private int currentFrame;
        private int frameDirection = 1;
        private float frameTimer;
        private SpriteAnimationClip activeClip;

        public void Reload()
        {
            clipMap.Clear();
            SpriteAnimationClip firstValid = null;
            foreach (var clip in clips)
            {
                if (clip == null || string.IsNullOrEmpty(clip.name) || clip.frames == null || clip.frames.Length == 0)
                    continue;
                clipMap[clip.name] = clip;
                firstValid ??= clip;
            }
            if (firstValid != null)
            {
                PlayClip(firstValid.name, true);
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
            SpriteAnimationClip clip = activeClip;
            if (clip == null || clip.frames == null || clip.frames.Length == 0)
            {
                clip = clips != null && clips.Length > 0 ? clips[0] : null;
            }
            if (clip == null || clip.frames == null || clip.frames.Length == 0)
                return;

            int clamped = Mathf.Clamp(index, 0, clip.frames.Length - 1);
            currentFrame = clamped;
            frameTimer = 0f;
            spriteRenderer.sprite = clip.frames[clamped];
            activeClip = clip;
            currentClip = clip.name;
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
            frameDirection = 1;
            frameTimer = 0f;
            spriteRenderer.sprite = activeClip.frames[0];
        }

        // 新增/修改：播放一次性的移动效果（落地/起跳）
        // isLand: true=落地, false=起跳
        public void PlayMoveEffect(bool isLand, Color color)
        {
            if (vfxRunDust == null) return;

            // 设置颜色
            var main = vfxRunDust.main;
            main.startColor = color;

            // 根据是落地还是起跳，可以调整发射数量或速度，这里简单发射
            int count = isLand ? 5 : 3;
            vfxRunDust.Emit(count);
        }

        // 新增/修改：持续更新移动效果（跑步时）
        // 如果 color 是 clear，则表示不播放
        public void UpdateMoveEffect(Color color)
        {
            if (vfxRunDust == null) return;

            if (color.a <= 0.01f) // Color.clear
            {
                // 如果不想持续发射，什么都不做，或者 Stop() 如果是 Loop 的
                // 这里假设我们用 Emit 方式手动控制，所以不需要 Stop
                return;
            }

            // 简单的计时器控制发射频率，避免每帧发射太多
            // 或者直接利用 ParticleSystem 的 Emission 模块，这里演示手动 Emit
            if (Random.value < 0.2f) // 简单模拟频率
            {
                var main = vfxRunDust.main;
                main.startColor = color;
                vfxRunDust.Emit(1);
            }
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

                if (activeClip.pingPong)
                {
                    currentFrame += frameDirection;
                    if (currentFrame >= activeClip.frames.Length)
                    {
                        currentFrame = activeClip.frames.Length - 1;
                        frameDirection = -1;
                    }
                    else if (currentFrame < 0)
                    {
                        currentFrame = 0;
                        frameDirection = 1;
                    }
                }
                else
                {
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
        public bool pingPong = false;
    }
 }
