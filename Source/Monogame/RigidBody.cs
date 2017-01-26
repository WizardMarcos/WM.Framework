using Microsoft.Xna.Framework;
using System;

namespace WM.Framework.Monogame
{
    public class RigidBody
    {
        private Vector3 position;
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

        private Matrix orientation;
        public Matrix Orientation
        {
            get
            {
                return orientation;
            }
            set
            {
                orientation = value;
            }
        }

        private Vector3 velocity;
        public Vector3 Velocity
        {
            get
            {
                return velocity;
            }
            set
            {
                velocity = value;
            }
        }

        private Vector3 rotationAxis;
        private float rotationSpeed;

        public Vector3 AngularVelocity
        {
            get
            {
                return Vector3.Multiply(rotationAxis, rotationSpeed);
            }
            set
            {
                rotationSpeed = value.Length();
                rotationAxis = Vector3.Divide(rotationAxis, rotationSpeed);
            }
        }

        private Matrix inertiaMatrix;
        public Matrix InertiaMatrix
        {
            get
            {
                return inertiaMatrix;
            }
        }

        // This stores the position of the centre of mass relative to the position.
        // This way, the position of the object that has the rigid body is the same
        // as the position of the rigid body. 
        private Vector3 centreOfMass;
        public Vector3 CentreOfMass
        {
            get
            {
                return centreOfMass;
            }
            set
            {
                centreOfMass = value;
            }
        }

        // The rigid body's weight.
        private float mass;
        public float Mass
        {
            get
            {
                return mass;
            }
            set
            {
                mass = value;
            }
        }

        // How much a rigid body will bounce when it hits a surface. Its values
        // should be within the interval 0-1, 0 being no bounce and 1 being
        // bounce with no energy loss.
        private float bounciness;
        public float Bounciness
        {
            get
            {
                return bounciness;
            }
            set
            {
                bounciness = value;
            }
        }

        // Drag is applied at all times. A value of 0 will make a rigid body not
        // lose any velocity while moving through space.
        private float drag;
        public float Drag
        {
            get
            {
                return drag;
            }
            set
            {
                drag = value;
            }
        }

        // Angular drag is similar to drag but is applied to the rotation speed
        // instead.
        private float angularDrag;
        public float AngularDrag
        {
            get
            {
                return angularDrag;
            }
            set
            {
                angularDrag = value;
            }
        }

        // Friction is applied while a rigid body is touching another one.
        private float friction;
        public float Friction
        {
            get
            {
                return friction;
            }
            set
            {
                friction = value;
            }
        }

        /// <summary>
        /// The translational kinetic energy of the rigid body (no rotational energy).
        /// </summary>
        public float TranslationalKineticEnergy
        {
            get
            {
                return 0.5f * mass * velocity.LengthSquared();
            }
        }

        /// <summary>
        /// The rotational kinetic energy of the rigid body (no translational energy).
        /// </summary>
        public float RotationalKineticEnergy
        {
            get
            {
                throw new NotImplementedException();
                return 0.5f * mass * AngularVelocity.LengthSquared();
            }
        }

        /// <summary>
        /// The total kinetic energy of the rigid body.
        /// </summary>
        public float KineticEnergy
        {
            get
            {
                return TranslationalKineticEnergy + RotationalKineticEnergy;
            }
        }

        public RigidBody()
        {
            throw new NotImplementedException();

            velocity = Vector3.Zero;
            rotationSpeed = 0f;
            rotationAxis = Vector3.Zero;
        }
    }
}
