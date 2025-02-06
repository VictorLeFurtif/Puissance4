using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private TMP_Text showTurnText;
    public static GameManager instance;
    public GameTurn currentPlayer = GameTurn.Player;
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        UpdateTurnText();
    }
    
    public enum GameTurn
    {
        Player,
        Ia,
        Wait,
        Finished,
    }

    public void ReloadActualScene()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }

    public void UpdateTurnText()
    {
        showTurnText.text = "Current Player : "+ currentPlayer.ToString();
    }
    
}
