using UnityEngine;

namespace Core.CelesteLikeMovement
{

    /// <summary>
    /// Controller关于表现相关
    /// </summary>
    public partial class PlayerController 
    {
        private Vector2 cameraPosition;

        private Bounds bounds;

        protected void UpdateCamera(float deltaTime)
        {
            var from = cameraPosition;
            var target = CameraTarget;
            var multiplier = 1f;

            cameraPosition = from + (target - from) * (1f - (float)Mathf.Pow(0.01f / multiplier, deltaTime));
        }

        public Vector2 GetCameraPosition() 
        {
            return cameraPosition;
        }

        //protected Vector2 CameraTarget
        //{
        //    get
        //    {
        //        Vector2 at = new Vector2();
        //        Vector2 target = new Vector2(this.Position.x, this.Position.y);

        //        at.x = Mathf.Clamp(target.x, bounds.min.x + 3200 / 100 / 2f, bounds.max.x - 3200 / 100 / 2f);
        //        at.y = Mathf.Clamp(target.y, bounds.min.y + 1800 / 100 / 2f, bounds.max.y - 1800 / 100 / 2f);
        //        return at;
        //    }
        //}
        protected Vector2 CameraTarget
        {
            get
            {
                Camera cam = Camera.main;
                float halfHeight = 9f; // 默认备用值 (如果找不到摄像机)
                float halfWidth = 16f;

                if (cam != null)
                {
                    halfHeight = cam.orthographicSize; // 摄像机正交大小（半高）
                    halfWidth = halfHeight * cam.aspect; // 根据宽高比计算半宽
                }

                Vector2 at = new Vector2();
                Vector2 target = new Vector2(this.Position.x, this.Position.y);

                // 计算允许摄像机移动的最小和最大边界
                float minX = bounds.min.x + halfWidth;
                float maxX = bounds.max.x - halfWidth;
                float minY = bounds.min.y + halfHeight;
                float maxY = bounds.max.y - halfHeight;

                // 如果关卡本身比摄像机视野还小，则强制居中显示
                // 否则在计算出的范围内进行 Clamp
                if (minX > maxX)
                    at.x = bounds.center.x;
                else
                    at.x = Mathf.Clamp(target.x, minX, maxX);

                if (minY > maxY)
                    at.y = bounds.center.y;
                else
                    at.y = Mathf.Clamp(target.y, minY, maxY);

                return at;
            }
        }
    }


}