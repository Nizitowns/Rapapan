using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using JetBrains.Annotations;

/// <summary>
/// Manages the combo system and fever Mode.
/// </summary>
public class ComboHandler : MonoBehaviour
{
    #region Singleton

    // Singleton instance
    public static ComboHandler Instance { get; private set; }

    /// <summary>
    /// Called when the script instance is being loaded.
    /// Ensures there is only one instance of RhythmSystem.
    /// </summary>
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keep this object across scene loads
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #endregion

    #region Combo Tracking

    public int comboCount = 0;
    public int maxComboCount = 0;
    public int currentNoteStreak = 0;
    public int highestNoteStreak = 0;
    public int playerScore = 0;

    public int scorePerNote = 1;
    public float comboTimer = 0f;

    public bool inFeverMode = false;

    private int feverModeThreshold = 4; // Number of consecutive hits to enter Fever Mode

    #endregion

    #region Multipliers

    public int feverModeScoreMultiplier = 2;

    #endregion

    #region Visual and Audio Feedback

    //in the combo test scene I made, these objects act as visual placeholders
    public GameObject feverModeEffect, comboBreakEffect;
    
    public AudioSource feverModeSound; // Sound effect for Fever Mode activation

    #endregion

    /// <summary>
    /// Update combo timer.
    /// </summary>
    private void Update()
    {
        if (currentNoteStreak > 0)
        {
            comboTimer += Time.deltaTime;
            if (comboTimer >= 5f) // Reset combo after 5 seconds of inactivity
            {
                ResetCombo();
            }
        }
    }

    /// <summary>
    /// Manages the combo and max combo count.
    /// Checks if Fever Mode is reached
    /// </summary>
    public void RegisterHit()
    {
        Debug.Log("Note Hit");
        currentNoteStreak++;

        comboTimer = 0f;

        if (currentNoteStreak > highestNoteStreak)
        {
            highestNoteStreak = currentNoteStreak;
        }

        if (inFeverMode)
        {
            Debug.Log("Apply multiplier");
            ScoreMultiplierHandler.Instance.ApplyScoreMultiplier(feverModeScoreMultiplier, scorePerNote);
        }

        else
        {
            Debug.Log("Increment normal");
            playerScore = playerScore + scorePerNote;
        }
        
        //if the player correctly inputs a 4 note combination
        //increment the combo counter
        if(currentNoteStreak % 4 == 0)
        {
            comboCount++;
        }

        // Check if combo count threshold for Fever Mode is reached
        if (comboCount >= feverModeThreshold && !inFeverMode)
        {
            EnterFeverMode();
        }

        //Update highest max combo
        if (comboCount > maxComboCount)
        {
            maxComboCount = comboCount;
        }

    }

    /// <summary>
    /// Handles Fever Mode state.
    /// </summary>
    private void EnterFeverMode()
    {
        // Activate Fever Mode
        inFeverMode = true;

        Debug.Log("Entered Fever Mode");

        // Play visual and audio effects
        if (feverModeEffect != null)
        {
            //spawn the feverModeEffect prefab clone at the location of the prefab
            Instantiate(feverModeEffect, feverModeEffect.transform.position, feverModeEffect.transform.rotation);
        }
        if (feverModeSound != null)
        {
            Debug.Log("Playing sound effect");
            feverModeSound.Play();
        }
    }

    /// <summary>
    /// Handles combo reset.
    /// </summary>
    public void ResetCombo()
    {
        Debug.Log("Combo Broken");
        // Reset combo variables
        comboCount = 0;
        currentNoteStreak = 0;
        comboTimer = 0f;

        // Deactivate Fever Mode
        if (inFeverMode)
        {
            inFeverMode = false;

            // Reset score multiplier
            ScoreMultiplierHandler.Instance.ResetScoreMultiplier();
        }

        //spawn the feverModeEffect prefab clone at the location of the prefab
        Instantiate(comboBreakEffect, comboBreakEffect.transform.position, comboBreakEffect.transform.rotation);

    }

    /// <summary>
    /// returns current combo count.
    /// </summary>
    public int GetCurrentComboCount()
    {
        return comboCount;
    }

    /// <summary>
    /// sets the current combo count.
    /// </summary>
    public void SetCurrentComboCount(int newComboCount)
    {
        comboCount = newComboCount;
    }

    /// <summary>
    /// returns current player score.
    /// </summary>
    public int GetCurrentPlayerScore()
    {
        return playerScore;
    }

    /// <summary>
    /// sets the current player score.
    ///used by multiplier script.
    /// </summary>
    public void SetCurrentPlayerScore(int newPlayerScore)
    {
        playerScore = newPlayerScore;
    }
}