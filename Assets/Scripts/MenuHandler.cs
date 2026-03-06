using UnityEngine;
using UnityEngine.SceneManagement; 
using TMPro; // Added for TextMeshPro

public class MenuHandler : MonoBehaviour
{
    public TextMeshProUGUI finalScoreText; // Assign this in the Game Over scene Inspector

    void Start()
    {
        // Check if we are in the Game Over scene and have a text component to update
        if (finalScoreText != null)
        {
            finalScoreText.text = "Final Score: " + PlayerController.finalOverallScore;
        }
    }

    // This function will be called when we click the Start button
    public void PlayGame()
    {
        SceneManager.LoadScene("SampleScene");
    }

    // This function will be called from the "Main Menu" button on your Game Over screen
    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}