using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace uRetroEngine
{
    /// <summary>
    /// Input manager
    /// </summary>
    public static class uRetroInput
    {
        public static int mouse_x = 0;
        public static int mouse_y = 0;
        public static bool isInside = false;
        private static Vector2 temp = new Vector2(0, 0);

        /// <summary>
        ///
        /// </summary>
        /// <param name="keyCode"></param>
        /// <returns></returns>

        public static bool ButtonDown(KeyCode keyCode)
        {
            bool res = false;

            if (Input.GetKeyDown(keyCode)) res = true;

            return res;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="keyCode"></param>
        /// <returns></returns>
        public static bool ButtonHold(KeyCode keyCode)
        {
            bool res = false;

            if (Input.GetKey(keyCode)) res = true;

            return res;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="keyCode"></param>
        /// <returns></returns>
        public static bool ButtonUp(KeyCode keyCode)
        {
            bool res = false;

            if (Input.GetKeyUp(keyCode)) res = true;

            return res;
        }

        public static void UpdateMousePosition()
        {
            var pos = Input.mousePosition;

            if (uRetroDisplay.displayTarget == null)
            {
                mouse_x = (int)pos.x;
                mouse_y = (int)pos.y;
            }
            else
            {
                isInside = RectTransformUtility.ScreenPointToLocalPointInRectangle(uRetroDisplay.displayTarget, pos, Camera.main, out temp);

                if (isInside)
                {
                    mouse_x = (int)temp.x;
                    mouse_y = (int)temp.y;
                }
                else
                {
                    mouse_x = -1;
                    mouse_y = -1;
                    return;
                }

                var width = (int)uRetroDisplay.displayTarget.rect.width;
                var height = (int)uRetroDisplay.displayTarget.rect.height;

                // invalidate mouse if out of bounds
                mouse_x = width / 2 + mouse_x;

                if (mouse_x > width || mouse_x < 0)
                    mouse_x = -1;

                mouse_y = ((int)uRetroDisplay.displayTarget.rect.height - 1) - (height / 2 - mouse_y);

                if (mouse_y > height || mouse_y < 0)
                    mouse_y = -1;

                if (mouse_x == -1 || mouse_y == -1)
                {
                    mouse_x = mouse_y = -1;
                }

                if (mouse_x > 0)
                {
                    mouse_x = (int)Mathf.Clamp(mouse_x, 0, uRetroConfig.screen_width);
                }
                if (mouse_y > 0)
                {
                    mouse_y = (int)Mathf.Clamp(mouse_y, 0, uRetroConfig.screen_height);
                }
            }
        }

        public static bool IsInside()
        {
            return isInside;
        }

        public static int MouseX()
        {
            return mouse_x;
        }

        public static int MouseY()
        {
            return mouse_y;
        }

        public static bool MouseButtonDown(int id)
        {
            return Input.GetMouseButtonDown(id);
        }

        public static bool MouseButtonHold(int id)
        {
            return Input.GetMouseButton(id);
        }

        public static bool MouseButtonUp(int id)
        {
            return Input.GetMouseButtonUp(id);
        }
    }
}