using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WM.Framework.Monogame
{
    // Just writing this here so it is seen.
    // There is still a bug with the camera, if you look up or
    // down, and the pitch you add makes it so you turn around,
    // the camera will quickly flip. I'm trying to come up with
    // a fix, meanwhile I'll open an issue in the repo.
    // Also, about the invert functions. I'm not really sure
    // if they are even needed. I just felt like adding them,
    // they can be completely wrong.

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
                viewDirty = true;
            }
        }
        public void ChangePosition(Vector3 position)
        {
            this.position = position;
            viewDirty = true;
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
                viewDirty = true;
            }
        }
        public void ChangeDirection(Vector3 direction)
        {
            this.direction = Vector3.Normalize(direction);
            viewDirty = true;
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
                viewDirty = true;
            }
        }
        public void ChangeUpVector(Vector3 up)
        {
            this.up = Vector3.Normalize(up);
            viewDirty = true;
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
                projectionDirty = true;
            }
        }
        public void SetNearPlane(float nearPlane)
        {
            this.nearPlane = nearPlane;
            projectionDirty = true;
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
                projectionDirty = true;
            }
        }
        public void SetFarPlane(float farPlane)
        {
            this.farPlane = farPlane;
            projectionDirty = true;
        }

        #endregion

        // The View and Projection Matrices.
        // I would reccomend you to read RB Whitaker's matrix explanation
        // to understand what these matrices are and why they are needed.
        // http://rbwhitaker.wikidot.com/monogame-basic-matrices
        #region VIEW MATRIX

        protected bool viewDirty;
        private Matrix viewMatrix;
        public Matrix ViewMatrix
        {
            get
            {
                if (viewDirty)
                {
                    viewMatrix = Matrix.CreateLookAt(position, position + direction, up);
                    viewDirty = false;
                }
                return viewMatrix;
            }
        }

        #endregion
        #region PROJECTION MATRIX

        protected bool projectionDirty;
        protected Matrix projectionMatrix;
        public abstract Matrix ProjectionMatrix { get; }

        #endregion

        #region MOVEMENT

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
            viewDirty = true;
        }

        /// <summary>
        /// Moves the camera into the direction it is pointing to.
        /// </summary>
        /// <param name="amount">The amount to move.</param>
        public void Thrust(float amount)
        {
            // The direction is a normalized vector, so we can simply multiply it
            // by an amount and it properly thrusts the camera.
            position += direction * amount;
            viewDirty = true;
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
            viewDirty = true;
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
            viewDirty = true;
        }

        /// <summary>
        /// Strafes the camera forwards in its direction but perpendicular to the
        /// up vector.
        /// </summary>
        /// <param name="amount">The amount to strafe.</param>
        public void StrafeForwards(float amount)
        {
            // This one is a bit more complex, as we want to find the forward
            // vector in relation to both the up and direction vectors.
            // It will be the cross product of the horizontal and up vectors.
            // Also, we need to flip the amount, because positive is actually backwards.
            position += Vector3.Normalize(Vector3.Cross(up, Vector3.Cross(up, direction))) * -amount;
            viewDirty = true;
        }

        /// <summary>
        /// Raises the camera in relation to its direction.
        /// </summary>
        /// <param name="amount">The amount to raise.</param>
        public void Raise(float amount)
        {
            // Similar to forward straffing, but we use the direction vector
            // instead of the up to go upwards in relation to where it is
            // pointing at.
            position += Vector3.Normalize(Vector3.Cross(direction, Vector3.Cross(up, direction))) * amount;
            viewDirty = true;
        }

        /*
        /// <summary>
        /// Follows a target, keeping the camera's up vector.
        /// </summary>
        /// <param name="target">The target to look at.</param>
        /// <param name="targetDirection">The target's direction.</param>
        /// <param name="relativeCameraDirection">The camera's direction relative to the target.</param>
        /// <param name="distance">The camera's distance to the target.</param>
        public void FollowUpright(Vector3 target, Vector3 targetDirection, Vector3 relativeCameraDirection, float distance = 1)
        {
            // This was a bit of a pain to write. Still, it works, which is nice.
            // The camera will keep its up vector, and follow a target around.
            direction = Vector3.Normalize(-Vector3.TransformNormal(relativeCameraDirection, 
                Matrix.CreateLookAt(Vector3.Zero, targetDirection, -up)));
            position = target - direction * distance;
            viewIsDirty = true;
        }
        */

        #endregion

        #region ROTATION

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
            viewDirty = true;
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

            viewDirty = true;
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

        #endregion

        #region MOUSE AND KEYBOARD CONTROLS

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

        #endregion

        #region OTHER

        /// <summary>
        /// Zooms the specified amount.
        /// </summary>
        /// <param name="amount"></param>
        public abstract void Zoom(float amount);

        /// <summary>
        /// Rotates the camera so it points to the target.
        /// </summary>
        /// <param name="target">Where the camera should look at.</param>
        public void LookAt(Vector3 target)
        {
            // The direction is calculated from the target and position.
            direction = Vector3.Normalize(target - position);
            viewDirty = true;
        }

        #endregion
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
                projectionDirty = true;
            }
        }
        public void SetFov(float fov)
        {
            this.fov = fov;
            projectionDirty = true;
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
                projectionDirty = true;
            }
        }
        public void SetAspectRatio(float aspectRatio)
        {
            this.aspectRatio = aspectRatio;
            projectionDirty = true;
        }

        #endregion

        #region PROJECTION MATRIX

        // Method to override the creation of the projection matrix.
        public override Matrix ProjectionMatrix
        {
            get
            {
                if (projectionDirty)
                {
                    // This camera creates the projection matrix using the field of view,
                    // aspect ratio and the near plane and far plane distances.
                    projectionMatrix = Matrix.CreatePerspectiveFieldOfView(fov, aspectRatio, NearPlane, FarPlane);
                    projectionDirty = false;
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
            viewDirty = true;
            projectionDirty = true;
        }

        public override void Zoom(float amount)
        {
            fov /= amount;
            projectionDirty = true;
        }

        /// <summary>
        /// Converts this <see cref="Camera3DPerspective"/> in a <see cref="Camera3DPerspectiveOffCentre"/>.
        /// </summary>
        /// <returns></returns>
        public Camera3DPerspectiveOffCentre ToOffCentrePerspective()
        {
            float height = (float)Math.Tan(fov * 0.5f);
            return new Camera3DPerspectiveOffCentre(position, direction, up, height * aspectRatio, height, nearPlane, farPlane);
        }
    }

    public class Camera3DPerspectiveOffCentre : Camera3D
    {
        // Left, Right, Bottom and Top.
        // These influence the Projection Matrix.
        // They define the area of the projection.
        #region LEFT

        private float left;
        public float Left
        {
            get
            {
                return left;
            }
            set
            {
                left = value;
                projectionDirty = true;
            }
        }
        public void SetLeft(float left)
        {
            this.left = left;
            projectionDirty = true;
        }

        #endregion
        #region RIGHT

        private float right;
        public float Tight
        {
            get
            {
                return right;
            }
            set
            {
                right = value;
                projectionDirty = true;
            }
        }
        public void SetRight(float right)
        {
            this.right = right;
            projectionDirty = true;
        }

        #endregion
        #region BOTTOM

        private float bottom;
        public float Bottom
        {
            get
            {
                return bottom;
            }
            set
            {
                bottom = value;
                projectionDirty = true;
            }
        }
        public void SetBottom(float bottom)
        {
            this.bottom = bottom;
            projectionDirty = true;
        }

        #endregion
        #region TOP

        private float top;
        public float Top
        {
            get
            {
                return top;
            }
            set
            {
                top = value;
                projectionDirty = true;
            }
        }
        public void SetTop(float top)
        {
            this.top = top;
            projectionDirty = true;
        }

        #endregion

        // These are obtained from the previous variables.
        #region WIDTH AND HEIGHT

        public float Width
        {
            get
            {
                return right - left;
            }
            set
            {
                float f = value / Width;
                left *= f;
                right *= f;
                projectionDirty = true;
            }
        }
        public float Height
        {
            get
            {
                return top - bottom;
            }
            set
            {
                float f = value / Height;
                top *= f;
                bottom *= f;
                projectionDirty = true;
            }
        }

        #endregion

        #region PROJECTION MATRIX

        // Method to override the creation of the projection matrix.
        public override Matrix ProjectionMatrix
        {
            get
            {
                if (projectionDirty)
                {
                    projectionMatrix = Matrix.CreatePerspectiveOffCenter(left, right, bottom, top, nearPlane, farPlane);
                    projectionDirty = false;
                }
                return projectionMatrix;
            }
        }

        #endregion

        /// <summary>
        /// Creates a <see cref="Camera3D"/> using a perspective projection, with a free projection area.
        /// </summary>
        /// <param name="position">Position of the camera.</param>
        /// <param name="direction">Direction to where the camera is looking.</param>
        /// <param name="up">The Up vector.</param>
        /// <param name="left">Left position.</param>
        /// <param name="right">Right position.</param>
        /// <param name="bottom">Bottom position.</param>
        /// <param name="top">Top position.</param>
        /// <param name="nearPlane">The closest distance at which objects are rendered.</param>
        /// <param name="farPlane">The farthest distance at which objects are rendered.</param>
        public Camera3DPerspectiveOffCentre(Vector3 position, Vector3 direction, Vector3 up,
            float left, float right, float bottom, float top, float nearPlane, float farPlane)
        {
            this.position = position;
            this.direction = Vector3.Normalize(direction);
            this.up = Vector3.Normalize(up);
            this.left = left;
            this.right = right;
            this.bottom = bottom;
            this.top = top;
            this.nearPlane = nearPlane;
            this.farPlane = farPlane;
            viewDirty = true;
            projectionDirty = true;
        }

        /// <summary>
        /// Creates a <see cref="Camera3D"/> using a perspective projection, with a free centred projection area.
        /// </summary>
        /// <param name="position">Position of the camera.</param>
        /// <param name="direction">Direction to where the camera is looking.</param>
        /// <param name="up">The Up vector.</param>
        /// <param name="width">The width of the area.</param>
        /// <param name="height">The height of the area.</param>
        /// <param name="nearPlane">The closest distance at which objects are rendered.</param>
        /// <param name="farPlane">The farthest distance at which objects are rendered.</param>
        public Camera3DPerspectiveOffCentre(Vector3 position, Vector3 direction, Vector3 up,
            float width, float height, float nearPlane, float farPlane)
        {
            this.position = position;
            this.direction = Vector3.Normalize(direction);
            this.up = Vector3.Normalize(up);
            this.left = width / -2f;
            this.right = left + width;
            this.bottom = height / -2f;
            this.top = bottom + height;
            this.nearPlane = nearPlane;
            this.farPlane = farPlane;
            viewDirty = true;
            projectionDirty = true;
        }

        public override void Zoom(float amount)
        {
            top /= amount;
            left /= amount;
            right /= amount;
            bottom /= amount;
            projectionDirty = true;
        }

        /// <summary>
        /// Inverts the projection around 0;
        /// </summary>
        public void InvertZero()
        {
            top = -top;
            left = -left;
            right = -right;
            bottom = -bottom;
            projectionDirty = true;
        }

        /// <summary>
        /// Inverts the projection.
        /// </summary>
        public void Invert()
        {
            float f = top;
            top = bottom;
            bottom = f;
            f = left;
            left = right;
            right = f;
        }

        /// <summary>
        /// Offsets the centre of the camera.
        /// </summary>
        /// <param name="offset"></param>
        public void Offset(Vector2 offset)
        {
            Offset(offset.X, offset.Y);
        }

        /// <summary>
        /// Offsets the centre of the camera.
        /// </summary>
        /// <param name="h">Horizontal offset.</param>
        /// <param name="v">Vertical offset.</param>
        public void Offset(float h, float v)
        {
            top += v;
            left += h;
            right += h;
            bottom += v;
        }
    }

    public class Camera3DOrthographic : Camera3D
    {
        // Width and Height.
        // These influence the Projection Matrix.
        // Unlike perspective projections, orthographic ones do not
        // have a fov. Since they don't have one, they need values for
        // the width and height of the projection, which can't be
        // calculated from an aspect ratio.
        #region WIDTH

        private float width;
        public float Width
        {
            get
            {
                return width;
            }
            set
            {
                width = value;
                projectionDirty = true;
            }
        }
        public void SetWidth(float width)
        {
            this.width = width;
            projectionDirty = true;
        }

        #endregion
        #region HEIGHT

        private float height;
        public float Height
        {
            get
            {
                return height;
            }
            set
            {
                height = value;
                projectionDirty = true;
            }
        }
        public void SetHeight(float height)
        {
            this.height = height;
            projectionDirty = true;
        }

        #endregion

        #region PROJECTION MATRIX

        // Method to override the creation of the projection matrix.
        public override Matrix ProjectionMatrix
        {
            get
            {
                if (projectionDirty)
                {
                    projectionMatrix = Matrix.CreateOrthographic(width, height, nearPlane, farPlane);
                    projectionDirty = false;
                }
                return projectionMatrix;
            }
        }

        #endregion

        /// <summary>
        /// Creates a <see cref="Camera3D"/> using an orthographic projection.
        /// </summary>
        /// <param name="position">Position of the camera.</param>
        /// <param name="direction">Direction to where the camera is looking.</param>
        /// <param name="up">The Up vector.</param>
        /// <param name="width">The width of the projection.</param>
        /// <param name="height">The height of the projection.</param>
        /// <param name="nearPlane">The closest distance at which objects are rendered.</param>
        /// <param name="farPlane">The farthest distance at which objects are rendered.</param>
        /// <remarks>Setting the height to X and the width to X * Aspect Ratio will make a
        /// properly scaled projection matrix.</remarks>
        public Camera3DOrthographic(Vector3 position, Vector3 direction, Vector3 up,
            float width, float height, float nearPlane, float farPlane)
        {
            this.position = position;
            this.direction = Vector3.Normalize(direction);
            this.up = Vector3.Normalize(up);
            this.width = width;
            this.height = height;
            this.nearPlane = nearPlane;
            this.farPlane = farPlane;
            viewDirty = true;
            projectionDirty = true;
        }

        public override void Zoom(float amount)
        {
            width /= amount;
            height /= amount;
            projectionDirty = true;
        }

        /// <summary>
        /// Transforms this <see cref="Camera3DOrthographic"/> in a <see cref="Camera3DOrthographicOffCentre"/>;
        /// </summary>
        /// <returns></returns>
        public Camera3DOrthographicOffCentre ToOrthographicOffCentre()
        {
            return new Camera3DOrthographicOffCentre(position, direction, up, width, height, nearPlane, farPlane);
        }

        /// <summary>
        /// Inverts the projection.
        /// </summary>
        public void Invert()
        {
            height = -height;
            width = -width;
            projectionDirty = true;
        }
    }

    public class Camera3DOrthographicOffCentre : Camera3D
    {
        // Left, Right, Bottom and Top.
        // These influence the Projection Matrix.
        // They define the area of the projection.
        #region LEFT

        private float left;
        public float Left
        {
            get
            {
                return left;
            }
            set
            {
                left = value;
                projectionDirty = true;
            }
        }
        public void SetLeft(float left)
        {
            this.left = left;
            projectionDirty = true;
        }

        #endregion
        #region RIGHT

        private float right;
        public float Tight
        {
            get
            {
                return right;
            }
            set
            {
                right = value;
                projectionDirty = true;
            }
        }
        public void SetRight(float right)
        {
            this.right = right;
            projectionDirty = true;
        }

        #endregion
        #region BOTTOM

        private float bottom;
        public float Bottom
        {
            get
            {
                return bottom;
            }
            set
            {
                bottom = value;
                projectionDirty = true;
            }
        }
        public void SetBottom(float bottom)
        {
            this.bottom = bottom;
            projectionDirty = true;
        }

        #endregion
        #region TOP

        private float top;
        public float Top
        {
            get
            {
                return top;
            }
            set
            {
                top = value;
                projectionDirty = true;
            }
        }
        public void SetTop(float top)
        {
            this.top = top;
            projectionDirty = true;
        }

        #endregion

        #region WIDTH AND HEIGHT

        public float Width
        {
            get
            {
                return right - left;
            }
            set
            {
                float f = value / Width;
                left *= f;
                right *= f;
                projectionDirty = true;
            }
        }
        public float Height
        {
            get
            {
                return top - bottom;
            }
            set
            {
                float f = value / Height;
                top *= f;
                bottom *= f;
                projectionDirty = true;
            }
        }

        #endregion

        #region PROJECTION MATRIX

        // Method to override the creation of the projection matrix.
        public override Matrix ProjectionMatrix
        {
            get
            {
                if (projectionDirty)
                {
                    projectionMatrix = Matrix.CreateOrthographicOffCenter(left, right, bottom, top, nearPlane, farPlane);
                    projectionDirty = false;
                }
                return projectionMatrix;
            }
        }

        #endregion

        /// <summary>
        /// Creates a <see cref="Camera3D"/> using an orthographic projection, with a free projection area.
        /// </summary>
        /// <param name="position">Position of the camera.</param>
        /// <param name="direction">Direction to where the camera is looking.</param>
        /// <param name="up">The Up vector.</param>
        /// <param name="left">Left position.</param>
        /// <param name="right">Right position.</param>
        /// <param name="bottom">Bottom position.</param>
        /// <param name="top">Top position.</param>
        /// <param name="nearPlane">The closest distance at which objects are rendered.</param>
        /// <param name="farPlane">The farthest distance at which objects are rendered.</param>
        public Camera3DOrthographicOffCentre(Vector3 position, Vector3 direction, Vector3 up,
            float left, float right, float bottom, float top, float nearPlane, float farPlane)
        {
            this.position = position;
            this.direction = Vector3.Normalize(direction);
            this.up = Vector3.Normalize(up);
            this.left = left;
            this.right = right;
            this.bottom = bottom;
            this.top = top;
            this.nearPlane = nearPlane;
            this.farPlane = farPlane;
            viewDirty = true;
            projectionDirty = true;
        }

        /// <summary>
        /// Creates a <see cref="Camera3D"/> using an orthographic projection, with a free centred projection area.
        /// </summary>
        /// <param name="position">Position of the camera.</param>
        /// <param name="direction">Direction to where the camera is looking.</param>
        /// <param name="up">The Up vector.</param>
        /// <param name="width">The width of the area.</param>
        /// <param name="height">The height of the area.</param>
        /// <param name="nearPlane">The closest distance at which objects are rendered.</param>
        /// <param name="farPlane">The farthest distance at which objects are rendered.</param>
        public Camera3DOrthographicOffCentre(Vector3 position, Vector3 direction, Vector3 up,
            float width, float height, float nearPlane, float farPlane)
        {
            this.position = position;
            this.direction = Vector3.Normalize(direction);
            this.up = Vector3.Normalize(up);
            this.left = width / -2f;
            this.right = left + width;
            this.bottom = height / -2f;
            this.top = bottom + height;
            this.nearPlane = nearPlane;
            this.farPlane = farPlane;
            viewDirty = true;
            projectionDirty = true;
        }

        public override void Zoom(float amount)
        {
            top /= amount;
            left /= amount;
            right /= amount;
            bottom /= amount;
            projectionDirty = true;
        }

        /// <summary>
        /// Inverts the projection around 0;
        /// </summary>
        public void InvertZero()
        {
            top = -top;
            left = -left;
            right = -right;
            bottom = -bottom;
            projectionDirty = true;
        }

        /// <summary>
        /// Inverts the projection.
        /// </summary>
        public void Invert()
        {
            float f = top;
            top = bottom;
            bottom = f;
            f = left;
            left = right;
            right = f;
        }

        /// <summary>
        /// Offsets the centre of the camera.
        /// </summary>
        /// <param name="offset"></param>
        public void Offset(Vector2 offset)
        {
            Offset(offset.X, offset.Y);
        }

        /// <summary>
        /// Offsets the centre of the camera.
        /// </summary>
        /// <param name="h">Horizontal offset.</param>
        /// <param name="v">Vertical offset.</param>
        public void Offset(float h, float v)
        {
            top += v;
            left += h;
            right += h;
            bottom += v;
        }
    }
}
