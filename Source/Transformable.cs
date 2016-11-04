using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WM.Framework.Monogame
{
    public abstract class Transformable
    {
        protected Matrix translation, rotation, scale, world;
        protected bool matrixChanged;

        public Matrix TranslationMatrix
        {
            get
            {
                return translation;
            }
            set
            {
                translation = value;
                matrixChanged = true;
            }
        }

        public Matrix RotationMatrix
        {
            get
            {
                return rotation;
            }
            set
            {
                rotation = value;
                matrixChanged = true;
            }
        }

        public Matrix ScaleMatrix
        {
            get
            {
                return scale;
            }
            set
            {
                scale = value;
                matrixChanged = true;
            }
        }

        public Matrix WorldMatrix
        {
            get
            {
                if (matrixChanged)
                {
                    matrixChanged = false;
                    SetWorld();
                }

                return world;
            }
        }

        protected virtual void SetWorld()
        {
            world = scale * rotation * translation;
        }

        protected void Initialize()
        {
            matrixChanged = true;
            translation = Matrix.Identity;
            rotation = Matrix.Identity;
            scale = Matrix.Identity;
        }

        public void Rotate(Matrix rotation)
        {
            this.rotation = rotation * this.rotation;
            matrixChanged = true;
        }

        public void Translate(Matrix translation)
        {
            this.translation = translation + this.translation;
            matrixChanged = true;
        }

        public void Scale(Matrix scale)
        {
            this.scale = scale * this.scale;
            matrixChanged = true;
        }

        public void Thrust(float amount)
        {
            Translate(Matrix.CreateTranslation(Vector3.Transform(Vector3.Forward, rotation) * amount));
        }

        public void StrafeHorizontally(float amount)
        {
            Translate(Matrix.CreateTranslation(
                Vector3.Normalize( Vector3.Cross(
                        Vector3.Transform(Vector3.Forward, rotation),
                        Vector3.Transform(Vector3.Up, rotation)))));
        }
    }
}