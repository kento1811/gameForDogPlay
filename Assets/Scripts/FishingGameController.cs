using UnityEngine;
using UnityEngine.InputSystem; // For handling input actions

public class FishingGameController : MonoBehaviour
{
    public FishingGame Game;
    public DisplayFishingGame Display;
    Vector2 mousePosition;

    public void OnMouseMove(InputAction.CallbackContext context)
    {
        mousePosition = context.ReadValue<Vector2>();
        Vector2 UImiddle = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Vector2 mouseRelativePosition = mousePosition - UImiddle;
        Display.FishingRod.transform.localPosition = mouseRelativePosition;
        if(Game.tugOfWar)
        {
            // Handle mouse movement for tug of war mechanics
            Game.pullDirection = (mouseRelativePosition).normalized;
            Display.pullBar.transform.localPosition = mouseRelativePosition;
            Game.pullStrength = Display.mainCamera.ScreenToWorldPoint(mouseRelativePosition).magnitude * 0.2f;
        }

    }

private bool preventMouseRelease = false; // Flag to prevent multiple triggers on mouse release

    public void OnMouseClick(InputAction.CallbackContext context)
    {
        if(!preventMouseRelease)
        {
            preventMouseRelease = true;
            if(!Game.isGameActive)
            {
                if(Game.gameState == 1)
                {
                    Game.gameState = 3;
                    Game.TriggerGameState(Game.gameState);
                } else
                {
                    Game.TriggerGameState(++Game.gameState); // Move to the next game state on click
                }
            }
            Debug.Log("Mouse click detected! Current game state: " + Game.gameState);
        }
        else
        {
            preventMouseRelease = false; // Reset flag on mouse release
        }

    }

    public void OnScroll(InputAction.CallbackContext context)
    { 
        Vector2 scrollValue = context.ReadValue<Vector2>();
    }

    public void OnRightClick(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            // You can add logic here to interact with the fishing game based on the right click
            Debug.Log("Right click detected!");
        }
    }


}
