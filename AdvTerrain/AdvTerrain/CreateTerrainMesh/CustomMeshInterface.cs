using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;



namespace AdvTerrain.CreateTerrainMesh
{
    
    interface CustomMeshInterface
    {
        VertexMultiTexture[] _terrainVertices { get;  }
        int[] _terrainIndices { get;  }
        VIBuffer _VIBuffer { get;  }
        VertexDeclaration _terrainVertexDeclaration { get; }
        VertexBuffer _terrainVertexBuffer { get; }
        IndexBuffer _terrainIndexBuffer { get; }
  
    }

    /*----------------- Enable for Colored Terrain -------------------------*/
    /*public struct vertexFormat
    {
        public Vector3 Position;
        public Color Color;
        public Vector3 Normal;

        public vertexFormat(Vector3 position, Vector3 normal, Color color)
        {
            this.Position = position;
            this.Normal = normal;
            this.Color = color;
        }

        public static readonly VertexElement[] vertexElements = new VertexElement[]
        {
          new VertexElement( 0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
          new VertexElement( sizeof(float) * 3, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
          new VertexElement( sizeof(float) * (3+3) , VertexElementFormat.Color, VertexElementUsage.Color, 0)
        };

        public static readonly VertexDeclaration vertexDeclaration =
          new VertexDeclaration(vertexElements);

    }*/

    /*------------- Enable for Single Textured Object -----------------*/
    /* 
     * Defauting to VertexPositionNormalTexture 
     *
     */

    /*------------- Enable for MultiTexturing ----------------------*/

    public struct VertexMultiTexture
    {
        public Vector3 Position;
        public Vector3 Normal;
        public Vector4 TextureCoordinate;
        public Vector4 TextureWeight;

        public static int SizeInBytes = (3 + 3 + 4 + 4) * sizeof(float);

        public VertexMultiTexture(Vector3 position, Vector3 normal, Vector4 texCoord, Vector4 texWeight)
        {
            this.Position = position;
            this.Normal = normal;
            this.TextureCoordinate = texCoord;
            this.TextureWeight = texWeight;
        }

        public static readonly VertexElement[] VertexElements = new VertexElement[]
        {
            new VertexElement(0,VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
            new VertexElement(sizeof(float) * 3, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
            new VertexElement(sizeof(float) * (3+3), VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 0),
            new VertexElement(sizeof(float) *(3 +3 + 4), VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 1)
        };

        public static readonly VertexDeclaration vertexDeclaration = new VertexDeclaration(SizeInBytes, VertexElements);
    }
    

    public struct VIBuffer
    {
        public VertexBuffer vertexBuffer;
        public IndexBuffer indexBuffer;

        public VIBuffer(VertexBuffer vBuffer, IndexBuffer iBuffer)
        {
            this.vertexBuffer = vBuffer;
            this.indexBuffer = iBuffer;
        }
    }

   
    
}
