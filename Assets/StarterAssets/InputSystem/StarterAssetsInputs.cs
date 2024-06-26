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

		[Header("Movement Settings")]
		public bool analogMovement;

		[Header("Mouse Cursor Settings")]
		public bool cursorLocked = true;
		public bool cursorInputForLook = true;

		GameManager gameManager;

        private void Start()
        {
            gameManager = GameManager.Instance;
        }

#if ENABLE_INPUT_SYSTEM
        public void OnMove(InputAction.CallbackContext context)
		{
			MoveInput(context.ReadValue<Vector2>());
		}

		public void OnLook(InputAction.CallbackContext context)
		{
			if(cursorInputForLook)
			{
				LookInput(context.ReadValue<Vector2>());
			}
		}

		public void OnJump(InputAction.CallbackContext context)
		{
			JumpInput(context.performed);
		}

		public void OnSprint(InputAction.CallbackContext context)
		{
			SprintInput(context.performed);
		}

		public void OnLClick(InputAction.CallbackContext context)
		{
			LClickInput(context.performed);
		}

        public void OnRClick(InputAction.CallbackContext context)
        {
            RClickInput(context.performed);
        }

		public void OnWheel(InputAction.CallbackContext context)
		{
			float ctx = context.ReadValue<float>();
			if (ctx != 0)
			{
				WheelInput(ctx > 0);

			}

		}

		public void OnTab(InputAction.CallbackContext context)
		{
			TabInput(!context.canceled);
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
		
		public void LClickInput(bool newLClickState)
		{
			if (newLClickState)
			{
				gameManager.Player.LClickInput();
			}
		}

        public void RClickInput(bool newLClickState)
        {
            if (newLClickState)
            {
				gameManager.Player.RClickInput();
            }
        }

		/// <summary>
		/// true면 위로 휠 이동 false면 아래로 휠 이동
		/// </summary>
		/// <param name="newWheelState"></param>
		public void WheelInput(bool newWheelState)
		{
			gameManager.Player.PolaroidTransform.SetPolaroidIndex(newWheelState);
		}

		public void TabInput(bool newTabState)
		{
			gameManager.ViewPolaroid = newTabState;
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