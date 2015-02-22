using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using GoblinXNA;
using GoblinXNA.Graphics;
using GoblinXNA.Shaders;
using GoblinXNA.SceneGraph;
using Model = GoblinXNA.Graphics.Model;
using GoblinXNA.Graphics.Geometry;
using GoblinXNA.Device.Generic;


namespace AdvTerrain.CreateTerrainMesh
{
    public class TerrainMap : PrimitiveModel 
    {
        /// <summary>
        /// Constructor to create the custom mesh of terrain
        /// </summary>
        /// <param name="heightMap"></param>
        public TerrainMap(Texture2D heightMap) :
            base(CreateMesh(heightMap))
        {
            customShapeParameters = heightMap.Bounds.Width + " , " + heightMap.Bounds.Height;
        }

        /// <summary>
        /// Method to Create Mesh from the Bitmap image provided
        /// </summary>
        /// <param name="heightMap"></param>
        /// <returns></returns>
        private static CustomMesh CreateMesh(Texture2D heightMap)
        {
            CustomMesh terrainMesh = new CustomMesh();
            VerticesContent _vertContent = new VerticesContent(State.Device, heightMap);

            terrainMesh.VertexBuffer = _vertContent._terrainVertexBuffer;
            terrainMesh.IndexBuffer = _vertContent._terrainIndexBuffer;
            terrainMesh.VertexDeclaration = _vertContent._terrainVertexDeclaration;
            terrainMesh.NumberOfVertices = _vertContent._terrainVertices.Length;
            terrainMesh.PrimitiveType = PrimitiveType.TriangleList;
            terrainMesh.NumberOfPrimitives = _vertContent._terrainIndices.Length / 3;

            // Call VerticesContent class to load vertex Content
            
            return terrainMesh;
        }
    }
}
