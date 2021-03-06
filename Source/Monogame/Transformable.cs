﻿using Microsoft.Xna.Framework;

namespace WM.Framework.Monogame
{
    public abstract class Transformable
    {
        protected Vector3 position, scaling;
        protected Matrix rotation;
        protected Matrix world, worldInv;
        protected bool worldDirty;
        protected bool worldInvDirty;

        public Vector3 Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
                worldDirty = true;
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
                worldDirty = true;
            }
        }
        public Matrix Rotation
        {
            get
            {
                return rotation;
            }
            set
            {
                rotation = value;
                worldDirty = true;
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
        public Matrix WorldInverse
        {
            get
            {
                if (worldInvDirty)
                {
                    worldInv = Matrix.Invert(world);
                    worldInvDirty = false;
                }

                return worldInv;
            }
        }

        public Vector3 Up
        {
            get
            {
                return rotation.Up;
            }
            set
            {
                rotation.Up = value;
                worldDirty = true;
            }
        }
        public Vector3 Down
        {
            get
            {
                return rotation.Down;
            }
            set
            {
                rotation.Down = value;
                worldDirty = true;
            }
        }
        public Vector3 Forward
        {
            get
            {
                return rotation.Forward;
            }
            set
            {
                rotation.Forward = value;
                worldDirty = true;
            }
        }
        public Vector3 Backward
        {
            get
            {
                return rotation.Backward;
            }
            set
            {
                rotation.Backward = value;
                worldDirty = true;
            }
        }
        public Vector3 Left
        {
            get
            {
                return rotation.Left;
            }
            set
            {
                rotation.Left = value;
                worldDirty = true;
            }
        }
        public Vector3 Right
        {
            get
            {
                return rotation.Right;
            }
            set
            {
                rotation.Right = value;
                worldDirty = true;
            }
        }

        public Quaternion RotationQ
        {
            get
            {
                return rotation.Rotation;
            }
        }

        // This method can be overriden to allow for different ways to
        // calculate the world matrix (if you wish to add skewing or
        // other transformations).
        protected virtual void CalculateWorldMatrix()
        {
            // To get the world matrix, we will need to multiply the matrices
            // in the opposite order we want to execute them. A different order
            // will yield a different result. If we rotate then move we will get
            // a different result than if we had moved then rotated.
            world = Matrix.CreateScale(scaling) * rotation * Matrix.CreateTranslation(position);
        }

        /// <summary>
        /// Initializes the variables.
        /// </summary>
        public virtual void Initialize()
        {
            position = Vector3.Zero;
            scaling = Vector3.One;
            rotation = Matrix.Identity;
            worldDirty = true;
        }

        #region TRANSLATION

        /// <summary>
        /// Translates this <see cref="Transformable"/>.
        /// </summary>
        public void Translate(Vector3 translation)
        {
            // You might question why I'm using Vector3.Add() instead
            // of just using the + operator. Well it seems that, for
            // some reason, using a method like this is faster than
            // using an operator, even though it shouldn't. ¯\_(ツ)_/¯
            position = Vector3.Add(position, translation);
            worldDirty = true;
        }

        /// <summary>
        /// Translates this <see cref="Transformable"/>.
        /// </summary>
        public void Translate(Matrix translation)
        {
            position = Vector3.Transform(position, translation);
            worldDirty = true;
        }

        /// <summary>
        /// Thrusts this <see cref="Transformable"/> forward.
        /// </summary>
        /// <param name="amount"></param>
        public void Thrust(float amount)
        {
            position = Vector3.Add(position, Vector3.Multiply(rotation.Forward, amount));
            worldDirty = true;
        }

        /// <summary>
        /// Strafes this <see cref="Transformable"/> horizontally.
        /// </summary>
        /// <param name="amount"></param>
        public void StrafeHorizontally(float amount)
        {
            position += rotation.Right * amount;
            worldDirty = true;
        }

        /// <summary>
        /// Strafes this <see cref="Transformable"/> vertically.
        /// </summary>
        /// <param name="amount"></param>
        public void StrafeVertically(float amount)
        {
            position += rotation.Up * amount;
            worldDirty = true;
        }

        #endregion

        #region ROTATION

        /// <summary>
        /// Applies a rotation to this <see cref="Transformable"/>.
        /// </summary>
        public void Rotate(Quaternion rotation)
        {
            this.rotation = Matrix.Multiply(this.rotation, Matrix.CreateFromQuaternion(rotation));
            worldDirty = true;
        }

        /// <summary>
        /// Applies a rotation to this <see cref="Transformable"/>.
        /// </summary>
        public void Rotate(Matrix rotation)
        {
            this.rotation = Matrix.Multiply(this.rotation, rotation);
            worldDirty = true;
        }

        /// <summary>
        /// Applies a rotation, but instead of applying to the object's pivot,
        /// it's around a custom pivot.
        /// </summary>
        /// <param name="pivot"></param>
        /// <param name="rotation"></param>
        public void RotateAround(Vector3 pivot, Matrix rotation)
        {
            position = Vector3.Add(pivot, Vector3.Transform(
                Vector3.Subtract(position, pivot),
                rotation));
            this.rotation = Matrix.Multiply(rotation, this.rotation);
            worldDirty = true;
        }

        /// <summary>
        /// Applies a rotation, but instead of applying to the object's pivot,
        /// it's around a custom pivot. This method doesn't apply the rotation
        /// to the object itself, only to the position.
        /// </summary>
        /// <param name="pivot"></param>
        /// <param name="rotation"></param>
        public void RotateAroundOnly(Vector3 pivot, Matrix rotation)
        {
            position = Vector3.Add(pivot, Vector3.Transform(
                Vector3.Subtract(position, pivot),
                rotation));
            worldDirty = true;
        }

        /// <summary>
        /// Sets the up vector, transforming the other components of the rotation.
        /// </summary>
        /// <param name="up">The new up vector.</param>
        /// <remarks>The right vector is created from the existing forward and the new up vectors.
        /// The new forward vector is then calculated from the new right and up vectors.</remarks>
        public void SetRotationFromUpR(Vector3 up)
        {
            rotation.Up = up;
            rotation.Right = Vector3.Normalize(Vector3.Cross(rotation.Forward, up));
            rotation.Forward = Vector3.Normalize(Vector3.Cross(up, rotation.Right));
            worldDirty = true;
        }

        /// <summary>
        /// Sets the up vector, transforming the other components of the rotation.
        /// </summary>
        /// <param name="up">The new up vector.</param>
        /// <remarks>The forward vector is created from the existing right and the new up vectors.
        /// The new right vector is then calculated from the new forward and up vectors.</remarks>
        public void SetRotationFromUpF(Vector3 up)
        {
            rotation.Up = up;
            rotation.Forward = Vector3.Normalize(Vector3.Cross(up, rotation.Right));
            rotation.Right = Vector3.Normalize(Vector3.Cross(rotation.Forward, up));
            worldDirty = true;
        }

        /// <summary>
        /// Rotates the <see cref="Transformable"/> around its up vector.
        /// </summary>
        /// <param name="angle"></param>
        public void Yaw(float angle)
        {
            rotation = Matrix.Multiply(rotation, Matrix.CreateFromAxisAngle(rotation.Up, angle));
            worldDirty = true;
        }

        /// <summary>
        /// Rotates the <see cref="Transformable"/> around its right vector.
        /// </summary>
        /// <param name="angle"></param>
        public void Pitch(float angle)
        {
            rotation = Matrix.Multiply(rotation, Matrix.CreateFromAxisAngle(rotation.Right, angle));
            worldDirty = true;
        }

        /// <summary>
        /// Rotates the <see cref="Transformable"/> around its forward vector.
        /// </summary>
        /// <param name="angle"></param>
        public void Roll(float angle)
        {
            rotation = Matrix.Multiply(rotation, Matrix.CreateFromAxisAngle(rotation.Forward, angle));
            worldDirty = true;
        }

        #endregion

        #region SCALE

        /// <summary>
        /// Scales this <see cref="Transformable"/>.
        /// </summary>
        public void Scale(Vector3 scale)
        {
            scaling = Vector3.Multiply(scaling, scale);
            worldDirty = true;
        }

        /// <summary>
        /// Scales this <see cref="Transformable"/>.
        /// </summary>
        public void Scale(float scale)
        {
            scaling = Vector3.Multiply(scaling, scale);
            worldDirty = true;
        }

        /// <summary>
        /// Scales this <see cref="Transformable"/>.
        /// </summary>
        public void Scale(Matrix scale)
        {
            scaling = Vector3.Transform(scaling, scale);
            worldDirty = true;
        }

        #endregion

        #region RESETS AND NORMALIZATIONS

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
            rotation = Matrix.Identity;
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

        /// <summary>
        /// Normalizes the vectors in the rotation matrix.
        /// </summary>
        public void NormalizeRotation()
        {
            rotation.Forward.Normalize();
            rotation.Right.Normalize();
            rotation.Up.Normalize();
            worldDirty = true;
        }

        #endregion
    }
}