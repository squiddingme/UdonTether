
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using Tether;

namespace Effects
{
    /// <summary>
    /// Basic example for rendering a tether using a line renderer.
    /// </summary>
    [RequireComponent(typeof(LineRenderer))]
    public class ExampleLineRenderer : UdonSharpBehaviour
    {
        [Tooltip("TetherController to get information from.")]
        public TetherController controller;

        [Tooltip("A line renderer, required for visualizing a grapple.")]
        public LineRenderer line;

        public void LateUpdate()
        {
            if (controller.GetTethering())
            {
                line.enabled = true;

                Vector3 worldTetherPoint = controller.GetTetherPoint();
                line.SetPosition(0, controller.GetTetherStartPoint());
                line.SetPosition(1, Vector3.Lerp(controller.GetTetherStartPoint(), worldTetherPoint, 0.5f));
                line.SetPosition(2, worldTetherPoint);
            }
            else
            {
                line.enabled = false;
            }
        }

        private void OnDisable()
        {
            line.enabled = false;
        }
    }
}
