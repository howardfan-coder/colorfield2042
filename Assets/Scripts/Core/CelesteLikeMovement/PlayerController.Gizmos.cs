using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace Core.CelesteLikeMovement
{
    public partial class PlayerController
    {
        public void Draw(EGizmoDrawType type)
        {
            switch (type)
            {
                case EGizmoDrawType.SlipCheck:
                    DrawSlipCheck();
                    break;
                case EGizmoDrawType.ClimbCheck:
                    DrawClimbCheck();
                    break;
            }
        }

        private void DrawSlipCheck()
        {
            int direct = Facing == Facings.Right ? 1 : -1;
            {
                Gizmos.color = Color.blue;
                Vector2 origin = this.Position + collider.position + Vector2.up * collider.size.y / 2f + Vector2.right * direct * (collider.size.x / 2f + STEP);
                Vector2 point1 = origin + Vector2.up * (-0.4f + 0.1f);
                Gizmos.DrawWireSphere(point1, 0.1f);

                Gizmos.color = Color.red;
                Vector2 point2 = origin + Vector2.up * (0.4f + 0.1f);
                Gizmos.DrawWireSphere(point2, 0.1f);
            }
        }
        public void DrawAllColliders()
        {
            // // 命中盒：站立/蹲伏 Gizmos.color = Color.green;
            // DrawRectGizmo(normalHitbox, "Hitbox-Stand");
            // Gizmos.color = Color.yellow;
            // DrawRectGizmo(duckHitbox, "Hitbox-Duck");
            //
            // //受伤盒：站立/蹲伏 Gizmos.color = Color.red;
            // DrawRectGizmo(normalHurtbox, "Hurtbox-Stand");
            // Gizmos.color = new Color(1f,0.5f,0.5f,0.8f);
            // DrawRectGizmo(duckHurtbox, "Hurtbox-Duck");

            // 当前实际碰撞盒（随姿态变化）
            Gizmos.color = Color.cyan;
            DrawRectGizmo(collider, "Collider-Active");
        }
        

        private void DrawClimbCheck()
        {
            //Gizmos.color = Color.blue;
            //Vector2 origin = this.Position + 
            //Vector2 point1 = origin + Vector2.up * (-0.4f + 0.1f);
            //Gizmos.DrawWireSphere(point1, 0.1f);
        }

        private void DrawRectGizmo(Rect rect, string label)
         {
            // Physics uses rect.position as BoxCast center, so gizmo mirrors that.
            Vector2 center = Position + rect.position;
            Vector3 size = new Vector3(rect.size.x, rect.size.y, 0f);
            Gizmos.DrawWireCube(center, size);
 #if UNITY_EDITOR
             Handles.Label(center, label);
 #endif
         }
    }
}