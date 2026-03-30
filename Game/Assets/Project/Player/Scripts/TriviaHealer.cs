using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TriviaHealer : MonoBehaviour
{
    [Header("UI")]
    public GameObject triviaPanel;
    public TMP_Text questionText;
    public Button[] answerButtons;

    [Header("Question")]
    public string question = "What planet is known as the Red Planet?";
    public string[] answers = { "Mars", "Venus", "Jupiter" };
    public int correctIndex = 0;

    [Header("Healing")]
    public int healAmount = 10;
    public PlayerHealth playerHealth;

    private bool used = false;

    private void OnTriggerEnter(Collider other)
    {
        if (used || !other.CompareTag("Player")) return;

        used = true;
        ShowTrivia();
    }

    void ShowTrivia()
    {
        triviaPanel.SetActive(true);
        questionText.text = question;

        for (int i = 0; i < answerButtons.Length; i++)
        {
            int index = i;
            answerButtons[i].GetComponentInChildren<TMP_Text>().text = answers[i];
            answerButtons[i].onClick.RemoveAllListeners();
            answerButtons[i].onClick.AddListener(() => OnAnswerSelected(index));
        }

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void OnAnswerSelected(int index)
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        triviaPanel.SetActive(false);

        if (index == correctIndex)
        {
            Debug.Log("Correct! Healing player.");
            if (playerHealth != null)
                playerHealth.Heal(healAmount);
        }
        else
        {
            Debug.Log("Wrong answer. No healing.");
        }
    }
}