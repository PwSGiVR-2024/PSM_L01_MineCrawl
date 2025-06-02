using UnityEngine;

public class SceneRestoreManager : MonoBehaviour
{
    void Start()
    {
        if (BattleTransferData.cameFromBattle)
        {

            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                player.transform.position = BattleTransferData.playerPosition;

            // Usuñ pokonanego przeciwnika
            if (!string.IsNullOrEmpty(BattleTransferData.defeatedEnemyID))
            {
                GameObject defeatedEnemy = GameObject.Find(BattleTransferData.defeatedEnemyID);
                if (defeatedEnemy != null)
                    Destroy(defeatedEnemy);

                BattleTransferData.defeatedEnemyID = null; 
            }


            BattleTransferData.cameFromBattle = false;
        }
    }
}
