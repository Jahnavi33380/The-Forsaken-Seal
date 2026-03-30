using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    
    public GameObject deathPanel;
    public TMP_Text countdownText;
    public Transform player;
    public Slider healthSlider;

    public float countdownDuration = 5f;


    public void ShowDeathScreen()
    {
        Debug.Log("Death screen is shown");
        deathPanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
         
        StartCoroutine(CountdownAndRestart());
    }

    private IEnumerator CountdownAndRestart()
    {
        Time.timeScale = 0f;

        float countdown = countdownDuration;
        while (countdown > 0)
        {
            countdownText.text = $"Restarting in {Mathf.CeilToInt(countdown)}...";
            yield return new WaitForSecondsRealtime(1f);
            countdown--;
        }

        RestartGame();
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}