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
        protected Vector3 position, scaling;
        protected Quaternion rotation;
        protected Matrix world;
        protected bool worldDirty;

        public Vector3 Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
            }
        }
        public Vector3 Scaling
        {
            get
            {
                return scaling;
            }
            set
            {
                scaling = value;
            }
        }
        public Quaternion Rotation
        {
            get
            {
                return rotation;
            }
            set
            {
                rotation = value;
                NormalizeRotationWhenNeeded();
            }
        }

        public Matrix World
        {
            get
            {
                if (worldDirty)
                {
                    CalculateWorldMatrix();
                    worldDirty = false;
                }

                return world;
            }
        }

        // This method can be overriden to allow for different ways to
        // calculate the world matrix.
        protected virtual void CalculateWorldMatrix()
        {
            // To get the world matrix, we will need to multiply the matrices
            // in the opposite order we want to execute them. A different order
            // will yield a different result. If we rotate then move we will get
            // a different result than if we had moved then rotated.
            world = Matrix.CreateScale(scaling) * Matrix.CreateFromQuaternion(rotation) * Matrix.CreateTranslation(position);
        }

        /// <summary>
        /// Initializes the variables.
        /// </summary>
        public virtual void Initialize()
        {
            position = Vector3.Zero;
            scaling = Vector3.One;
            rotation = Quaternion.Identity;
            worldDirty = true;
        }

        private void NormalizeRotationWhenNeeded()
        {
            float lengthSr = rotation.LengthSquared();
            if (lengthSr > 1.0001 || lengthSr < 0.9998)
            {
                rotation.Normalize();
                worldDirty = true;
            }
        }

        /// <summary>
        /// Translates this instance.
        /// </summary>
        public void Translate(Vector3 translation)
        {
            position = Vector3.Add(position, translation);
            worldDirty = true;
        }

        /// <summary>
        /// Translates this instance.
        /// </summary>
        public void Translate(Matrix translation)
        {
            position = Vector3.Transform(position, translation);
            worldDirty = true;
        }

        /// <summary>
        /// Applies a rotation to this instance.
        /// </summary>
        public void Rotate(Quaternion rotation)
        {
            this.rotation = Quaternion.Multiply(rotation, this.rotation);
            NormalizeRotationWhenNeeded();
            worldDirty = true;
        }

        /// <summary>
        /// Applies a rotation to this instance.
        /// </summary>
        public void Rotate(Matrix rotation)
        {
            this.rotation = Quaternion.Multiply(Quaternion.CreateFromRotationMatrix(rotation), this.rotation);
            NormalizeRotationWhenNeeded();
            worldDirty = true;
        }

        /// <summary>
        /// Scales this instance.
        /// </summary>
        public void Scale(Vector3 scale)
        {
            scaling = Vector3.Multiply(scaling, scale);
            worldDirty = true;
        }

        /// <summary>
        /// Scales this instance.
        /// </summary>
        public void Scale(float scale)
        {
            scaling = Vector3.Multiply(scaling, scale);
            worldDirty = true;
        }

        /// <summary>
        /// Scales this instance.
        /// </summary>
        public void Scale(Matrix scale)
        {
            scaling = Vector3.Transform(scaling, scale);
            worldDirty = true;
        }

        /// <summary>
        /// Resets the position to the origin.
        /// </summary>
        public void ResetPosition()
        {
            position = Vector3.Zero;
            worldDirty = true;
        }

        /// <summary>
        /// Resets the rotation to none.
        /// </summary>
        public void ResetRotation()
        {
            rotation = Quaternion.Identity;
            worldDirty = true;
        }

        /// <summary>
        /// Resets the scale to one.
        /// </summary>
        public void ResetScale()
        {
            scaling = Vector3.One;
            worldDirty = true;
        }
    }
}