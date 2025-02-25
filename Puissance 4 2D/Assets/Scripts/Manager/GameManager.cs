using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private TMP_Text showTurnText;
    public static GameManager instance;
    public GameTurn currentPlayer = GameTurn.You;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject secondPlayer;
    [SerializeField] private GameObject iaRandom;
    [SerializeField] private GameObject iaCool;
    
    
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
        You,
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

    public void GameWithTWoPlayer()
    {
        Instantiate(player);
        Instantiate(secondPlayer);
    }

    public void GameWithIaRandom()
    {
        Instantiate(player);
        Instantiate(iaRandom);
    }
    
    public void GameWithIaCool()
    {
        Instantiate(player);
        Instantiate(iaCool);
    }

    public void UndoRedo(bool undo)
    {
        switch (undo)
        {
            case true :
                if (MapManager.instance.undoList.Count == 0)
                {
                    break;
                }
                var mapWanted = MapManager.instance.undoList.Pop();
                MapManager.instance.redoList.Push(MapManager.instance.mapArray);
                MapManager.instance.mapArray = mapWanted;
                break;

            case false : 
                Debug.Log("I m doing it");
                if (MapManager.instance.redoList.Count == 0)
                {
                    Debug.Log("I m empty idiot");
                    break;
                }
                mapWanted = MapManager.instance.redoList.Pop();
                MapManager.instance.undoList.Push(MapManager.instance.mapArray);
                MapManager.instance.mapArray = mapWanted;
                break;
        }
        MapManager.instance.RefreshMap();
    }
}
