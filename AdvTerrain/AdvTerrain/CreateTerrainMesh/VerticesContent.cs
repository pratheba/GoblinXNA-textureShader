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
    public class VerticesContent : TerrainMetaInformation, CustomMeshInterface
    {
        private VertexMultiTexture[] terrainVertices;
        private int[] terrainIndices ;
        private VIBuffer VIBuffer;
        private VertexBuffer terrainVertexBuffer;
        private IndexBuffer terrainIndexBuffer;
        GraphicsDevice device;

        
        VertexDeclaration terrainVertexDeclaration;

        public VerticesContent(GraphicsDevice _device)
        {     device = _device;   
            
        }

        public VerticesContent(GraphicsDevice device, Texture2D heightMap)
        {
            LoadHeightData(heightMap);
            terrainVertices = SetTerrainVertices();
            terrainIndices = SetTerrainIndices();
            terrainVertices = CalculateNormal();
            VIBuffer = CopyToTerrainBuffers();
            terrainVertexBuffer = VIBuffer.vertexBuffer;
            terrainIndexBuffer = VIBuffer.indexBuffer;
            terrainVertexDeclaration = VertexMultiTexture.vertexDeclaration;
              
        }

        public VertexMultiTexture[] _terrainVertices
        {
            get { return terrainVertices; }
        }
        public int[] _terrainIndices
        {
            get { return terrainIndices; }
        }
        public VIBuffer _VIBuffer
        {
            get { return VIBuffer; }
        }

        public VertexDeclaration _terrainVertexDeclaration
        {
            get { return terrainVertexDeclaration; }
        }

        public VertexBuffer _terrainVertexBuffer
        {
            get { return terrainVertexBuffer; }
        }

        public IndexBuffer _terrainIndexBuffer
        {
            get { return terrainIndexBuffer; }
        }
    }
}
