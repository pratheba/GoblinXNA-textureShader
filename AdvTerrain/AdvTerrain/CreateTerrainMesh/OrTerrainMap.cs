
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using GoblinXNA;
using GoblinXNA.Graphics;
using GoblinXNA.Graphics.Geometry;
using GoblinXNA.Helpers;


namespace AdvTerrain.CreateTerrainMesh
{
    /// <summary>
    /// A pyramid geometry primitive constructed with CustomMesh
    /// </summary>
    public class OrTerrainMap : PrimitiveModel
    {
        /// <summary>
        /// Create a terrain with the given bitMap
        /// </summary>

       AdvTerrain.AddShader.TerrainShader terrainShader;

        static short[] Indices;
        private static int terrainWidth;
        private static int terrainHeight;
        private static float[,] heightData;

        static VertexBuffer myVertexBuffer;
        static IndexBuffer myIndexBuffer;

        public struct VertexPositionColorNormal
        {
            public Vector3 Position;
            public Color Color;
            public Vector3 Normal;

            public readonly static VertexDeclaration vertexDeclaration = new VertexDeclaration
            (
                new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
                new VertexElement(sizeof(float) * 3, VertexElementFormat.Color, VertexElementUsage.Color, 0),
                new VertexElement(sizeof(float) * 3 + 4, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0)
            );
        }

        static VertexMultiTexture[] t_vertices;

        public OrTerrainMap(Texture2D heightMap) :
            base(CreateShape(heightMap))
        {
            customShapeParameters = heightMap.Bounds.Width + " , " + heightMap.Bounds.Height;

        }

        private static CustomMesh CreateShape(Texture2D heightMap)
        {
            // Create a primitive mesh for creating our custom pyramid shape
            CustomMesh terrain = new CustomMesh();
            LoadHeightData(heightMap);
            SetUpVertices();
            SetUpIndices();
            CalculateNormals();
            CopyToBuffers();

            terrain.VertexBuffer = myVertexBuffer;
            terrain.IndexBuffer = myIndexBuffer;
            terrain.VertexDeclaration = VertexMultiTexture.vertexDeclaration;
            terrain.NumberOfVertices = t_vertices.Length;
            terrain.PrimitiveType = PrimitiveType.TriangleList;
            terrain.NumberOfPrimitives = Indices.Length / 3;

            Console.WriteLine(terrainWidth.ToString() + " " + terrainHeight.ToString());

            return terrain;
        }

        private static void LoadHeightData(Texture2D heightMap)
        {
            float minimumHeight = float.MaxValue;
            float maximumHeight = float.MinValue;
 
            terrainWidth = heightMap.Width;
            terrainHeight = heightMap.Height;

            Color[] heightMapColors = new Color[terrainWidth * terrainHeight];
            heightMap.GetData(heightMapColors);

            heightData = new float[terrainWidth, terrainHeight];

            for (int x = 0; x < terrainWidth; x++)
                for (int y = 0; y < terrainHeight; y++)
                {
                    heightData[x, y] = heightMapColors[x + y * terrainWidth].R;
                    if (heightData[x, y] < minimumHeight) minimumHeight = heightData[x, y];
                    if (heightData[x, y] > maximumHeight) maximumHeight = heightData[x, y];
                }

            for (int x = 0; x < terrainWidth; x++)
                for (int y = 0; y < terrainHeight; y++)
                    heightData[x, y] = (heightData[x, y] - minimumHeight) / (maximumHeight - minimumHeight) * 30.0f;
        }


        private static void CopyToBuffers()
        {
            myVertexBuffer = new VertexBuffer(State.Device, VertexMultiTexture.vertexDeclaration, t_vertices.Length
                 , BufferUsage.None);
            myVertexBuffer.SetData(t_vertices);

            myIndexBuffer = new IndexBuffer(State.Device, typeof(short), Indices.Length, BufferUsage.WriteOnly);
            myIndexBuffer.SetData(Indices);
        }

        private static void CalculateNormals()
        {
            for (int i = 0; i < t_vertices.Length; i++)
                t_vertices[i].Normal = new Vector3(0, 0, 0);

            for (int i = 0; i < Indices.Length / 3; i++)
            {
                int index1 = Indices[i * 3];
                int index2 = Indices[i * 3 + 1];
                int index3 = Indices[i * 3 + 2];

                Vector3 side1 = t_vertices[index1].Position - t_vertices[index3].Position;
                Vector3 side2 = t_vertices[index1].Position - t_vertices[index2].Position;
                Vector3 normal = Vector3.Cross(side1, side2);

                t_vertices[index1].Normal += normal;
                t_vertices[index2].Normal += normal;
                t_vertices[index3].Normal += normal;
            }

            for (int i = 0; i < t_vertices.Length; i++)
                t_vertices[i].Normal.Normalize();

        }

        private static void SetUpVertices()
        {
            t_vertices = new VertexMultiTexture[terrainWidth * terrainHeight];


            for (int x = 0; x < terrainWidth; x++)
            {
                for (int y = 0; y < terrainHeight; y++)
                {
                    t_vertices[x + y * terrainWidth].Position = new Vector3(x, heightData[x, y], -y);

                 //   t_vertices[x + y * terrainWidth].TextureCoordinate.X = (float)x / 30.0f;
                 //   t_vertices[x + y * terrainWidth].TextureCoordinate.Y = (float)y / 30.0f;

                }
            }
        }

        private static void SetUpIndices()
        {
            Indices = new short[(terrainWidth - 1) * (terrainHeight - 1) * 6];
            int counter = 0;

            for (short y = 0; y < terrainHeight - 1; y++)
            {
                for (short x = 0; x < terrainWidth - 1; x++)
                {
                    short lowerLeft = (short)(x + y * terrainWidth);
                    short lowerRight = (short)((x + 1) + y * terrainWidth);
                    short topLeft = (short)(x + (y + 1) * terrainWidth);
                    short topRight = (short)((x + 1) + (y + 1) * terrainWidth);

                    Indices[counter++] = topLeft;
                    Indices[counter++] = lowerRight;
                    Indices[counter++] = lowerLeft;

                    Indices[counter++] = topLeft;
                    Indices[counter++] = topRight;
                    Indices[counter++] = lowerRight;
                }
            }
        }
    }
}