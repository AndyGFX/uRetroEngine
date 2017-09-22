//----------------------------------------------
//            Heavy-Duty Inspector
//      Copyright © 2013 - 2014  Illogika
//----------------------------------------------

using UnityEngine;
using System;
using System.Diagnostics;
using System.Collections;

namespace CubesTeam
{

    public class ButtonAttribute : PropertyAttribute
    {

        public string buttonText
        {
            get;
            private set;
        }

        public string buttonFunction
        {
            get;
            private set;
        }

        public bool hideVariable
        {
            get;
            private set;
        }

        /// <summary>
        /// Displays a button before the affected variable. In versions of Unity older than 4.3, the variable can only be displayed if it is of type bool, int, float, string, Color, Object reference, Rect, Vector2 or Vector3. Other variable types will have the variable hidden by default. In Unity 4.3 or higher, variables of any type can be displayed.
        /// </summary>
        /// <param name="buttonText">Text displayed on the button.</param>
        /// <param name="buttonFunction">The name of the function to be called</param>
        /// <param name="hideVariable">If set to <c>true</c> hides the variable.</param>
        public ButtonAttribute(string buttonText, string buttonFunction, bool hideVariable = false)
        {
            this.buttonText = buttonText;
            this.buttonFunction = buttonFunction;
            this.hideVariable = hideVariable;
        }
    }

}
