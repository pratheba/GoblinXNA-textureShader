using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using GoblinXNA;
using GoblinXNA.Graphics;
using GoblinXNA.SceneGraph;
using GoblinXNA.Shaders;

using Model = GoblinXNA.Graphics.Model;
using GoblinXNA.Graphics.Geometry;
using GoblinXNA.Helpers;


namespace AdvTerrain.AddShader
{
    public class TerrainShader : Shader
    {
        private EffectParameter
            EnableLighting,
            worldForNormal,
            cameraPosition,

            light,
            numberOfLights,
            ambientLightColor,
            diffuseColor,

            grassTexture,
            rockTexture,
            sandTexture,
            snowTexture,
            cloudTexture;

        private List<LightSource> lightSources;
        private List<LightSource> dirLightSources;

        private bool is_3_0;
        private bool forcePS20;

        private Vector3 cameraPos;
        private string defaultTechnique;

        common _commonObj = new common();

        public TerrainShader(common commonObj)
            : base("AdvTerrainEffect")
        {
            lightSources = new List<LightSource>();
            dirLightSources = new List<LightSource>();

            defaultTechnique = "MultiTextured";
            is_3_0 = false;
            forcePS20 = true;

            _commonObj = commonObj;

            if ((State.Device.GraphicsProfile == GraphicsProfile.HiDef && !forcePS20))
            {
                is_3_0 = true;
            }
            else
            {
                is_3_0 = false;
            }
        }

       

        public bool UsePS0
        {
            get { return forcePS20; }
            set { forcePS20 = value; }
        }

        private void GetMinimumParameters()
        {
            world = effect.Parameters["world"];
            viewProj = effect.Parameters["viewProjection"];
            cameraPosition = effect.Parameters["cameraPosition"];
            worldForNormal = effect.Parameters["worldForNormal"];

            light = effect.Parameters["light"];
            numberOfLights = effect.Parameters["numberOfLights"];
            ambientLightColor = effect.Parameters["ambientLightColor"];
            diffuseColor = effect.Parameters["diffuseColor"];
            EnableLighting = effect.Parameters["EnableLighting"];

            grassTexture = effect.Parameters["grassTexture"];
        }

        protected override void GetParameters()
        {

            world = effect.Parameters["world"];
            viewProj = effect.Parameters["viewProjection"];
            cameraPosition = effect.Parameters["cameraPosition"];
            worldForNormal = effect.Parameters["worldForNormal"];

            light = effect.Parameters["light"];
            numberOfLights = effect.Parameters["numberOfLights"];
            ambientLightColor = effect.Parameters["ambientLightColor"];
            diffuseColor = effect.Parameters["diffuseColor"];
            EnableLighting = effect.Parameters["EnableLighting"];

            grassTexture = effect.Parameters["grassTexture"];
            rockTexture = effect.Parameters["rockTexture"];
            sandTexture = effect.Parameters["sandTexture"];
            snowTexture = effect.Parameters["snowTexture"];
           // cloudTexture = effect.Parameters["cloudTexture"];
        }

        public override void SetParameters(CameraNode camera)
        {
            cameraPos = camera.WorldTransformation.Translation;

            cameraPosition.SetValue(cameraPos);
        }

        public override void SetParameters(Material material)
        {
            if (material.InternalEffect != null)
            {
                effect = material.InternalEffect;
                GetMinimumParameters();
                cameraPosition.SetValue(cameraPos);
                defaultTechnique = "MultiTextured";
            }
            else
            {
                GetParameters();
                defaultTechnique = "MultiTextured";
            }

            SetTextureParameters();
        }

         
        public void SetTextureParameters()
        {
            grassTexture.SetValue(_commonObj.grassTexture);
            rockTexture.SetValue(_commonObj.rockTexture);
            sandTexture.SetValue(_commonObj.sandTexture);
            snowTexture.SetValue(_commonObj.snowTexture);
          //  cloudTexture.SetValue(_commonObj.cloudTexture);
        }

        public override void SetParameters(List<LightNode> globalLights, List<LightNode> localLights)
        {
            bool ambientSet = false;
            this.lightSources.Clear();

            LightNode lNode = null;
            Vector4 ambientLightColor = new Vector4(0, 0, 0, 1);

            EnableLighting.SetValue(1);

            for (int i = localLights.Count - 1; i >= 0; i--)
            {
                lNode = localLights[i];

                if (!ambientSet && (!lNode.AmbientLightColor.Equals(ambientLightColor)))
                {
                    ambientLightColor = lNode.AmbientLightColor;
                    ambientSet = true;
                }

                if (!lNode.LightSource.Enabled)
                    continue;

                LightSource source = new LightSource(lNode.LightSource);
                if (lNode.LightSource.Type != LightType.Directional)
                    source.Position = lNode.LightSource.TransformedPosition;
                if (lNode.LightSource.Type != LightType.Point)
                    source.Direction = lNode.LightSource.TransformedDirection;

                lightSources.Add(source);
            }
            for (int i = 0; i < globalLights.Count; i++)
            {
                lNode = globalLights[i];
                if (!ambientSet && (!lNode.AmbientLightColor.Equals(ambientLightColor)))
                {
                    ambientLightColor = lNode.AmbientLightColor;
                    ambientSet = true;
                }

                // skip the light source if not enabled
                if (!lNode.LightSource.Enabled)
                    continue;

                LightSource source = new LightSource(lNode.LightSource);
                if (lNode.LightSource.Type != LightType.Directional)
                    source.Position = lNode.LightSource.TransformedPosition;
                if (lNode.LightSource.Type != LightType.Point)
                    source.Direction = lNode.LightSource.TransformedDirection;

                // EnableLighting.SetValue(true);

                lightSources.Add(source);
            }

            dirLightSources.Clear();

            foreach (LightSource l in lightSources)
            {
                switch (l.Type)
                {
                    case LightType.Directional:
                        dirLightSources.Add(l);
                        break;
                }
            }

            this.ambientLightColor.SetValue(ambientLightColor);

        }

        public override void Render(ref Matrix worldMatrix, string techniqueName, RenderHandler renderDelegate)
        {
            if (renderDelegate == null)
                throw new GoblinException("renderDelegate is null");


            world.SetValue(worldMatrix);
            viewProj.SetValue(State.ViewProjectionMatrix);
            worldForNormal.SetValue(Matrix.Transpose(Matrix.Invert(worldMatrix)));

           
            

            effect.CurrentTechnique = effect.Techniques[defaultTechnique];
            {
                BlendState origBlendState = State.Device.BlendState;
                DepthStencilState origDepthState = State.Device.DepthStencilState;

                State.Device.BlendState = BlendState.Opaque;

                effect.CurrentTechnique.Passes["multitexturedpass"].Apply();
                renderDelegate();

                if (is_3_0)
                {
                    DoRendering30(renderDelegate, dirLightSources);
                }
                else
                {
                    DoRendering20(renderDelegate, dirLightSources);
                }

                State.Device.BlendState = origBlendState;
                State.Device.DepthStencilState = origDepthState;
            }
        }

        private void DoRendering30(RenderHandler renderDelegate, List<LightSource> lightSources)
        {
        }

        private void DoRendering20(RenderHandler renderDelegate, List<LightSource> lightSources)
        {
            string passName;
            if (lightSources.Count == 0)
            {
                return;
            }
            else
            {
                passName = "multitexturedpass";
            }
            for (int passCount = 0; passCount < lightSources.Count; passCount++)
            {
                SetUpSingleLightSource(lightSources[passCount]);
                effect.CurrentTechnique.Passes[passName].Apply();
                renderDelegate();
            }
        }

        private void SetUpSingleLightSource(LightSource lightSource)
        {
            light.StructureMembers["direction"].SetValue(lightSource.Direction);
            light.StructureMembers["position"].SetValue(lightSource.Position);
            light.StructureMembers["color"].SetValue(lightSource.Diffuse);
        }

        public override void Dispose()
        {
            base.Dispose();

            lightSources.Clear();
            dirLightSources.Clear();

        }
    }
}