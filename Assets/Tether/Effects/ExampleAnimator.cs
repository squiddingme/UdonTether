
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using Tether;

namespace Effects
{
    /// <summary>
    /// Script for the visual representation and sound effects of the example grapple device.
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class ExampleAnimator : UdonSharpBehaviour
    {
        [Tooltip("TetherController to get information from.")]
        public TetherController controller;

        [Header("Animator")]
        [Tooltip("Animator to send parameter values to.")]
        public Animator animator;
        [Tooltip("Name of animator float for displaying value of player input.")]
        public string inputParam = "Input";
        [Tooltip("Name of animator boolean to determine if the player is giving input or not.")]
        public string inputOnParam = "InputOn";
        [Tooltip("Name of animator float for displaying length of tether line.")]
        public string lengthParam = "Length";
        [Tooltip("Name of animator boolean to determine if the line is out or not.")]
        public string lengthOnParam = "LengthOn";
        [Tooltip("Name of animator boolean for displaying if the player is unreeling or not.")]
        public string heldParam = "Clamp";

        [Header("End Point Effects")]
        [Tooltip("End point game object.")]
        public GameObject endPoint;
        [Tooltip("Particle system to play when first tethering.")]
        public ParticleSystem particles;
        [Tooltip("Number of particles to emit on tether.")]
        public int particleEmission = 5;

        [Header("Sound Effects")]
        [Tooltip("Audio to play on hit.")]
        public AudioSource hitSound;
        [Tooltip("Audio to play on held.")]
        public AudioSource heldSound;
        [Tooltip("Audio to play while unwinding.")]
        public AudioSource unwindSound;
        [Tooltip("Pitch for slowest unwind speed")]
        public float unwindPitchLow = 0.5f;
        [Tooltip("Pitch for fastest unwind speed")]
        public float unwindPitchHigh = 1.0f;

        private bool tetheringDown = false;
        private bool heldDown = false;

        public void Update()
        {
            animator.SetFloat(inputParam, controller.GetInput());
            animator.SetBool(inputOnParam, controller.GetInput() > 0.0f);

            animator.SetFloat(lengthParam, controller.GetTetherLength());
            animator.SetBool(lengthOnParam, controller.GetTetherLength() > 0.0f);

            if (controller.GetTethering())
            {
                endPoint.transform.SetPositionAndRotation(controller.GetTetherPoint(), Quaternion.LookRotation(controller.GetTetherNormal()));

                if (!tetheringDown)
                {
                    particles.Emit(particleEmission);
                    hitSound.PlayOneShot(hitSound.clip);
                    tetheringDown = true;
                }

                if (!controller.IsInputHeld())
                {
                    if (!unwindSound.isPlaying)
                    {
                        unwindSound.Play();
                        unwindSound.pitch = Mathf.Lerp(unwindPitchLow, unwindPitchHigh, controller.GetTetherUnwindRate());
                    }
                }
                else
                {
                    unwindSound.Stop();
                }
            }
            else
            {
                tetheringDown = false;
                unwindSound.Stop();
            }

            animator.SetBool(heldParam, controller.IsInputHeld());

            if (controller.IsInputHeld())
            {
                if (!heldDown)
                {
                    heldSound.PlayOneShot(heldSound.clip);
                    heldDown = true;
                }
            }
            else
            {
                heldDown = false;
            }
        }
    }
}
