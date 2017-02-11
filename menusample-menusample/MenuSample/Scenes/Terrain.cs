using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace Series3D1
{

    public struct VertexMultitextured : IVertexType
    {
        public Vector3 Position;
        public Vector3 Normal;
        public Vector4 TextureCoordinate;
        public Vector4 TexWeights;



        public static int SizeInBytes = (3 + 3 + 4 + 4) * sizeof(float);
        public static VertexElement[] VertexElements = new VertexElement[]
         {
             new VertexElement( 0,  VertexElementFormat.Vector3, VertexElementUsage.Position, 0 ),
             new VertexElement( sizeof(float) * 3, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0 ),
             new VertexElement(sizeof(float) * 6, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 0 ),
             new VertexElement( sizeof(float) * 10, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 1 ),
         };

        VertexDeclaration IVertexType.VertexDeclaration
        {
            get { return new VertexDeclaration(VertexMultitextured.SizeInBytes, VertexMultitextured.VertexElements); }
        }

    }



    class Terrain : Microsoft.Xna.Framework.Game
    {

       
        GraphicsDevice device;
        public float angle = 0f;
        public int terrainWidth;
        public int terrainHeight;
        float[,] heightData;
        int[] terrainIndices;


        VertexBuffer terrainVertexBuffer;
        IndexBuffer terrainIndexBuffer;
        VertexDeclaration terrainVertexDeclaration;

    

        Effect effect;



        Texture2D grassTexture;
        Texture2D sandTexture;
        Texture2D rockTexture;
        Texture2D snowTexture;
        VertexMultitextured[] terrainVertices;
        ContentManager content;



        public Terrain(ContentManager content, GraphicsDevice device)
        {
            this.content = content;
            this.device = device;

            effect = content.Load<Effect>("Series4Effects");
            PresentationParameters pp = device.PresentationParameters;
            LoadVertices();
            LoadTextures();
        }


      

        private void LoadVertices()
        {

            Texture2D heightMap = content.Load<Texture2D>("heightmap"); 
            LoadHeightData(heightMap);

            terrainVertices = SetUpTerrainVertices();
            terrainIndices = SetUpTerrainIndices();
            terrainVertices = CalculateNormals(terrainVertices, terrainIndices);
            terrainVertexDeclaration = new VertexDeclaration(VertexMultitextured.SizeInBytes, VertexMultitextured.VertexElements);
            CopyToTerrainBuffers(terrainVertices, terrainIndices);

          

        }

        private void LoadTextures()
        {

            grassTexture = content.Load<Texture2D>("grass");

            sandTexture = content.Load<Texture2D>("sand");
            rockTexture = content.Load<Texture2D>("rock");
            snowTexture = content.Load<Texture2D>("snow");
        }


        private VertexMultitextured[] SetUpTerrainVertices()
        {
            VertexMultitextured[] terrainVertices = new VertexMultitextured[terrainWidth * terrainHeight];

            for (int x = 0; x < terrainWidth; x++)
            {
                for (int y = 0; y < terrainHeight; y++)
                {

                    terrainVertices[x + y * terrainWidth].Position = new Vector3(x, heightData[x, y], -y);
                    terrainVertices[x + y * terrainWidth].TextureCoordinate.X = (float)x / 5.0f;
                    terrainVertices[x + y * terrainWidth].TextureCoordinate.Y = (float)y / 5.0f;

                    terrainVertices[x + y * terrainWidth].TexWeights.X = MathHelper.Clamp(1.0f - Math.Abs(heightData[x, y] - 0) / 8.0f, 0, 1);
                    terrainVertices[x + y * terrainWidth].TexWeights.Y = MathHelper.Clamp(1.0f - Math.Abs(heightData[x, y] - 12) / 6.0f, 0, 1);
                    terrainVertices[x + y * terrainWidth].TexWeights.Z = MathHelper.Clamp(1.0f - Math.Abs(heightData[x, y] - 20) / 6.0f, 0, 1);
                    terrainVertices[x + y * terrainWidth].TexWeights.W = MathHelper.Clamp(1.0f - Math.Abs(heightData[x, y] - 30) / 6.0f, 0, 1);

                    float total = terrainVertices[x + y * terrainWidth].TexWeights.X;
                    total += terrainVertices[x + y * terrainWidth].TexWeights.Y;
                    total += terrainVertices[x + y * terrainWidth].TexWeights.Z;
                    total += terrainVertices[x + y * terrainWidth].TexWeights.W;

                    terrainVertices[x + y * terrainWidth].TexWeights.X /= total;
                    terrainVertices[x + y * terrainWidth].TexWeights.Y /= total;
                    terrainVertices[x + y * terrainWidth].TexWeights.Z /= total;
                    terrainVertices[x + y * terrainWidth].TexWeights.W /= total;
                }
            }

            return terrainVertices;
        }

     
        private int[] SetUpTerrainIndices()
        {
            int[] indices = new int[(terrainWidth - 1) * (terrainHeight - 1) * 6];
            int counter = 0;
            for (int y = 0; y < terrainHeight - 1; y++)
            {
                for (int x = 0; x < terrainWidth - 1; x++)
                {
                    int lowerLeft = x + y * terrainWidth;
                    int lowerRight = (x + 1) + y * terrainWidth;
                    int topLeft = x + (y + 1) * terrainWidth;
                    int topRight = (x + 1) + (y + 1) * terrainWidth;

                    indices[counter++] = topLeft;
                    indices[counter++] = lowerRight;
                    indices[counter++] = lowerLeft;

                    indices[counter++] = topLeft;
                    indices[counter++] = topRight;
                    indices[counter++] = lowerRight;
                }
            }

            return indices;
        }

        private void LoadHeightData(Texture2D heightMap)
        {
            float minimumHeight = float.MaxValue;
            float maximumHeight = float.MinValue;

            terrainWidth = heightMap.Width;
            terrainHeight = heightMap.Height;

            Color[] heightMapColors = new Color[terrainWidth * terrainHeight];
            heightMap.GetData(heightMapColors);

            heightData = new float[terrainWidth, terrainHeight];
            //for (int x = 0; x < terrainWidth; x++)
            //    for (int y = 0; y < terrainHeight; y++)
            //        heightData[x, y] = heightMapColors[x + y * terrainWidth].R / 5.0f;
            for (int x = 0; x < terrainWidth; x++)
                for (int y = 0; y < terrainHeight; y++)
                {
                    heightData[x, y] = heightMapColors[x + y * terrainWidth].R / 5.0f;
                    if (heightData[x, y] < minimumHeight) minimumHeight = heightData[x, y];
                    if (heightData[x, y] > maximumHeight) maximumHeight = heightData[x, y];
                }

           for (int x = 0; x < terrainWidth; x++)
                for (int y = 0; y < terrainHeight; y++)
                    heightData[x, y] = (heightData[x, y] - minimumHeight) / (maximumHeight - minimumHeight) * 30.0f;
        }

        public bool outOfTerrain(Vector3 position)
        {

            if (position.X < -terrainWidth && position.X > terrainWidth && position.Z < -terrainHeight && position.Z > terrainHeight)
                return true;
            return false;
        }


        public float getHeightValue(int x, int y)
        {


            if (x < terrainWidth && x > 0 && y < terrainHeight && y > 0)
                return heightData[x, y];
            else return -10.0f;

        }

        public Vector3 getGraphicCoordinate(int x, int y, float z)
        {
            return new Vector3(x - ((terrainWidth) / 2), z, ((terrainHeight) / 2) - y);
        }

        private VertexMultitextured[] CalculateNormals(VertexMultitextured[] vertices, int[] indices)
        {
            for (int i = 0; i < vertices.Length; i++)
                vertices[i].Normal = new Vector3(0, 0, 0);

            for (int i = 0; i < indices.Length / 3; i++)
            {
                int index1 = indices[i * 3];
                int index2 = indices[i * 3 + 1];
                int index3 = indices[i * 3 + 2];

                Vector3 side1 = vertices[index1].Position - vertices[index3].Position;
                Vector3 side2 = vertices[index1].Position - vertices[index2].Position;
                Vector3 normal = Vector3.Cross(side1, side2);

                vertices[index1].Normal += normal;
                vertices[index2].Normal += normal;
                vertices[index3].Normal += normal;
            }

            for (int i = 0; i < vertices.Length; i++)
                vertices[i].Normal.Normalize();

            return vertices;
        }

        private void CopyToTerrainBuffers(VertexMultitextured[] vertices, int[] indices)
        {
            terrainVertexBuffer = new VertexBuffer(device, typeof(VertexMultitextured), vertices.Length, BufferUsage.WriteOnly);
            terrainVertexBuffer.SetData(vertices);

            terrainIndexBuffer = new IndexBuffer(device, typeof(int), indices.Length, BufferUsage.WriteOnly);
            terrainIndexBuffer.SetData(indices);
        }

        public void drawTerrain(Matrix viewMatrix, Matrix projectionMatrix)
        {
            device.BlendState = BlendState.Opaque;
            effect.CurrentTechnique = effect.Techniques["MultiTextured"];
            effect.Parameters["xTexture0"].SetValue(sandTexture);
            effect.Parameters["xTexture1"].SetValue(grassTexture);
            effect.Parameters["xTexture2"].SetValue(rockTexture);
            effect.Parameters["xTexture3"].SetValue(snowTexture);

            Matrix worldMatrix = Matrix.CreateTranslation(-terrainWidth / 2.0f, 0, terrainHeight / 2.0f) * Matrix.CreateRotationY(angle);
            effect.Parameters["xWorld"].SetValue(worldMatrix);
            effect.Parameters["xView"].SetValue(viewMatrix);
            effect.Parameters["xProjection"].SetValue(projectionMatrix);

            effect.Parameters["xEnableLighting"].SetValue(true);
            effect.Parameters["xAmbient"].SetValue(0.4f);
            effect.Parameters["xLightDirection"].SetValue(new Vector3(-0.5f, -1, -0.5f));

            effect.CurrentTechnique.Passes[0].Apply();
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();


               /* device.Indices = terrainIndexBuffer;
                device.SetVertexBuffer(terrainVertexBuffer);

                int noVertices = terrainVertexBuffer.VertexCount / VertexMultitextured.SizeInBytes;
                int noTriangles = terrainIndexBuffer.IndexCount / 3;*/
                //device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, terrainVertices.Length * VertexMultitextured.SizeInBytes, 0, noTriangles);
                device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, terrainVertices, 0, terrainVertices.Length, terrainIndices, 0, terrainIndices.Length / 3, terrainVertexDeclaration);

            }
        }


    }
}
