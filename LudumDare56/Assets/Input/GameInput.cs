using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour
{
    private InputActions _inputActions;
    private Camera _mainCamera;

    [Header("Input Values")] 
    [SerializeField] private Vector2 moveNormalized;
    [SerializeField] private Vector2 lookNormalized;
    [SerializeField] private Vector2 primaryPositionScreen;
    [SerializeField] private Vector3 primaryPositionWorld;
    [SerializeField] private Vector2 primaryDeltaNormalized;
    
    
    [Header("Debug Toggles")]
    [SerializeField] private bool logMove;
    [SerializeField] private bool logLook;
    [SerializeField] private bool logPrimary;
    [SerializeField] private bool logPrimaryPosition;
    [SerializeField] private bool logPrimaryDelta;

    void Awake()
    {
        OnEnable();
    }
    
    void OnEnable()
    {
        _mainCamera ??= Camera.main;
        
        // If null - create one.
        _inputActions ??= new InputActions();
        
        _inputActions.Enable();
        _inputActions.Player.Move.performed += Move;
        _inputActions.Player.Move.canceled += Move;
        _inputActions.Player.Look.performed += Look;
        _inputActions.Player.Look.canceled += Look;
        _inputActions.Player.Primary.performed += PrimaryDown;
        _inputActions.Player.Primary.canceled += PrimaryUp;
        _inputActions.Player.PrimaryPosition.performed += PrimaryPositionScreen;
        _inputActions.Player.PrimaryPosition.canceled += PrimaryPositionScreen;
        _inputActions.Player.PrimaryDelta.performed += PrimaryDelta;
        _inputActions.Player.PrimaryDelta.canceled += PrimaryDelta;
    }
    
    private void OnDisable()
    {
        _inputActions.Player.Move.performed -= Move;
        _inputActions.Player.Move.canceled -= Move;
        _inputActions.Player.Look.performed -= Look;
        _inputActions.Player.Look.canceled -= Look;
        _inputActions.Player.Primary.performed -= PrimaryDown;
        _inputActions.Player.Primary.canceled -= PrimaryUp;
        _inputActions.Player.PrimaryPosition.performed -= PrimaryPositionScreen;
        _inputActions.Player.PrimaryPosition.canceled -= PrimaryPositionScreen;
        _inputActions.Player.PrimaryDelta.performed -= PrimaryDelta;
        _inputActions.Player.PrimaryDelta.canceled -= PrimaryDelta;

        _inputActions.Disable();
    }
    
    private void Move(InputAction.CallbackContext ctxVector2)
    {
        Vector2 direction = ctxVector2.ReadValue<Vector2>().normalized;
        
        if(logMove) Debug.Log($"Move: {direction}");

        moveNormalized = direction;
    }
    
    private void Look(InputAction.CallbackContext ctxVector2)
    {
        Vector2 direction = ctxVector2.ReadValue<Vector2>().normalized;
        if(logLook) Debug.Log($"Move: {direction}");

        lookNormalized = direction;
    }

    private void PrimaryDown(InputAction.CallbackContext ctxButton)
    {
        if(logPrimary) Debug.Log($"Primary Input Down (Performed)");
    }
    private void PrimaryUp(InputAction.CallbackContext ctxButton)
    {
        if(logPrimary) Debug.Log($"Primary Input Up (Cancelled)");
    }

    private void PrimaryPositionScreen(InputAction.CallbackContext ctxVector2)
    {
        Vector2 screenPosition = ctxVector2.ReadValue<Vector2>();
        if(logPrimaryPosition) Debug.Log($"Primary Position (Screen): {screenPosition}");
        
        // Also send this to world.
        PrimaryPositionWorld(screenPosition);

        primaryPositionScreen = screenPosition;
    }

    private void PrimaryPositionWorld(Vector2 posScreen)
    {
        Vector3 posWorld = _mainCamera.ScreenToWorldPoint(posScreen);
        
        if(logPrimaryPosition) Debug.Log($"Primary Position (World): {posWorld}");

        primaryPositionWorld = posWorld;
    }

    private void PrimaryDelta(InputAction.CallbackContext ctxVector2)
    {
        Vector2 deltaDirection = ctxVector2.ReadValue<Vector2>().normalized;
        if(logPrimaryDelta) Debug.Log($"Primary Delta: {deltaDirection}");

        primaryDeltaNormalized = deltaDirection;
    }

    

    

    

}
