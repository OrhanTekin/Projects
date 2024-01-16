using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour
{
    public static GameInput Instance {get; private set;}
    private PlayerInputActions playerInputActions;

    public event EventHandler OnPauseToggled;

    private void Awake(){
        Instance = this;
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
        playerInputActions.Player.Pause.performed += Pause_performed;
    }

    private void Pause_performed(InputAction.CallbackContext obj)
    {
        OnPauseToggled?.Invoke(this, EventArgs.Empty);
    }

    public Vector2 GetMovementVectorNormalized(){
        Vector2 inputVector = playerInputActions.Player.Move.ReadValue<Vector2>();

        inputVector = inputVector.normalized;

        return inputVector;
    }

    public Vector3 GetMousePosition(){
        return Mouse.current.position.ReadValue();
    }

    public float GetTestFire()
    {
        return playerInputActions.Player.TestFire.ReadValue<float>();
    }
}
