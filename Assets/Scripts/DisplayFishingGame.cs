using UnityEngine;
using UnityEngine.UI;
using TMPro; // TextMeshProUGUI for better text rendering

public class DisplayFishingGame : MonoBehaviour
{
    public Camera mainCamera;
    public LineRenderer lineRenderer;
    public FishingGame Game;
    public GameObject Fish;
    public GameObject fishingArea;
    public GameObject FishingRod;
    public GameObject Bobber;
    public GameObject WaterRipple;
    public GameObject SplashEffect;
    public GameObject Background;
    public GameObject pullBar;
    public GameObject catchBar;
    public GameObject TimerText;
    public GameObject ResultText;
    public GameObject BiteIndicator;
    public GameObject EscapeIndicator;

    private bool drawfishingLineBobber;
    private bool drawfishingLineFish;

    float scaleX;
    float scaleY;
    void Start()
    {
        scaleX = fishingArea.transform.localScale.x;
        scaleY = fishingArea.transform.localScale.y;
        lineRenderer = GetComponent<LineRenderer>();
    }

    public void DrawfishingLineBobber()
    {
        // Draw line from rod to bobber
        drawfishingLineBobber = true;
    }
    public void DrawfishingLineFish()
    {
        // Draw line from rod to bobber
        drawfishingLineFish = true;
    }

    public void UnDrawFishinngLine()
    {
        drawfishingLineBobber = false;
        drawfishingLineFish = false;
        lineRenderer.SetPosition(0, new Vector3(0,0,0));
        lineRenderer.SetPosition(1, new Vector3(0,0,0));  
    }

    void Update()
    {
        if(drawfishingLineBobber)
        {
            Vector3 rodWorldPosition = mainCamera.ScreenToWorldPoint(FishingRod.transform.position);
            Vector3 bobberWorldPosition = mainCamera.ScreenToWorldPoint(Bobber.transform.position);
            rodWorldPosition.z = 0f;
            bobberWorldPosition.z = 0f;
            lineRenderer.SetPosition(0, rodWorldPosition);
            lineRenderer.SetPosition(1, bobberWorldPosition);
        }
        if(drawfishingLineFish)
        {
            Vector3 rodWorldPosition = mainCamera.ScreenToWorldPoint(FishingRod.transform.position);
            rodWorldPosition.z = 0f;
            lineRenderer.SetPosition(0, rodWorldPosition);
            lineRenderer.SetPosition(1, Fish.transform.position);
        }
    }

    public void ShowBobber(bool show)
    {
        Bobber.SetActive(show);
    }

    public void BobberDip()
    {
        // Show bobber dipping animation
        Bobber.GetComponent<Animator>().SetTrigger("Dip");
        // invoke fish bited after ranndomly time
    }

    public void FishingRodFish()
    {
        FishingRod.GetComponent<Animator>().SetTrigger("Fishing");
    }

    public void FishingRodUnFish()
    {
        FishingRod.GetComponent<Animator>().SetTrigger("UnFish");
    }

    public void FishBited()
    {
        // Show bobber bited animation
        Bobber.GetComponent<Animator>().SetTrigger("Bited");
        // Show bite indicator
    }

    public void UpdateFishAnimation(string state)
    {
        // Update fish animation based on state (e.g., swimming, struggling, escaping)
        Fish.GetComponent<Animator>().SetTrigger(state);
    }

    public void UpdateFish()
    {
        Vector2 position = Game.fishPosition;
        // Update fish position on screen based on game logic
        Vector2 scaled_position = new Vector2(position.x * scaleX/ (Game.escapeRadius * 2) , position.y * scaleY/(Game.escapeRadius * 2));
        Fish.transform.localPosition = scaled_position;
        Fish.transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * ((Game.fishDirection.x>0)? 1:-1), transform.localScale.y, transform.localScale.z);
    }

    public void ShowPullBar(bool show)
    {
        // Show or hide pull bar UI
        pullBar.SetActive(show);
    }

    public void UpdatePullBar()
    {
        float pullStrength = Game.pullStrength;
        float maxPullStrength = Game.maxPullStrength;
        // Update pull bar UI based on pull strength
        pullBar.transform.localScale = new Vector3(1f, pullStrength/maxPullStrength, 1f);
    }

    public void UpdateCatchBar()
    {
        float catchProgress = (Game.catching)? (Game.GetComponent<FishingGame>().timer - Game.GetComponent<FishingGame>().catchStartTime) / Game.GetComponent<FishingGame>().catchTime : 1;
        // Update catch bar UI based on catch progress
        catchBar.transform.localScale = new Vector3(1 - catchProgress,1 - catchProgress, 1f);
    }   

    public void CreateRipple(Vector2 position)
    {
        // Instantiate ripple effect at the given position
        Vector2 scaled_position = new Vector2(position.x * scaleX, position.y * scaleY);
        GameObject ripple = Instantiate(WaterRipple, scaled_position, Quaternion.identity);
        // Set ripple animation and destroy after animation completes
        Destroy(ripple, 1f); // Assuming ripple animation lasts 1 second
    }

    public void CreateSplash(Vector2 position)
    {
        // Instantiate splash effect at the given position
        Vector2 scaled_position = new Vector2(position.x * scaleX, position.y * scaleY);
        GameObject splash = Instantiate(SplashEffect, scaled_position, Quaternion.identity);
        // Set splash animation and destroy after animation completes
        Destroy(splash, 1f); // Assuming splash animation lasts 1 second
    }

    public void changeBackgroundColor(Color color)
    {
        // Change background color based on game state
        Background.GetComponent<SpriteRenderer>().color = color;
    }

    public void ShowTimer(bool show)
    {
        TimerText.SetActive(show);
    }

    public void UpdateTimer()
    {
        // Update timer text on screen
        float time = Game.GetComponent<FishingGame>().timer;
        TimerText.GetComponent<TextMeshProUGUI>().text = "Time: " + time.ToString("F2") + "s";
    }

    public void ShowResult(string result, Color color, bool show)
    {
        // Display result text (e.g., "You Win!", "You Lose!", "Missed!")
        ResultText.GetComponent<TextMeshProUGUI>().text = result;
        ResultText.GetComponent<TextMeshProUGUI>().color = color;
        ResultText.SetActive(show);
    }

    public void ShowEscapeIndicator(bool show)
    {
        // Go near the fish position and show escape indicator
        Vector2 position = Game.GetComponent<FishingGame>().fishPosition;
        EscapeIndicator.transform.localPosition = position;
        EscapeIndicator.SetActive(show);
    }

    public void ShowBitInndicator(bool show)
    {
        BiteIndicator.SetActive(show);
    }
/// <summary>
/// The following methods are called by the FishingGame script to trigger different animations and UI updates based on game state changes. They handle the visual feedback for the player, such as showing the bobber dipping, fish biting, updating the tug of war bars, and displaying results when the player wins or loses. Each method corresponds to a specific event in the game and ensures that the UI reflects the current state of the fishing game accurately.
/// </summary>
    public void ResetUI()
    {
        // Reset all UI elements to default state for new game
        ShowPullBar(false);
        pullBar.transform.localScale = new Vector3(0f, 1f, 1f);
        catchBar.transform.localScale = new Vector3(0f, 1f, 1f);
        ResultText.SetActive(false);
        ShowEscapeIndicator(false);
        ShowBitInndicator(false);
        ShowBobber(false);
        ShowTimer(false);
    }

    public void OnReadyAnimation()
    {
        // Set animation trigger OnReady
        // Reset variables and prepare for new game
        Debug.Log("Ready animation triggered");
        ResetUI();
    }

    public void OnDipAnimation()
    {
        // Set animation trigger OnDip
        Debug.Log("Bobber dip animation triggered");
        BobberDip();
        ShowBobber(true);
        DrawfishingLineBobber();
        FishingRodFish();
    }

    public void OnBitedAnimation()
    {
        // Set animation trigger OnBited
        Debug.Log("Fish bited! Show bited animation");
        FishBited();
        ShowBitInndicator(true);
        ShowBobber(false);
    }

    public void OnPullAnimation()
    {
        // Set animation trigger OnPull
        Debug.Log("Pull animation triggered");
        ShowBitInndicator(false);
        UnDrawFishinngLine();
        return;
    }

    public void OnHookedAnimation()
    {
        ShowPullBar(true);
        DrawfishingLineFish();
        ShowTimer(true);
    }

    public void UpdateTugOfWarAnimation()
    {
        Debug.Log("Updating tug of war animation");
        UpdateFish();
        UpdatePullBar();
        UpdateCatchBar();
        UpdateTimer();
    }
    public void OnMissedPullAnimation()
    {
        // Show missed pull result and end game
        Debug.Log("Play missed pull animation");
        ShowResult("Missed!", Color.red, true);
        ShowBobber(false);
        FishingRodUnFish();
        EndGame();
    }

     public void OnLoseTugOfWarAnimation()
    {
        // Show lose result and end game
        Debug.Log("Play lose animation");
        ShowResult("You Lose!", Color.red, true);
        EndGame();
    }

    public void OnWinTugOfWarAnimation()
    {
        // Show win result and end game
        Debug.Log("Play win animation");
        ShowResult("You Win!", Color.green, true);
        EndGame();
    }

    public void EndGame()
    {
        Debug.Log("Show end game UI and reset game state");
        ShowPullBar(false);
        ShowEscapeIndicator(false);
    }


}
