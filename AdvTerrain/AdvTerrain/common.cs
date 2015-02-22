using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

using Model = GoblinXNA.Graphics.Model;

namespace AdvTerrain
{
    public class common : AddShader.TerrainShaderInterface
    {
        static Texture2D _grassTexture;
        static Texture2D _rockTexture;
        static Texture2D _sandTexture;
        static Texture2D _snowTexture;
        static Texture2D _cloudTexture;

        static bool _isGrassTextureEnable;
        static bool _isRockTextureEnable;
        static bool _isSandTextureEnable;
        static bool _isSnowTextureEnable;

        Model _skyDome;

        public common() { }

        public Texture2D grassTexture
        {
            get { return _grassTexture; }
            set { _grassTexture = value; }
           
        }

        public Texture2D rockTexture
        {
            get { return _rockTexture; }
            set { _rockTexture = value; }
        }

        public Texture2D sandTexture
        {
            get { return _sandTexture; }
            set { _sandTexture = value; }
        }

        public Texture2D snowTexture
        {
            get { return _snowTexture; }
            set { _snowTexture = value; }
        }

        public Texture2D cloudTexture
        {
            get { return _cloudTexture; }
            set { _cloudTexture = value; }
        }
        public bool isGrassTextureEnable
        {
            get { return _isGrassTextureEnable; }
            set { _isGrassTextureEnable = value; }
        }

        public bool isRockTextureEnable
        {
            get { return _isGrassTextureEnable; }
            set { _isGrassTextureEnable = value; }
        }


        public bool isSandTextureEnable
        {
            get { return _isGrassTextureEnable; }
            set { _isGrassTextureEnable = value; }
        }


        public bool isSnowTextureEnable
        {
            get { return _isGrassTextureEnable; }
            set { _isGrassTextureEnable = value; }
        }

        public Model skyDome
        {
            get { return _skyDome; }
            set { _skyDome = value; }
        }

    }
}
