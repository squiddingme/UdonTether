
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Tether
{
    /// <summary>
    /// Holds properties for TetherControllers. Can be used to share properties between multiple TetherControllers.
    /// </summary>
    public class TetherProperties : UdonSharpBehaviour
    {
        [Header("Tether")]
        [Tooltip("Maximum length of a grapple. Also determines how far the grapple gun can shoot.")]
        public float tetherMaximumLength = 20.0f;

        [Header("Force")]
        [Tooltip("Force to tug the player back to center with. Gives the grapple a bungee-like springiness.")]
        public float tetherSpringFactor = 45.0f;
        [Tooltip("Maximum amount of force the tether will tug the player back to the center with. Use this to prevent the grapple from pulling the player too fast.")]
        public float tetherMaximumSpringForce = 10.0f;
        [Tooltip("Rate to project the player's velocity inside the grapple's sphere. Increasing this makes swinging smoother, but reduces the bounciness of the line.")]
        public float tetherProjectionRate = 4.0f;

        [Header("Physics")]
        [Tooltip("Whether the grapple should manipulate rigidbodies, rather than swing on them.")]
        public bool manipulatesRigidbodies;
        [Tooltip("Mass of the player. The player will swing on rigidbodies heavier than this.")]
        public float playerMass = 70.0f;
        [Tooltip("Force to tug rigidbodies back to center with. Gives the grapple a bungee-like springiness.")]
        public float rigidbodySpringFactor = 45.0f;
        [Tooltip("Maximum amount of force the tether will tug the rigidbody back to the center with. Use this to prevent the grapple from pulling rigidbodies too fast.")]
        public float rigidbodyMaximumSpringForce = 10.0f;
        [Tooltip("Amount to project the rigidbody's velocity inside the grapple's sphere. Increasing this makes swinging smoother, but reduces the bounciness of the line.")]
        public float rigidbodyProjectionRate = 0.25f;

        [Header("Detection")]
        [Tooltip("Layers you can grapple on.")]
        public LayerMask tetherDetectionMask;
        [Tooltip("How wide to cast a detection ray for finding objects to grapple on.")]
        public float tetherDetectionSize = 0.75f;
        [Tooltip("Number of times to cast a detection ray. Each ray's size is a division of tetherDetectionSize. Makes auto-aim more accurate.")]
        public int tetherDetectionIncrements = 2;

        [Header("Input")]
        [Tooltip("Deadzone before inputs are accepted. Controllers will probably already have their own deadzone, so this is 0 by default.")]
        [Range(0.0f, 1.0f)]
        public float tetherInputDeadzone = 0.0f;
        [Tooltip("Value of input needed to stop unreeling tether.")]
        [Range(0.0f, 1.0f)]
        public float tetherHoldDeadzone = 0.95f;
        [Tooltip("Whether to allow a tether to unwind if the trigger is pressed only half-way (below tetherHoldDeadzone)")]
        public bool allowUnwinding = true;
        [Tooltip("Maximum speed at which the tether unwinds.")]
        public float unwindRate = 2.0f;
    }
}
