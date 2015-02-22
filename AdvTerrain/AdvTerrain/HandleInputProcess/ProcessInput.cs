using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using GoblinXNA;
using GoblinXNA.Device.Generic;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace AdvTerrain.HandleInputProcess
{
    class ProcessInput
    {
        float _amount;
        CreateSceneContent.sceneContentInterface _IsceneContent;
        Point originalMouseState;

        public ProcessInput(float amount, CreateSceneContent.sceneContentInterface IsceneContent)
        {
            _amount = amount;
            _IsceneContent = IsceneContent;
            Mouse.SetPosition(State.Device.Viewport.Width / 2, State.Device.Viewport.Height / 2);
            originalMouseState = new Point(State.Device.Viewport.Width / 2, State.Device.Viewport.Height / 2);
        }

        public void UpdateVectorAmount(float amount)
        {
            _amount = amount;
        }

        public void HandleMouseInput(Point currentMouseState)
        {
            if (currentMouseState != originalMouseState)
            {
                float xDifference = currentMouseState.X - originalMouseState.X;
                float yDifference = currentMouseState.Y - originalMouseState.Y;
                _IsceneContent.UpdateMoves(xDifference * _amount, yDifference * _amount);
               
            }
        }
        public void HandleInput(Keys key, KeyModifier modifer)
        {
            Vector3 moveVector = new Vector3(0);
            if (key == Keys.Up)
            {
                moveVector += new Vector3(0, 0, -1);
            }
            if (key == Keys.Down)
            {
                moveVector += new Vector3(0, 0, 1);
            }
            if (key == Keys.Left)
            {
                moveVector += new Vector3(-1, 0, 0);
            }
            if (key == Keys.Right)
            {
                moveVector += new Vector3(1, 0, 0);
            }
            if (key == Keys.F)
            {
                moveVector += new Vector3(0, -1, 0);
            }
            if (key == Keys.B)
            {
                moveVector += new Vector3(0, 1, 0);
            }

            _IsceneContent.UpdateCameraPostion( moveVector * _amount);
        }
    }
}
