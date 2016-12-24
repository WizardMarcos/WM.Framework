using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WM.Framework
{
    public class SpriteBatch3D
    {
        private SpriteBatch spriteBatch;

        public SpriteBatch3D(SpriteBatch spriteBatch)
        {
            this.spriteBatch = spriteBatch;
        }

        public void Begin(SpriteSortMode sortMode = SpriteSortMode.BackToFront, BlendState blendState = null,
            SamplerState samplerState = null, DepthStencilState depthStencilState = null,
            RasterizerState rasterizerState = null, Effect effect = null, Matrix? transformMatrix = null)
        {
            spriteBatch.Begin(sortMode, blendState, samplerState, depthStencilState, rasterizerState, effect, transformMatrix);
        }

        public void Draw()
        {

        }
    }
}
