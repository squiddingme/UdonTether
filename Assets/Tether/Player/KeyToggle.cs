
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Player
{
    /// <summary>
    /// Basic gameobject toggle on key press script.
    /// </summary>
    public class KeyToggle : UdonSharpBehaviour
    {
        [Tooltip("Key that toggles gameobjects.")]
        public KeyCode key;
        [Tooltip("List of game objects to toggle on/off.")]
        public GameObject[] toggleObject;

        public void Update()
        {
            if (Input.GetKeyDown(key))
            {
                for (int i = 0; i < toggleObject.Length; i++)
                {
                    toggleObject[i].SetActive(!toggleObject[i].activeSelf);
                }
            }
        }
    }
}