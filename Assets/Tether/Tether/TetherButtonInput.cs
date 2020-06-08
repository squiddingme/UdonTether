
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Tether
{
    /// <summary>
    /// Basic TetherController input example that reads an input directly. Combine with TrackedObject.
    /// </summary>
    public class TetherButtonInput : UdonSharpBehaviour
    {
        [Tooltip("TetherController script.")]
        public TetherController controller;

        [Tooltip("Input to read and send to tether controller.")]
        public string tetherInput;

        public void Update()
        {
            float input = Input.GetButton(tetherInput) == true ? 1.0f : 0.0f;
            controller.SetInput(input);
        }

        private void OnDisable()
        {
            controller.SetInput(0.0f);
        }
    }
}