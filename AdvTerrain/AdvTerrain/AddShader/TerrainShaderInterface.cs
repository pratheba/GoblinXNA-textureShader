using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

using Model = GoblinXNA.Graphics.Model;

namespace AdvTerrain.AddShader
{
    interface TerrainShaderInterface
    {
        bool isGrassTextureEnable { get; set; }
        bool isRockTextureEnable { get; set; }
        bool isSandTextureEnable { get; set; }
        bool isSnowTextureEnable { get; set; }

        Texture2D grassTexture { get; set; }
        Texture2D rockTexture { get; set; }
        Texture2D sandTexture { get; set; }
        Texture2D snowTexture { get; set; }
        Texture2D cloudTexture { get; set; }

        Model skyDome { get; set; }
    }
}
