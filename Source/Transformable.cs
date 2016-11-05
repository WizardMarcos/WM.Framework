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
                    CalculateWorld();
                }

                return world;
            }
        }

        // We are declaring this method as protected and virtual
        // to allow anyone to override and add more matrices to
        // the world matrix calculation, for example, to rotate
        // around a point.
        protected virtual void CalculateWorld()
        {
            // To calculate the world matrix, we need to multiply
            // all transformation matrices. The multiplication
            // order is important. It usually the inverse order
            // of what you want to do. Why? If you rotate and then
            // move something in the rotated direction, you will
            // have it at a different position than if you moved
            // it and then rotated.
            world = scale * rotation * translation;
        }

        protected void Initialize()
        {
            matrixChanged = true;
            translation = Matrix.Identity;
            rotation = Matrix.Identity;
            scale = Matrix.Identity;
        }

        /// <summary>
        /// Resets the rotation.
        /// </summary>
        public void ResetRotation()
        {
            rotation = Matrix.Identity;
            matrixChanged = true;
        }

        /// <summary>
        /// Resets the location.
        /// </summary>
        public void ResetTranslation()
        {
            translation = Matrix.Identity;
            matrixChanged = true;
        }

        /// <summary>
        /// Resets the scale.
        /// </summary>
        public void ResetScale()
        {
            scale = Matrix.Identity;
            matrixChanged = true;
        }

        /// <summary>
        /// Applies a rotation to the current one.
        /// </summary>
        /// <param name="rotation"></param>
        public void Rotate(Matrix rotation)
        {
            this.rotation = this.rotation * rotation;
            matrixChanged = true;
        }

        /// <summary>
        /// Translates relative to the current position.
        /// </summary>
        /// <param name="translation"></param>
        public void Translate(Matrix translation)
        {
            this.translation = this.translation * translation;
            matrixChanged = true;
        }

        /// <summary>
        /// Scales relative to the current scale.
        /// </summary>
        /// <param name="scale"></param>
        public void Scale(Matrix scale)
        {
            this.scale = this.scale * scale;
            matrixChanged = true;
        }

        /// <summary>
        /// Tries to set the world matrix. Returns true if it was able, else returns false.
        /// </summary>
        /// <param name="world"></param>
        /// <returns></returns>
        public bool SetWorldMatrix(Matrix world)
        {
            Vector3 t, s;
            Quaternion r;
            bool b = world.Decompose(out s, out r, out t);

            if (b)
            {
                translation = Matrix.CreateTranslation(t);
                rotation = Matrix.CreateFromQuaternion(r);
                scale = Matrix.CreateScale(s);
                matrixChanged = true;
            }

            return b;
        }
    }
}