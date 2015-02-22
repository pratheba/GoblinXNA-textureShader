using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GoblinXNA.SceneGraph;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace AdvTerrain.CreateSceneContent
{
    interface sceneContentInterface
    {
        Camera _camera {get; set;}
        TransformNode _terrainTransNode { get; set; }
        AddShader.TerrainShader _terrainShader { get; set; }

        void CreateLights();
        void CreateCamera(GraphicsDevice device);
        void CreateObjects(common commonObj, Texture2D heightMap);
        void UpdateViewMatrix();
        void UpdateCameraPostion(Vector3 inputVector);
        void UpdateMoves(float xDiff, float yDiff);
  
    }

     
   
    
}
