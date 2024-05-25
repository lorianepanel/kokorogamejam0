using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MicVibrator : MonoBehaviour
{

    /*

    public GameObject vibrator;
    public bool visible;

    private MicInput inputActions; 

    private void Awake()
    {
        inputActions = new MicInput();
        visible = true;
        Debug.Log("awake");

    }

    private void OnEnable()
    {
        inputActions.Mic.ShowHide.performed += OnInteract;
        inputActions.Mic.Enable();
 
        //Debug.Log("enable");
    }

    private void OnDisable()
    {
        inputActions.Mic.ShowHide.performed -= OnInteract;
        inputActions.Mic.Disable();

        //Debug.Log("disable");
    }


    private void OnInteract(InputAction.CallbackContext context)
    {
        if (visible == true)
        {
            vibrator.SetActive(false);
            Debug.Log("enable");
            visible = false;

        }

       
        else
        {
            vibrator.SetActive(true);
            Debug.Log("disable");
            visible = true;

        }
        

        //vibrator.SetActive(vibrator.activeSelf);
    }
    */



    public GameObject vibrator;

    private MicInput inputActions; 

    private bool visible = true;

    private void Awake()
    {
        inputActions = new MicInput();
        Debug.Log("awake");
    }

    private void OnEnable()
    {
        inputActions.Mic.ShowHide.performed += OnInteract;
        inputActions.Mic.Enable();
        Debug.Log("enable");
    }

    // private void OnDisable()
    // {
    //     inputActions.Mic.ShowHide.performed += OnInteract;
    //     inputActions.Mic.Disable();
    //     Debug.Log("disable");
    // }

    private void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (visible == false)
            {
                vibrator.SetActive(true);
                Debug.Log("Toggled vibrator. New state: visible");
                visible = true;
            }
            else
            {
                vibrator.SetActive(false);
                Debug.Log("Toggled vibrator. New state: invisble");
                visible = false;
            }
        }
    }

}
