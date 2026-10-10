using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public DeckManager deckManager;
    public Player player;

    public bool isPlayerTurn = true;

    public void EndPlayerTurn(){
        if (!isPlayerTurn){
            return;
        }

        isPlayerTurn = false;

        deckManager.EndTurn();

        Debug.Log("Enemy turn begins.");

        RunEnemyTurn();
    }

    public void StartPlayerTurn(){
        isPlayerTurn = true;

        player.ResetEnergy();
        player.currentBlock = 0;

        deckManager.StartPlayerTurn();
        Debug.Log("Player turn begins.");
    }

    public void RunEnemyTurn(){
        Enemy[] enemies = FindObjectsByType<Enemy>();

        foreach (Enemy enemy in enemies){
            if (enemy.gameObject.activeInHierarchy){
                enemy.TakeTurn(player);
            }
        }

        StartPlayerTurn();
    }

    public bool enemiesWiped(){
        Enemy[] enemies = FindObjectsByType<Enemy>();

        foreach (Enemy enemy in enemies){
            if (enemy.gameObject.activeInHierarchy){
                return false;
            }
        }

        return true;
    }
}
