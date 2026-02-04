using System;
using UnityEngine;

namespace Core.CelesteLikeMovement
{
    /// <summary>
    /// 玩家类：包含
    /// 1、玩家显示器
    /// 2、玩家控制器（核心控制器）
    /// 并允许两者在内部进行交互
    /// </summary>
    public class Player
    {
        private PlayerRenderer playerRenderer;
        private PlayerController playerController;

        private IGameContext gameContext;

        private Bounds bounds;
        private Vector2 startPosition;

        private PlayerConfig config; // Player类需要缓存一下config以便重生使用

        public Player(IGameContext gameContext)
        {
            this.gameContext = gameContext;
        }

        //加载玩家实体
        public void Reload(Bounds bounds, Vector2 startPosition, PlayerConfig config)
        {
            this.bounds = bounds;
            this.startPosition = startPosition;
            this.config = config;

            try
            {
                this.playerRenderer = UnityEngine.Object.Instantiate(Resources.Load<PlayerRenderer>("PlayerRenderer"));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            this.playerRenderer.Reload();
            //初始化
            this.playerController = new PlayerController(playerRenderer, gameContext.EffectControl);
            this.playerController.Init(bounds, startPosition, config);

            PlayerParams playerParams = Resources.Load<PlayerParams>("PlayerParam");
            playerParams.SetReloadCallback(() => this.playerController.RefreshAbility());
            playerParams.ReloadParams();
        }

        public void Update(float deltaTime)
        {
            if (playerController.CheckDeadGround())
            {
                EventCenter.Instance.EventTriger(EventType.PlayerDeath, null);
                playerController.Init(this.bounds, this.startPosition, this.config);
            }

            playerController.Update(deltaTime);
            Render();
        }

        private void Render()
        {
            playerRenderer.Render(Time.deltaTime);

            Vector2 scale = playerRenderer.transform.localScale;
            scale.x = Mathf.Abs(scale.x) * (int)playerController.Facing;
            playerRenderer.transform.localScale = scale;
            playerRenderer.transform.position = playerController.Position;

            //if (!lastFrameOnGround && this.playerController.OnGround)
            //{
            //    this.playerRenderer.PlayMoveEffect(true, this.playerController.GroundColor);
            //}
            //else if (lastFrameOnGround && !this.playerController.OnGround)
            //{
            //    this.playerRenderer.PlayMoveEffect(false, this.playerController.GroundColor);
            //}
            //this.playerRenderer.UpdateMoveEffect();

            this.lastFrameOnGround = this.playerController.OnGround;
        }

        private bool lastFrameOnGround;

        public Vector2 GetCameraPosition()
        {
            if (this.playerController == null)
            {
                return Vector3.zero;
            }
            return playerController.GetCameraPosition();
        }
    }

}
