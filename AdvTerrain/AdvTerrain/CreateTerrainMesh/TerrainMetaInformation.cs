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
    public class TerrainMetaInformation 
    {
        private int terrainWidth;
        private int terrainHeight;
        private float[,] heightData;

        VertexMultiTexture[] terrainvertices;
        int[] terrainIndices;

        private int noOfPixels;
  

        public TerrainMetaInformation()
        { }

       /// <summary>
       /// Get the height information for the terrain from a bitmap image
       /// converting the grayscale value to height value;
       /// </summary>
       /// <param name="heightMap"></param>
        protected void LoadHeightData(Texture2D heightMap)
        {
            float minHeightValue = float.MaxValue;
            float maxHeightValue = float.MinValue;

            terrainWidth = heightMap.Width;
            terrainHeight = heightMap.Height;

            noOfPixels = terrainWidth * terrainHeight;

            Color[] heightMapColor = new Color[noOfPixels];
            heightMap.GetData(heightMapColor);

            heightData = new float[terrainWidth, terrainHeight];

            for (int m_wid = 0; m_wid < terrainWidth; m_wid++)
            {
                for (int m_hei = 0; m_hei < terrainHeight; m_hei++)
                {
                    heightData[m_wid, m_hei] = heightMapColor[m_wid + m_hei * terrainWidth].R ;
                    if (heightData[m_wid, m_hei] < minHeightValue)
                        minHeightValue = heightData[m_wid, m_hei];
                    if (heightData[m_wid, m_hei] > maxHeightValue)
                        maxHeightValue = heightData[m_wid, m_hei];
                }
            }

            for (int m_wid = 0; m_wid < terrainWidth; m_wid++)
                for (int m_hei = 0; m_hei < terrainHeight; m_hei++)
                    heightData[m_wid, m_hei] = (heightData[m_wid, m_hei] - minHeightValue) / (maxHeightValue - minHeightValue) * 30.0f;

        }


        /// <summary>
        /// Load the vertices with the necessary information except normals
        /// // Position from heightMap
        /// // Color from the height variation of heightMap
        /// </summary>
        /// <returns></returns>
        protected VertexMultiTexture[] SetTerrainVertices()
        {
            terrainvertices = new VertexMultiTexture[noOfPixels];

            for (int m_wid = 0; m_wid < terrainWidth; m_wid++)
            {
                for (int m_hei = 0; m_hei < terrainHeight; m_hei++)
                {
                    int pos = m_wid + m_hei * terrainWidth;
                    terrainvertices[pos].Position =
                        new Vector3(m_wid, heightData[m_wid, m_hei], -m_hei);

                    // Set the textureCoordinate Mapping it to a value from 0 to 30
                    terrainvertices[pos].TextureCoordinate.X = (float)m_wid  / 30.0f;
                    terrainvertices[pos].TextureCoordinate.Y = (float)m_hei / 30.0f;
                    terrainvertices[pos].TextureCoordinate.Z = (float)m_wid / 10.0f;
                    terrainvertices[pos].TextureCoordinate.W = (float)m_hei / 10.0f;

                    // Terrain texturing with clamping of values at the edges
                    // 0 - 8  --> sand :: 6 - 18 --> grass :: 14 - 26 --> rock :: 24 - 30 --> snow
                    //    X   --> sand ::     Y  --> grass ::      Z  --> rock ::      W  --> snow   
                    // Change the Value with accordance to the amount of texture wanted 
                    // E.g Rocky mountain needs less grass and more rock i.e., 6 - 12 --> grass and 10 - 26 --> rock

                    float HData = heightData[m_wid, m_hei];

                    float sandV = terrainvertices[pos].TextureWeight.X = MathHelper.Clamp(1 - Math.Abs(HData - 0) / 8.0f, 0, 1);
                    float grassV = terrainvertices[pos].TextureWeight.Y = MathHelper.Clamp(1 - Math.Abs(HData - 12) / 6.0f, 0, 1);
                    float rockV = terrainvertices[pos].TextureWeight.Z = MathHelper.Clamp(1 - Math.Abs(HData - 20) / 6.0f, 0, 1);
                    float snowV = terrainvertices[pos].TextureWeight.W = MathHelper.Clamp(1 - Math.Abs(HData - 30) / 6.0f, 0, 1);

                    // Normalize Values to get only Texture information without any pixel gaining black color

                    float totalPercent = sandV + grassV + rockV + snowV;
                    terrainvertices[pos].TextureWeight.X /= totalPercent;
                    terrainvertices[pos].TextureWeight.Y /= totalPercent;
                    terrainvertices[pos].TextureWeight.Z /= totalPercent;
                    terrainvertices[pos].TextureWeight.W /= totalPercent;
                        
                }
            }

            return terrainvertices;
        }


        /// <summary>
        /// Create a Horizontal triangle strip to draw the terrain
        /// // As improvement can reduce the bandwidth by havign shared vertices
        /// </summary>
        /// <returns></returns>
        protected int[] SetTerrainIndices()
        {
            terrainIndices = new int[(terrainWidth -1) * (terrainHeight - 1) * 6];
            int index =0;

            for (int m_hei = 0; m_hei < terrainHeight - 1; m_hei++)
            {
                for (int m_wid = 0; m_wid < terrainWidth-1; m_wid++)
                {
                    int lowerLeft = m_wid + m_hei * terrainWidth;
                    int lowerRight = (m_wid + 1) + m_hei * terrainWidth;
                    int upperLeft = m_wid + (m_hei + 1) * terrainWidth;
                    int upperRight = (m_wid + 1) + (m_hei + 1) * terrainWidth;

                    terrainIndices[index++] = upperLeft;
                    terrainIndices[index++] = lowerRight;
                    terrainIndices[index++] = lowerLeft;

                    terrainIndices[index++] = upperLeft;
                    terrainIndices[index++] = upperRight;
                    terrainIndices[index++] = lowerRight;
                }
            }

            return terrainIndices;
        }


        /// <summary>
        /// Calculate the normal at each vertex
        /// </summary>
        /// <param name="vertices"></param>
        /// <param name="indices"></param>
        /// <returns></returns>
        protected VertexMultiTexture[] CalculateNormal()
        {

            for(int i=0; i< terrainvertices.Length ; i++)
                terrainvertices[i].Normal = new Vector3(0, 0, 0);


            for (int i = 0; i <  terrainIndices.Length / 3 ; i++)
            {
                int index1 = terrainIndices [i * 3];
                int index2 = terrainIndices[i * 3 + 1];
                int index3 = terrainIndices[i * 3 + 2];

                Vector3 p1 = terrainvertices[index1].Position - terrainvertices[index3].Position;
                Vector3 p2 = terrainvertices[index1].Position - terrainvertices[index2].Position;

                Vector3 normal = Vector3.Cross(p1, p2);

                terrainvertices[index1].Normal += normal;
                terrainvertices[index2].Normal += normal;
                terrainvertices[index3].Normal += normal;

            }

            for (int i = 0; i < terrainvertices.Length; i++)
                terrainvertices[i].Normal.Normalize();

            return terrainvertices;

        }

        protected VIBuffer CopyToTerrainBuffers()
        {
            VertexBuffer vertexBuffer = new VertexBuffer(State.Device, VertexMultiTexture.vertexDeclaration,
                terrainvertices.Length * VertexMultiTexture.SizeInBytes , BufferUsage.None);
            IndexBuffer indexBuffer = new IndexBuffer(State.Device, typeof(int),terrainIndices.Length , BufferUsage.WriteOnly);

            vertexBuffer.SetData(terrainvertices);
            indexBuffer.SetData(terrainIndices);

            VIBuffer _VIBuffer = new VIBuffer(vertexBuffer, indexBuffer);
            return _VIBuffer;

        }
    }
}
