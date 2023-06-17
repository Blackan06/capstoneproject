using UnityEngine;

public class PlayerControllerMobile : MonoBehaviour
{
    [SerializeField] private FixedJoystick _joystick;

    [SerializeField] private CharacterController characterController;

    private void FixedUpdate()
    {
        characterController.SetMovingFromJoytick(_joystick.Horizontal, _joystick.Vertical);
    }
}