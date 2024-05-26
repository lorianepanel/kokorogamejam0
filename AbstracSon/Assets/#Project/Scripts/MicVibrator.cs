using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MicVibrator : MonoBehaviour
{

    public GameObject vibrator;

    public OutputAudioRecorder4 recorder;

    private MicInput inputActions; 

    public bool visible = false;

    private void Awake()
    {
        inputActions = new MicInput();
        vibrator.SetActive(false);
        Debug.Log("awake");
    }

    private void OnEnable()
    {
        inputActions.Mic.ShowHide.performed += OnInteract;
        inputActions.Mic.Enable();
        // Debug.Log("enable");
    }



    private void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (visible == false)
            {
                vibrator.SetActive(true);
                recorder.StartRecording();
                // Debug.Log("Toggled vibrator. New state: visible");
                visible = true;
            }
            else
            {
                vibrator.SetActive(false);
                recorder.PauseRecording();
                // Debug.Log("Toggled vibrator. New state: invisble");
                visible = false;
            }
        }
    }

}
