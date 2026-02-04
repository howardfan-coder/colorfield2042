using UnityEngine;

namespace Core.CelesteLikeMovement
{
    [CreateAssetMenu(fileName = "PlayerConfig", menuName = "Celeste/PlayerConfig")]
    public class PlayerConfig : ScriptableObject
    {
        [Header("Collision Box (Relative to Position)")]
        // 将原文件中硬编码的数值搬过来作为默认值
        public Rect normalHitbox = new Rect(0, 0.23f, 0.65f, 0.55f);
        public Rect duckHitbox = new Rect(0, 0.18f, 0.65f, 0.5f);
        
        [Header("Hurt Box")]
        public Rect normalHurtbox = new Rect(0f, -0.15f, 0.65f, 0.9f);
        public Rect duckHurtbox = new Rect(8f, 4f, 0.65f, 0.4f);

        [Header("Raycast Settings")]
        public float platformProbe = 0.2f; // 原 CONST PLATFORM_PROBE
        
        [Header("Constants Tuning")]
        // 如果想配置 Constants 中的值，也可以搬过来
        public float wallSpeedRetentionTime = 0.06f; 
    }
}