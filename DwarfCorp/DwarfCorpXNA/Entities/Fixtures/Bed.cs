using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DwarfCorp.GameStates;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;

namespace DwarfCorp
{
    public class Bed : Body, IRenderableComponent
    {
        public Bed()
        {
            Tags.Add("Bed");
            CollisionType = CollisionManager.CollisionType.Static;
        }

        public Bed(ComponentManager manager, Vector3 position) :
            base(manager, "Bed", Matrix.CreateTranslation(position), new Vector3(2.0f, 0.5f, 1.0f), new Vector3(0.45f, 0.2f, 0.0f))
        {
            Tags.Add("Bed");
            CollisionType = CollisionManager.CollisionType.Static;
            SetFlag(Flag.RotateBoundingBox, true);

            CreateCosmeticChildren(manager);

            OrientToWalls();
        }

        public override void RenderSelectionBuffer(DwarfTime gameTime, ChunkManager chunks, Camera camera, SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, Shader effect)
        {
            effect.SelectionBufferColor = this.GetGlobalIDColor().ToVector4();
            GetComponent<Box>().Render(gameTime, chunks, camera, spriteBatch, graphicsDevice, effect, false);
        }

        public void Render(DwarfTime gameTime, ChunkManager chunks, Camera camera, SpriteBatch spriteBatch,
            GraphicsDevice graphicsDevice, Shader effect, bool renderingForWater)
        {
#if DEBUG
            if (GamePerformance.DebugVisualizationEnabled)
                Drawer3D.DrawBox(BoundingBox, Color.Blue, 0.02f, false);
#endif
        }

        public override void CreateCosmeticChildren(ComponentManager Manager)
        {
            base.CreateCosmeticChildren(Manager);

            var spriteSheet = TextureManager.GetTexture(ContentPaths.Entities.Furniture.bedtex);

            AddChild(new Box(Manager, 
                "bedbox", 
                Matrix.CreateTranslation(-0.40f, 0.00f, -0.45f) * Matrix.CreateRotationY((float)Math.PI * 0.5f), 
                new Vector3(1.0f, 1.0f, 2.0f), 
                new Vector3(0.0f, 0.0f, 0.0f), 
                "bed", 
                spriteSheet)).SetFlag(Flag.ShouldSerialize, false);

            AddChild(new NewVoxelListener(Manager,
                Matrix.Identity,
                new Vector3(1.5f, 0.5f, 0.75f), // Position just below surface.
                new Vector3(0.5f, -0.30f, 0.0f),
                (v) =>
                {
                    if (v.Type == VoxelChangeEventType.VoxelTypeChanged
                        && v.NewVoxelType == 0)
                        Die();
                }))
                .SetFlag(Flag.ShouldSerialize, false)
                .SetFlag(Flag.RotateBoundingBox, true);
        }
    }
}
