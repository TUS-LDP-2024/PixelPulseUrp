using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
    public class StarterAssetsInputs : MonoBehaviour
    {
        [Header("Character Input Values")]
        public Vector2 move;
        public Vector2 look;
        public bool jump;
        public bool sprint;
        public bool fire; // State for firing a weapon
        public bool throwGrenade; // State for throwing a grenade
        public bool activateButton; // State for activating a button or interacting with objects
        public bool interact; // State for interacting with objects or NPCs
        public bool Aim; // State for aiming down sights or focusing
        public bool reload; // State for reloading the weapon
        public bool switchWeapon; // State for switching weapons

        [Header("Movement Settings")]
        public bool analogMovement;

        [Header("Mouse Cursor Settings")]
        public bool cursorLocked = true;
        public bool cursorInputForLook = true;

#if ENABLE_INPUT_SYSTEM
        public void OnMove(InputValue value)
        {
            MoveInput(value.Get<Vector2>());
        }

        public void OnLook(InputValue value)
        {
            if (cursorInputForLook)
            {
                LookInput(value.Get<Vector2>());
            }
        }

        public void OnJump(InputValue value)
        {
            JumpInput(value.isPressed);
        }

        public void OnSprint(InputValue value)
        {
            SprintInput(value.isPressed);
        }

        public void OnFire(InputValue value)
        {
            FireInput(value.isPressed);
        }

        public void OnThrowGrenade(InputValue value)
        {
            ThrowGrenadeInput(value.isPressed);
        }

        public void OnAim(InputValue value)
        {
            AimInput(value.isPressed);
        }

        public void OnInteract(InputValue value)
        {
            InteractInput(value.isPressed);
        }

        public void OnReload(InputValue value)
        {
            ReloadInput(value.isPressed);
        }

        public void OnSwitchWeapon(InputValue value)
        {
            SwitchWeaponInput(value.isPressed);
        }
#endif

        public void MoveInput(Vector2 newMoveDirection)
        {
            move = newMoveDirection;
        }

        public void LookInput(Vector2 newLookDirection)
        {
            look = newLookDirection;
        }

        public void JumpInput(bool newJumpState)
        {
            jump = newJumpState;
        }

        public void SprintInput(bool newSprintState)
        {
            sprint = newSprintState;
        }

        public void FireInput(bool newFireState)
        {
            fire = newFireState; // Update the firing state based on player input
        }

        public void ThrowGrenadeInput(bool newThrowGrenadeState)
        {
            throwGrenade = newThrowGrenadeState; // Update the grenade throwing state based on player input
        }

        public void AimInput(bool newAimState)
        {
            Aim = newAimState; // Update the aiming state based on player input
        }

        public void InteractInput(bool newActivateInteractState)
        {
            activateButton = newActivateInteractState; // Update the button activation state
            interact = newActivateInteractState; // Update the interaction state
        }

        public void ReloadInput(bool newReloadState)
        {
            reload = newReloadState; // Update the reloading state based on player input
        }

        public void SwitchWeaponInput(bool newSwitchWeaponState)
        {
            switchWeapon = newSwitchWeaponState; // Update the weapon switching state based on player input
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            SetCursorState(cursorLocked);
        }

        private void SetCursorState(bool newState)
        {
            Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
        }
    }
}