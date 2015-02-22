 using System;
 using System.Collections.Generic;
 using Microsoft.Xna.Framework;
 using Microsoft.Xna.Framework.Audio;
 using Microsoft.Xna.Framework.Content;
 using Microsoft.Xna.Framework.GamerServices;
 using Microsoft.Xna.Framework.Graphics;
 using Microsoft.Xna.Framework.Input;
 using Microsoft.Xna.Framework.Net;
 using Microsoft.Xna.Framework.Storage;

using GoblinXNA;
using GoblinXNA.Graphics;
using GoblinXNA.Shaders;
using GoblinXNA.SceneGraph;
using GoblinXNA.Device;
using GoblinXNA.UI.UI2D;
using GoblinXNA.Device.Generic;

using Model = GoblinXNA.Graphics.Model;

 namespace AdvTerrain
 {
     public class TerrainClass : Microsoft.Xna.Framework.Game
     {
         GraphicsDeviceManager graphics;
         GraphicsDevice device;

         Scene scene;
         float angle = 0.0f;
         SpriteFont textFont;

         Texture2D heightMap;

        
         CreateSceneContent.sceneContent sceneContent;
         CreateSceneContent.sceneContentInterface IsceneContent;
         HandleInputProcess.ProcessInput processInput;
         common commonObj = new common();
         
         public TerrainClass()
         {
             graphics = new GraphicsDeviceManager(this);
             Content.RootDirectory = "Content";

             graphics.PreferredBackBufferWidth = 500;
             graphics.PreferredBackBufferHeight = 500;
         }
 
         protected override void Initialize()
         {
             base.Initialize();
             State.InitGoblin(graphics, Content, "");
             scene = new Scene();
             scene.BackgroundColor = Color.Black;

             CallToCreateSceneContent();

             processInput = new HandleInputProcess.ProcessInput(1.0f, IsceneContent);
             KeyboardInput.Instance.KeyPressEvent += new HandleKeyPress(processInput.HandleInput);
            // MouseInput.Instance.MouseMoveEvent += new HandleMouseMove(processInput.HandleMouseInput);

         }

         private void CallToCreateSceneContent()
         {
            
             // Create scene Content
             sceneContent = new CreateSceneContent.sceneContent(device,scene);
             IsceneContent = (CreateSceneContent.sceneContentInterface)sceneContent;
             IsceneContent.CreateLights();
             IsceneContent.CreateObjects(commonObj,heightMap);
             IsceneContent.CreateCamera(device);
             getValues();
            
         }

         private void getValues()
         {
             
         }

         protected override void LoadContent()
         {
             device = graphics.GraphicsDevice;
             heightMap = Content.Load<Texture2D> ("heightmap");
             textFont = Content.Load<SpriteFont>("Sample");

             Texture2D grassTexture = Content.Load<Texture2D>("grass");
             Texture2D rockTexture = Content.Load<Texture2D>("rock");
             Texture2D sandTexture = Content.Load<Texture2D>("sand");
             Texture2D snowTexture = Content.Load<Texture2D>("snow");
             Texture2D cloudTexture = Content.Load<Texture2D>("cloudMap");

             commonObj.grassTexture = grassTexture;
             commonObj.rockTexture = rockTexture;
             commonObj.sandTexture = sandTexture;
             commonObj.snowTexture = snowTexture;

            
        }



        protected override void UnloadContent()
        {        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            UpdateControl(gameTime);

            float timeDifference = (float)gameTime.ElapsedGameTime.TotalMilliseconds / 100.0f;
            processInput.UpdateVectorAmount(timeDifference);

            scene.Update(gameTime.ElapsedGameTime, gameTime.IsRunningSlowly, this.IsActive);
        }


        protected void UpdateControl(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            //UI2DRenderer.WriteText(Vector2.Zero, "Hello World!!", Color.GreenYellow, textFont,
            //GoblinEnums.HorizontalAlignment.Center, GoblinEnums.VerticalAlignment.Center);

            scene.Draw(gameTime.ElapsedGameTime, gameTime.IsRunningSlowly);
        }       
    }
}
 
 


