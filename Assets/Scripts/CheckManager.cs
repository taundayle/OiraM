using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckManager : MonoBehaviour
{
    public GameSession gameSession;
    public PlayerUILevelUpManager playerUILevelUpManager;
    private void Start()
    {
        gameSession = FindObjectOfType<GameSession>();
        playerUILevelUpManager = FindObjectOfType<PlayerUILevelUpManager>();
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Y))
        {
            playerUILevelUpManager.GainExp(50);
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            gameSession.Health -= 5;
            Debug.Log($"Giam {gameSession.Health}");
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            gameSession.Stamina -= 5;
            Debug.Log($"Giam {gameSession.Stamina}");
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            gameSession.Mana -= 5;
            Debug.Log($"Giam  {gameSession.Mana}");
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            gameSession.Health += 5;
            Debug.Log($"Cong {gameSession.Health}");
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            gameSession.Stamina += 5;
            Debug.Log($"Cong {gameSession.Stamina}");
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            gameSession.Mana += 5;
            Debug.Log($"Cong {gameSession.Mana}");
        }
    }
}
