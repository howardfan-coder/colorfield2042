using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Core.CelesteLikeMovement
{
    public enum Facings
    {
        Right = 1,
        Left = -1
    }

    public struct VirtualIntegerAxis
    {

    }
    public struct VirtualJoystick
    {
        private readonly string stickX;
        private readonly string stickY;
        private readonly string dpadX;
        private readonly string dpadY;
        private readonly float deadZone;

        public VirtualJoystick(string stickX = "Horizontal", string stickY = "Vertical", string dpadX = "DPadX", string dpadY = "DPadY", float deadZone = 0.1f)
        {
            this.stickX = stickX;
            this.stickY = stickY;
            this.dpadX = dpadX;
            this.dpadY = dpadY;
            this.deadZone = deadZone;
        }

        private static float SafeAxis(string axisName)
        {
            if (string.IsNullOrEmpty(axisName))
                return 0f;
            try
            {
                return UnityEngine.Input.GetAxisRaw(axisName);
            }
            catch (ArgumentException)
            {
                return 0f;
            }
        }

        public Vector2 Value
        {
            get
            {
                float x = SafeAxis(stickX);
                float y = SafeAxis(stickY);

                x = Mathf.Abs(x) > deadZone ? x : 0f;
                y = Mathf.Abs(y) > deadZone ? y : 0f;

                float dx = SafeAxis(dpadX);
                float dy = SafeAxis(dpadY);

                if (Mathf.Abs(dx) > deadZone) x = dx;
                if (Mathf.Abs(dy) > deadZone) y = dy;

                return new Vector2(x, y);
            }
        }


    }
    public struct VisualButton
    {
        private KeyCode key;
        private float bufferTime;
        private bool consumed;
        private float bufferCounter;
        public VisualButton(KeyCode key) : this(key, 0) {
        }

        public VisualButton(KeyCode key, float bufferTime)
        {
            this.key = key;
            this.bufferTime = bufferTime;
            this.consumed = false;
            this.bufferCounter = 0f;
        }
        public void ConsumeBuffer()
        {
            this.bufferCounter = 0f;
        }

        public bool Pressed()
        {
            return UnityEngine.Input.GetKeyDown(key)||(!this.consumed && (this.bufferCounter > 0f));
        }

        public bool Checked()
        {
            return UnityEngine.Input.GetKey(key);
        }

        public void Update(float deltaTime)
        {
            this.consumed = false;
            this.bufferCounter -= deltaTime;
            bool flag = false;
            if (UnityEngine.Input.GetKeyDown(key))
            {
                this.bufferCounter = this.bufferTime;
                flag = true;
            }
            else if (UnityEngine.Input.GetKey(key))
            {
                flag = true;
            }
            if (!flag)
            {
                this.bufferCounter = 0f;
                return;
            }
        }
    }
    public static class GameInput
    {
        // PS5 DualSense: Cross=JoystickButton0, Square=JoystickButton2, L2=JoystickButton6
        public static VisualButton Jump = new VisualButton(KeyCode.JoystickButton0, 0.08f);
        public static VisualButton Dash = new VisualButton(KeyCode.JoystickButton2, 0.08f);
        public static VisualButton Grab = new VisualButton(KeyCode.JoystickButton6);
        public static VirtualJoystick Aim = new VirtualJoystick();
        public static Vector2 LastAim;

        //根据当前朝向,决定移动方向.
        public static Vector2 GetAimVector(Facings defaultFacing = Facings.Right)
        {
            Vector2 value = GameInput.Aim.Value;
            if (value == Vector2.zero)
            {
                GameInput.LastAim = Vector2.right * ((int)defaultFacing);
            }
            else
            {
                GameInput.LastAim = value;
            }
            return GameInput.LastAim.normalized;
        }

        public static void Update(float deltaTime)
        {
            Jump.Update(deltaTime);
            Dash.Update(deltaTime);
        }
    }




}
