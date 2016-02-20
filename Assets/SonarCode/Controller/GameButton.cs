using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace Sonar
{
    public class GameButton
    {
        string button;
        bool justPressed;
        bool justReleased;
        bool held;
        byte index;
        
        public GameButton(String Button)
        {
            button = Button;
        }

        public GameButton(String Button, byte Index)
        {
            index = Index;
            button = Button;
        }

        [Obsolete("For Unity Port")]
        public void Update() {}

        public bool Pressed {
            get { return Input.GetButtonDown(button);}
        }

        public bool Released
        {
            get { return Input.GetButtonUp(button); }
        }

        public bool Held
        {
            get { return Input.GetButtonDown(button); }
        }

        public byte Index
        {
            get { return index; }
        }
    }
}
