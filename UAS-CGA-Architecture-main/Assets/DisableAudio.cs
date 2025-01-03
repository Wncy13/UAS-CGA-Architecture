using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableAudio : MonoBehaviour
{
    // Start is called before the first frame update
    void Start() {
        AudioListener[] listeners = FindObjectsOfType<AudioListener>();
        foreach (AudioListener listener in listeners) {
            if (listener != GetComponent<AudioListener>()) {
                listener.enabled = false;  // Disable other AudioListeners
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
