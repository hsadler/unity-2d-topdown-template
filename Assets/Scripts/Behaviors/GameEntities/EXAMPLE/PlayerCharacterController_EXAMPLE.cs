using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacterController_EXAMPLE : MonoBehaviour
{


    // debug settings
    private readonly bool useLogging = false;


    void Start() { }

    void Update()
    {
        if (PlaySceneManager.instance.playerInputManager.inputMode == GameSettings.INPUT_MODE_PLAYER_CHARACTER_CONTROL)
        {
            if (this.TryGetComponent<Movable>(out Movable movable))
            {
                Vector3 direction = Vector3.zero;
                if (Input.GetKeyDown(GameSettings.PLAYER_CHARACTER_CONTROL_MOVE_UP_KEY))
                {
                    direction = this.transform.up;
                }
                if (Input.GetKeyDown(GameSettings.PLAYER_CHARACTER_CONTROL_MOVE_DOWN_KEY))
                {
                    direction = -this.transform.up;
                }
                if (Input.GetKeyDown(GameSettings.PLAYER_CHARACTER_CONTROL_MOVE_LEFT_KEY))
                {
                    direction = -this.transform.right;
                }
                if (Input.GetKeyDown(GameSettings.PLAYER_CHARACTER_CONTROL_MOVE_RIGHT_KEY))
                {
                    direction = this.transform.right;
                }
                if (direction != Vector3.zero)
                {
                    if (this.useLogging)
                    {
                        Debug.Log("PlayerCharacterController_EXAMPLE is moving Movable: " + movable.gameObject.GetInstanceID().ToString() + " in direction: " + direction.ToString());
                    }
                    var animationDuration = GameSettings.DEFAULT_TICK_DURATION / 2;
                    movable.AddMovementForce(direction, 1);
                    movable.CommitMovement(animationDuration);
                    movable.ClearMovementForces();
                }
            }
        }
    }


}
