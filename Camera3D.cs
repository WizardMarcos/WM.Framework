using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WM.Framework.Monogame
{
    public abstract class Camera3D
    {
        // Every variable as a public property assigned to it, which also
        // updates a bool that tells us if we should update our matrices.
        // This improves the performance of our camera if it is static,
        // at the cost of a little bit more memory.

        // Position, Direction and Up vectors.
        // These influence the View Matrix.
        #region POSITION

        protected Vector3 position;
        public Vector3 Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
                viewIsDirty = true;
            }
        }
        public void ChangePosition(Vector3 position)
        {
            this.position = position;
            viewIsDirty = true;
        }

        #endregion
        #region DIRECTION

        // The Direction vector is always normalized.
        protected Vector3 direction;
        public Vector3 Direction
        {
            get
            {
                return direction;
            }
            set
            {
                direction = Vector3.Normalize(value);
                viewIsDirty = true;
            }
        }
        public void ChangeDirection(Vector3 direction)
        {
            this.direction = Vector3.Normalize(direction);
            viewIsDirty = true;
        }

        #endregion
        #region UP

        // The Up vector is always normalized.
        protected Vector3 up;
        public Vector3 Up
        {
            get
            {
                return up;
            }
            set
            {
                up = Vector3.Normalize(value);
                viewIsDirty = true;
            }
        }
        public void ChangeUpVector(Vector3 up)
        {
            this.up = Vector3.Normalize(up);
            viewIsDirty = true;
        }

        #endregion

        // Near Plane and Far Plane.
        // These influence the Projection Matrix.
        // They are common to all projection matrix creation forms, and so
        // are included in the base class.
        #region NEAR PLANE

        // The Near Plane is the closest distance at which the camera renders.
        protected float nearPlane;
        public float NearPlane
        {
            get
            {
                return nearPlane;
            }
            set
            {
                nearPlane = value;
                projectionIsDirty = true;
            }
        }
        public void SetNearPlane(float nearPlane)
        {
            this.nearPlane = nearPlane;
            projectionIsDirty = true;
        }

        #endregion
        #region FAR PLANE

        // The Far Plane is the farthest distance at which the camera renders.
        // While a very large value may be desirable, it comes at a cost of 
        // performance.
        protected float farPlane;
        public float FarPlane
        {
            get
            {
                return farPlane;
            }
            set
            {
                farPlane = value;
                projectionIsDirty = true;
            }
        }
        public void SetFarPlane(float farPlane)
        {
            this.farPlane = farPlane;
            projectionIsDirty = true;
        }

        #endregion

        // The View and Projection Matrices.
        // I would reccomend you to read RB Whitaker's matrix explanation
        // to understand what these matrices are why they are needed.
        // http://rbwhitaker.wikidot.com/monogame-basic-matrices
        #region VIEW MATRIX

        protected bool viewIsDirty;
        private Matrix viewMatrix;
        public Matrix ViewMatrix
        {
            get
            {
                if (viewIsDirty)
                {
                    viewMatrix = Matrix.CreateLookAt(position, position + direction, up);
                    viewIsDirty = false;
                }
                return viewMatrix;
            }
        }

        #endregion
        #region PROJECTION MATRIX

        protected bool projectionIsDirty;
        protected Matrix projectionMatrix;
        public abstract Matrix ProjectionMatrix { get; }

        #endregion

        /// <summary>
        /// Rotates the camera so it points to the target.
        /// </summary>
        /// <param name="target">Where the camera should look at.</param>
        public void LookAt(Vector3 target)
        {
            // The direction is calculated from the target and position.
            direction = Vector3.Normalize(target - position);
            viewIsDirty = true;
        }

        /// <summary>
        /// Moves the camera.
        /// </summary>
        /// <param name="translation">The amount to move.</param>
        public void Translate(Vector3 translation)
        {
            // Translation is very simple, as we only need to change the position,
            // because the target is calculated with the direction, which doesn't
            // change.
            position += translation;
            viewIsDirty = true;
        }

        /// <summary>
        /// Moves the camera into the direction it is pointing to.
        /// </summary>
        /// <param name="amount">The amount to move.</param>
        public void Thurst(float amount)
        {
            // The direction is a normalized vector, so we can simply multiply it
            // by an amount and it properly thrusts the camera.
            position += direction * amount;
            viewIsDirty = true;
        }

        /// <summary>
        /// Strafes the camera horizontally in relation to its direction.
        /// </summary>
        /// <param name="amount">The amount to strafe.</param>
        public void StrafeHorizontally(float amount)
        {
            // To strafe horizontally, we need to find the camera's horizontal
            // vector, which happens to be perpendicular to both the up and direction
            // vectors. We can use Vector3.Cross to do that. Then we just have to
            // normalize the result and multiply it by the amount we want to strafe.
            position += Vector3.Normalize(Vector3.Cross(up, direction)) * amount;
            viewIsDirty = true;
        }

        /// <summary>
        /// Strafes the camera vertically along its up vector.
        /// </summary>
        /// <param name="amount">The amount to strafe.</param>
        public void StrafeVertically(float amount)
        {
            // It is much easier to strafe vertically than horizontally.
            // We just need to move the camera along its up vector, which
            // is already normalized, so we just multiply it by the amount
            // we want to strafe.
            position += up * amount;
            viewIsDirty = true;
        }

        /// <summary>
        /// Yaw.
        /// </summary>
        /// <param name="angle">The angle in radians.</param>
        public void Yaw(float angle)
        {
            // To apply yaw to the camera, we need to rotate the direction around
            // the up vector. To do that, we can just apply a transformation to
            // the vector. The method CreateFromAxisAngle (which is also available
            // in matrices) can be used for that.
            direction = Vector3.Transform(direction, Quaternion.CreateFromAxisAngle(up, angle));
            viewIsDirty = true;
        }

        /// <summary>
        /// Pitch.
        /// </summary>
        /// <param name="angle">The angle in radians.</param>
        public void Pitch(float angle, bool changeUp = false)
        {
            // To apply pitch the camera, we need to rotate it around the
            // horizontal vector (calculated the same way we did in the
            // StrafeHorizontally method.
            Vector3 v = Vector3.Normalize(Vector3.Cross(up, direction));

            // We apply a rotation around the horizontal vector to the direction.
            direction = Vector3.Transform(direction, Quaternion.CreateFromAxisAngle(v, angle));

            // If we need to change the up vector, we can also rotate it
            // around the same axis. Most of the time just changing the
            // direction will be fine (when using the mouse, for example).
            if (changeUp)
                up = Vector3.Transform(up, Quaternion.CreateFromAxisAngle(v, angle));

            viewIsDirty = true;
        }

        /// <summary>
        /// Roll.
        /// </summary>
        /// <param name="angle">The angle in radians.</param>
        public void Roll(float angle)
        {
            // To roll, we just need to rotate the up vector around the direction.
            Up = Vector3.Transform(up, Matrix.CreateFromAxisAngle(direction, MathHelper.ToRadians(angle)));
        }

        /// <summary>
        /// Basic function to use the mouse to control the camera.
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="centre">The centre of the screen, used to reset the mouse after rotating.</param>
        /// <param name="sensitivity">Mouse sensitivity.</param>
        public void UseMouse(GameTime gameTime, Point centre, float sensitivity)
        {
            // This is a very basic function to use the mouse to control the camera.
            // It will behave similarly to a FPS game.
            // We need the gametime to avoid inconscistency with varying frame intervals
            // (when there are framedrops, for example).
            // The centre point is used to calculate the offset. The centre of the screen
            // should be used because it allows for more freedom of movement.
            // The sensitivity allows for a more precise control of the speed.

            // First we need to get the mouse state.
            MouseState mouseState = Mouse.GetState();

            // We apply the mouse's offset to the centre of the screen to the camera,
            // The horizontal position being the yaw and the vertical the pitch.
            Yaw(sensitivity * (centre.X - mouseState.X) * (float)gameTime.ElapsedGameTime.TotalSeconds);
            Pitch(sensitivity * (mouseState.Y - centre.Y) * (float)gameTime.ElapsedGameTime.TotalSeconds);

            // We reset the mouse's position. This could be performed outside of the method,
            // but is included here to simply the implementation.
            Mouse.SetPosition(centre.X, centre.Y);
        }

        /// <summary>
        /// Basic function to use the mouse to control the camera.
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="centre">The centre of the screen, used to reset the mouse after rotating.</param>
        /// <param name="sensitivityX">Mouse horizontal sensitivity.</param>
        /// <param name="sensitivityY">Mouse vertical sensitivity.</param>
        public void UseMouse(GameTime gameTime, Point centre, float sensitivityX, float sensitivityY)
        {
            // This method is equal to the previous, but allows different sensitivities
            // for each of the directions (x and y).
            MouseState mouseState = Mouse.GetState();

            Yaw(sensitivityX * (centre.X - mouseState.X) * (float)gameTime.ElapsedGameTime.TotalSeconds);
            Pitch(sensitivityY * (mouseState.Y - centre.Y) * (float)gameTime.ElapsedGameTime.TotalSeconds);

            Mouse.SetPosition(centre.X, centre.Y);
        }

        /// <summary>
        /// Basic function to use the keyboard to control the camera.
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="up">Up.</param>
        /// <param name="down">Down.</param>
        /// <param name="left">Left.</param>
        /// <param name="right">Right.</param>
        /// <param name="speed">Speed.</param>
        public void UseKeys(GameTime gameTime, Keys up, Keys down, Keys left, Keys right, float speed)
        {
            // This is a simple method to use the Keyboard to control the camera.
            // it behaves in a similar way to the mouse, but the values are obtained
            // in a different way.

            // First we get the keyboard state.
            KeyboardState state = Keyboard.GetState();

            // We create a point, with value zero.
            Point centre = Point.Zero;

            // For each key that was pressed, we change the value of the point.
            if (state.IsKeyDown(down))
                centre.Y++;
            if (state.IsKeyDown(up))
                centre.Y--;
            if (state.IsKeyDown(left))
                centre.X++;
            if (state.IsKeyDown(right))
                centre.X--;

            // Then we just apply it to the yaw and pitch in the same way as the mouse.
            Yaw(speed * centre.X * (float)gameTime.ElapsedGameTime.TotalSeconds);
            Pitch(speed * centre.Y * (float)gameTime.ElapsedGameTime.TotalSeconds);
        }

        /// <summary>
        /// Basic function to use the keyboard to control the camera.
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="up">Up.</param>
        /// <param name="down">Down.</param>
        /// <param name="left">Left.</param>
        /// <param name="right">Right.</param>
        /// <param name="speedX">Horizontal speed.</param>
        /// <param name="speedY">Vertical speed.</param>
        public void UseKeys(GameTime gameTime, Keys up, Keys down, Keys left, Keys right, float speedX, float speedY)
        {
            // Equal to the previous method, but different sensitivities applied to each axis.
            KeyboardState state = Keyboard.GetState();
            Point centre = Point.Zero;

            if (state.IsKeyDown(down))
                centre.Y++;
            if (state.IsKeyDown(up))
                centre.Y--;
            if (state.IsKeyDown(left))
                centre.X++;
            if (state.IsKeyDown(right))
                centre.X--;

            Yaw(speedX * centre.X * (float)gameTime.ElapsedGameTime.TotalSeconds);
            Pitch(speedY * centre.Y * (float)gameTime.ElapsedGameTime.TotalSeconds);
        }
    }

    public class Camera3DPerspective : Camera3D
    {
        // Field Of View and Aspect Ratio.
        // These influence the Projection Matrix.
        #region FOV

        // The field of view is a float with its values ranging from 0 to pi.
        private float fov;
        public float Fov
        {
            get
            {
                return fov;
            }
            set
            {
                fov = value;
                projectionIsDirty = true;
            }
        }
        public void SetFov(float fov)
        {
            this.fov = fov;
            projectionIsDirty = true;
        }

        #endregion
        #region ASPECT RATIO

        private float aspectRatio;
        public float AspectRatio
        {
            get
            {
                return aspectRatio;
            }
            set
            {
                aspectRatio = value;
                projectionIsDirty = true;
            }
        }
        public void SetAspectRatio(float aspectRatio)
        {
            this.aspectRatio = aspectRatio;
            projectionIsDirty = true;
        }

        #endregion

        #region PROJECTION MATRIX

        // Method to override the creation of the projection matrix.
        public override Matrix ProjectionMatrix
        {
            get
            {
                if (projectionIsDirty)
                {
                    // This camera creates the projection matrix using the field of view,
                    // aspect ratio and the near plane and far plane distances.
                    projectionMatrix = Matrix.CreatePerspectiveFieldOfView(fov, aspectRatio, NearPlane, FarPlane);
                    projectionIsDirty = false;
                }
                return projectionMatrix;
            }
        }

        #endregion

        /// <summary>
        /// Creates a <see cref="Camera3D"/> using a perspective projection.
        /// </summary>
        /// <param name="position">Position of the camera.</param>
        /// <param name="direction">Direction to where the camera is looking.</param>
        /// <param name="up">The Up vector.</param>
        /// <param name="fov">Field of view.</param>
        /// <param name="aspectRatio">Aspect ratio.</param>
        /// <param name="nearPlane">The closest distance at which objects are rendered.</param>
        /// <param name="farPlane">The farthest distance at which objects are rendered.</param>
        public Camera3DPerspective(Vector3 position, Vector3 direction, Vector3 up,
            float fov, float aspectRatio, float nearPlane, float farPlane)
        {
            this.position = position;
            this.direction = Vector3.Normalize(direction);
            this.up = Vector3.Normalize(up);
            this.fov = fov;
            this.aspectRatio = aspectRatio;
            this.nearPlane = nearPlane;
            this.farPlane = farPlane;
            viewIsDirty = true;
            projectionIsDirty = true;
        }
    }
}
