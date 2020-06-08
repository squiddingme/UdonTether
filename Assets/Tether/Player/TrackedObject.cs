
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Player
{
    /// <summary>
    /// Script to track objects to the player's hands or head.
    /// </summary>
    public class TrackedObject : UdonSharpBehaviour
    {

        [Tooltip("Whether this TrackedObject should be active if you are in desktop mode or VR mode.")]
        public bool vrEnabled;
        [Tooltip("Which GameObject to enable if vrEnabled matches which mode we are in.")]
        public GameObject vrEnabledObject;
        [Tooltip("Which tracking point to attach this object to.")]
        public VRCPlayerApi.TrackingDataType trackingType;

        private bool editorMode = true;
        private VRCPlayerApi localPlayer;

        public void Start()
        {
            localPlayer = Networking.LocalPlayer;

            if (localPlayer != null)
            {
                editorMode = false;

                vrEnabledObject.SetActive(vrEnabled == localPlayer.IsUserInVR());
            }
        }

        public void Update()
        {
            if (!editorMode && vrEnabled == localPlayer.IsUserInVR())
            {
                VRCPlayerApi.TrackingData data = localPlayer.GetTrackingData(trackingType);
                transform.SetPositionAndRotation(data.position, data.rotation);
            }
        }
    }
}