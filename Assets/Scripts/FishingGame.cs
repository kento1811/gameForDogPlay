using UnityEngine;

public class FishingGame : MonoBehaviour
{
    public DisplayFishingGame Display;
    public int gameState = 0; // 0 = ready, 1 = dip, 2 = bited, 3 = pull, 4 = tug of war, 5 = win, 6 = lose, 7 = missed pull
    public int maxGameState = 7;
    public bool isGameActive = false;
    public int score = 0;
    public Vector2 fishPosition;
    public Vector2 fishVelocity;
    public Vector2 fishDirection;
    public Vector2 pullDirection;
    public float timer = 0f;
    public float catchStartTime = 0f;
    public bool catching;
    public bool dip;
    public bool bited;
    public bool pull;
    public bool missPull;
    public bool tugOfWar;
    public bool escape;
    public bool win;
    public bool lose;

    public float reactionTime = 1f;
    public float fishMaxSpeed = 2f;
    public float fishStrength = 1f;
    public float maxFishStrength = 3f;
    public float catchTime = 3f;
    public float catchRadius = 0.5f;
    public float escapeRadius = 5f;
    public float tugOfWarTimeLimit = 30f;
    public float pullStrength = 1.5f;
    public float maxPullStrength = 3f;
    public float pullStrengthDecreasePercent = 10f;
    public float pullStrengthDecreaseLinnear = 1f;


    void Start()
    {
        OnReady();
    }

    public void FishAlready()
    {
        OnReady();
        OnDip();
    }

    public void OnReady()
    {
        // Set Display OnReady
        Debug.Log("Game is ready! Click to start.");
        gameState = 0;
        // Reset variables and prepare for new game
        fishPosition = new Vector2(0, 0);
        fishVelocity = Vector2.zero;
        fishDirection = Random.insideUnitCircle.normalized; // Random initial direction
        pullDirection = Vector2.zero;
        pullStrength = 2f;
        timer = 0f;
        bited = false;
        pull = false;
        dip = false;
        catching = false;
        missPull = false;
        tugOfWar = false;
        escape = false;
        win = false;
        lose = false;
        Display.OnReadyAnimation();
    }

    public void OnDip()
    {
        Debug.Log("Bobber dipped!");
        gameState = 1;
        dip = true;
        Invoke(nameof(OnBited), Random.Range(5f, 10f)); // Simulate fish bited after random time
        Display.OnDipAnimation();
    }

    public void OnBited()
    {
        if (!pull)
        {
            Debug.Log("Fish bited!");
            bited = true;
            gameState = 2;
            Invoke(nameof(BiteCountdown), reactionTime); // Start counting time for reaction
            Display.OnBitedAnimation();
        }
    }

    public void OnPull()
    {
        Debug.Log("Pull action triggered!");
        Display.OnPullAnimation();
        pull = true;
        dip = false;
        gameState = 3;
        bool success = !missPull && bited; // Determine if pull was successful based on reaction time
        // Check if fish bited and react on timee
        if (success)
        {
            // Hooked
            OnHooked();
        } else
        {
            // Missed pull, lose the game
            Invoke(nameof(OnMissedPull), 0f);
        }
    }

    public void OnHooked()
    {
        Debug.Log("Hooked");
        Display.OnHookedAnimation();
        Invoke(nameof(OnTugOfWar), 0f);
        Invoke(nameof(FishTryGetAway), 0f); // Start fish trying to escape
    }

    void OnTugOfWar()
    {
        Debug.Log("Fish hooked! Tug of war started!");
        // Main game tug of war
        // Keep fish inside the catch zone for amount of time --> win
        // fish get to escape zone --> lose
        // after tug of war for time limit --> win
        tugOfWar = true;
        gameState = 4;
        timer = 0f; // Reset timer for tug of war
        catchStartTime = 0f; // Reset catch timer
        Invoke(nameof(OnTugOfWarTimeLimitReached), tugOfWarTimeLimit); // Set time limit for tug of war
    }

    void OnTugOfWarTimeLimitReached()
    {
        if (tugOfWar) // Check if tug of war is still active
        {
            Debug.Log("Time limit reached! You win the tug of war!");
            OnWinTugOfWar();
            tugOfWar = false;
        }
        return;
    }

    void Update()
    {
        if (tugOfWar)
        {
            // Update fish position based on its velocity
            UpdateFishPosition();
            Debug.Log("Fish position: " + fishPosition + " Fish strength: " + fishStrength + " Fish direction: " + fishDirection + " Fish velocity: " + fishVelocity + " Pull strength: " + pullStrength + " Pull direction: " + pullDirection);
            // Check if fish is in catch radius
            if (Vector2.Distance(fishPosition, Vector2.zero) <= catchRadius)
            {
                if (!catching)
                {
                    catchStartTime = timer; // Start catch timer
                }       
                catching = true;
                // Check if fish has been in catch radius for required time
                if (timer - catchStartTime >= catchTime)
                {
                    // Fish caught, win the game
                    Invoke(nameof(OnWinTugOfWar), 0f);
                    tugOfWar = false;
                    return;
                }
                Debug.Log("Fish is in catch radius! Keep it there for " + (catchTime - (timer - catchStartTime)).ToString("F2") + " seconds to catch!");   
            } else
            {
                catching = false;
                Debug.Log("Fish is outside catch radius! Keep it inside for " + catchTime.ToString("F2") + " seconds to catch!");
            }

            // Check if fish is in escape radius
            if (Vector2.Distance(fishPosition, Vector2.zero) >= escapeRadius)
            {
                // Fish escaped, lose the game
                escape = true;
                Invoke(nameof(OnLoseTugOfWar), 0f);
                tugOfWar = false;
                return;
            }
            timer += Time.deltaTime;
            if(pullStrength > maxPullStrength) pullStrength = maxPullStrength;
            pullStrength -=  (pullStrengthDecreasePercent*pullStrength + pullStrengthDecreaseLinnear) * Time.deltaTime; // Decrease pull strength over time

            // Fish gets stronger the longer the tug of war goes on
            fishStrength += 0.01f * Time.deltaTime; 
            if (fishStrength > 3f)
            {
                fishStrength = 3f; // Cap fish strength to prevent it from becoming impossible
            }
            Display.UpdateTugOfWarAnimation();
        }
    }

    void UpdateFishPosition()
    {
        // Update fish position based on its velocity
        fishVelocity += fishDirection * fishStrength + pullDirection * pullStrength;
        if (fishVelocity.magnitude > fishMaxSpeed)
        {
            fishVelocity = fishVelocity.normalized * fishMaxSpeed;
        }   
        fishPosition += fishVelocity * Time.deltaTime;
    }

    void OnMissedPull()
    {
        // Set animation trigger OnMissedPull
        // Lose the game
        gameState = 7;
        missPull = true;
        Debug.Log("You missed the pull! You lose!");
        Display.OnMissedPullAnimation();    
    }

    void OnLoseTugOfWar()
    {
        Debug.Log("Fish escaped! You lost the tug of war!");
        gameState = 6;
        lose = true;
        Debug.Log("You lost the tug of war!");
        Display.OnLoseTugOfWarAnimation();
        BroadcastMessage("YouGotTheFish");
    }

    void OnWinTugOfWar()
    {
        Debug.Log("Congratulations! You won the tug of war!");
        gameState = 5;
        score += 1;
        win = true;
        Debug.Log("You won the tug of war!");
        Display.OnWinTugOfWarAnimation();
        BroadcastMessage("YouGotTheFish");
    }

    void EndGame()
    {
        Debug.Log("Game ended! Your score: " + score);
        // Reset game state and variables
        gameState = 0;
        fishPosition = Vector2.zero;
        fishVelocity = Vector2.zero;
        pullDirection = Vector2.zero;
        pullStrength = 1.5f;
        timer = 0f;
        bited = false;
        pull = false;
        dip = false;
        catching = false;
        missPull = false;
        tugOfWar = false;
        escape = false;
        win = false;
        lose = false;
        Display.EndGame();
    }

    public void TriggerGameState(int state)
    {
        switch (state)
        {
            case 0:
                OnReady();
                break;
            case 1:
                OnDip();
                break;
            case 2:
                OnBited();
                break;
            case 3:
                OnPull();
                break;
            default:
                Debug.Log("Invalid game state");
                break;
        }
        Debug.Log("Game state triggered: " + state);
    }

    void BiteCountdown()
    {
        timer = 0f;
        if (!pull)
        {
            Invoke(nameof(OnMissedPull), 0f); // If reaction time exceeded without pulling, trigger missed pull
        }
    }

    void OnDrawGizmos()
    {
        // Draw fish position and catch/escape radius for debugging
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(fishPosition, 0.1f);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(Vector2.zero, catchRadius);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(Vector2.zero, escapeRadius);
    }

    void FishTryGetAway()
    {
        fishDirection = Random.insideUnitCircle.normalized;
        if (tugOfWar)
        {
            Invoke(nameof(FishTryGetAway), Random.Range(0.5f, 2f)); // Fish tries to escape at random intervals
        }
    }
}
