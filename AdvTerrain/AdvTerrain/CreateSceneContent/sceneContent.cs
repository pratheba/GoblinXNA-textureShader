using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;



using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using GoblinXNA.Graphics;
using GoblinXNA.Graphics.Geometry;


using GoblinXNA;
using GoblinXNA.Helpers;
using GoblinXNA.SceneGraph;
using Model = GoblinXNA.Graphics.Model;

namespace AdvTerrain.CreateSceneContent
{
    public class sceneContent : sceneContentInterface
    {
        # region Fields
        Scene _scene;
        GraphicsDevice _device;
        Camera camera;
        TransformNode terrainTransformNode;
        TransformNode skyDomeTransformNode;
        AdvTerrain.AddShader.TerrainShader terrainShader;

        Matrix ViewMatrix;
        ModelLoader loader = new ModelLoader();

        // Camera  movement
        float CamMoveUpDown = -MathHelper.Pi / 10.0f;
        float CamMoveLeftRight = MathHelper.PiOver2;
        const float CamRotationSpeed = 10.0f;
        const float CamMoveSpeed = 30.0f;
        MouseState originalMouseState;

        Vector3 CameraEye = new Vector3(130, 30, -20);

        #endregion

        public sceneContent(GraphicsDevice device,Scene scene)
        {
            _scene = scene;
            _device = device;
        }

        void sceneContentInterface.CreateLights()
        {
                // create a Light Source
                LightSource lightSource = new LightSource();
                lightSource.Direction = new Vector3(-0.5f, -1, -0.5f);
                lightSource.Direction.Normalize();
                lightSource.Diffuse = Color.Wheat.ToVector4();

                // LightNode to hold the lightsource
                LightNode lightNode = new LightNode();
                lightNode.AmbientLightColor = Color.White.ToVector4();
                lightNode.LightSource = lightSource;

                // TransformNode to add the lightnode to scene's rootnode
                TransformNode lightTransformNode = new TransformNode();
                lightTransformNode.AddChild(lightNode);

                _scene.RootNode.AddChild(lightTransformNode);
            
        }
        
        void sceneContentInterface.CreateCamera(GraphicsDevice device)
        {
           camera = new Camera();

            float aspectRatio = device.Viewport.AspectRatio;
            float fov = MathHelper.PiOver4;


            Matrix projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4,
                device.Viewport.AspectRatio, 0.3f, 1000.0f);

            camera.Projection = projectionMatrix;
            camera.AspectRatio = aspectRatio;

            camera.FieldOfViewY = fov;
            camera.ZNearPlane = 0.3f;
            camera.ZFarPlane = 1000.0f;

            UpdateViewMatrix();
            camera.View = ViewMatrix;
  
           // Now assign this camera to a camera node, and add this camera node to our scene graph
            CameraNode cameraNode = new CameraNode(camera);
            cameraNode.Name = "cameraNode";

            _scene.RootNode.AddChild(cameraNode);

            // Assign the camera node to be our scene graph's current camera node
            _scene.CameraNode = cameraNode;
        }

        void sceneContentInterface.CreateObjects(common commonObj, Texture2D heightMap)
        {
            terrainShader = new AddShader.TerrainShader(commonObj);
            
            GeometryNode terrainNode = new GeometryNode("TerrainNode")
            {
                Model = new AdvTerrain.CreateTerrainMesh.TerrainMap(heightMap),
            };
            terrainNode.Model.Shader = terrainShader;
            terrainNode.Name = "terrainNode";

            terrainTransformNode = new TransformNode("terrainTransNode");
           // terrainTransformNode.Rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitY, 0f);
           // terrainTransformNode.Translation = new Vector3(-heightMap.Width / 2, 0, heightMap.Width / 2);

            terrainTransformNode.AddChild(terrainNode);


            GeometryNode skyDomeNode = new GeometryNode("skyDomeNode");

            Model skymodel = (Model)loader.Load("", "dome");

            //skymodel.Mesh[0].MeshParts[0].Effect
            skyDomeNode.Model = skymodel;
           
            //skyDomeNode.IgnoreDepth = true;
            skyDomeNode.Name = "skyDomeNode";

            skyDomeNode.Material.Texture = commonObj.cloudTexture;

            skyDomeTransformNode = new TransformNode("skydomeTransNode");
            //skyDomeTransformNode.Translation = camera.Translation;
            //skyDomeTransformNode.Scale = new Vector3(30, 30, 30);


            skyDomeTransformNode.AddChild(skyDomeNode);

            _scene.RootNode.AddChild(skyDomeTransformNode);
            _scene.RootNode.AddChild(terrainTransformNode);
        }


        void sceneContentInterface.UpdateMoves(float xDiff, float yDiff)
        {
            CamMoveUpDown -= CamRotationSpeed * yDiff;
            CamMoveLeftRight -= CamRotationSpeed * xDiff;
            Mouse.SetPosition(State.Device.Viewport.Width / 2, State.Device.Viewport.Height / 2);
            UpdateViewMatrix();
        }

        void sceneContentInterface.UpdateCameraPostion(Vector3 inputVector)
        {
            Matrix RotationMatrix = Matrix.CreateRotationX(CamMoveUpDown) * Matrix.CreateRotationY(CamMoveLeftRight);
            Vector3 RotatedVector = Vector3.Transform(inputVector, RotationMatrix);
            CameraEye += CamMoveSpeed * RotatedVector;

            UpdateViewMatrix();
        }

        void sceneContentInterface.UpdateViewMatrix()
        {
            UpdateViewMatrix();
        }

        private void UpdateViewMatrix()
        {
            Matrix RotationMatrix = Matrix.CreateRotationX(CamMoveUpDown) * Matrix.CreateRotationY(CamMoveLeftRight);

            // View Matrix is created by the Camera Position (Eye) , Camera's target (Center) and camera's Up Vector (up)

            // Update the Camera's target ( lookat position)
            Vector3 originalCameraTarget = new Vector3(0, 0, -1);
            Vector3 rotatedCameraTarget = Vector3.Transform(originalCameraTarget, RotationMatrix);
            Vector3 newCameraTarget = CameraEye + rotatedCameraTarget;

            // Update Camera's Up vector ( which is relative to camera's displacement)
            Vector3 originalCameraUpVector = new Vector3(0, 1, 0);
            Vector3 newUpVector = Vector3.Transform(originalCameraUpVector, RotationMatrix);

            ViewMatrix = Matrix.CreateLookAt(CameraEye, newCameraTarget, newUpVector);
            camera.Rotation = Quaternion.CreateFromRotationMatrix(RotationMatrix);

            camera.View = ViewMatrix;

            skyDomeTransformNode.WorldTransformation = Matrix.CreateTranslation(0, 0.5f, 0) * Matrix.CreateScale(100.0f)
                * Matrix.CreateTranslation(CameraEye);
            
        }

        public Camera _camera
        {
            get { return camera; }
            set { camera = value; }
        }

        public TransformNode _terrainTransNode
        {
            get { return terrainTransformNode; }
            set { terrainTransformNode = value; }
        }

        public AddShader.TerrainShader _terrainShader
        {
            get { return terrainShader; }
            set { terrainShader = value; }
        }
    }

    
}
