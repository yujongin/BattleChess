using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveButton : MonoBehaviour
{
    public Button N;
    public Button E;
    public Button S;
    public Button W;

    public int x;
    public int z;
    BattleController BC;
    Player1Controller PC1;
    Player2Controller PC2;
    void Start()
    {
        BC = GameObject.Find("BattleManager").GetComponent<BattleController>();
        
    }

    public void CreateMovePanel()
    {
        BC.MovePanel.SetActive(true);
        BC.ActionPanel.SetActive(false);
    }

    public void MoveN()
    {
        if (BC.battleState == BattleController.State.Player1Turn)
        {
            PC1 = GameObject.Find(BC.playerList[0].name).GetComponent<Player1Controller>();
            x = (int)BC.playerList[0].transform.position.x;
            z = (int)BC.playerList[0].transform.position.z;

            if (PC1.Mobil > 0 && BC.Board[x + 1, z + 2] == 0)
            {
                BC.playerList[0].transform.position += new Vector3(0f, 0f, 1.0f);
                PC1.Mobil--;
                PC1.UpdateStatus();
                BC.UpdateBoard();
            }
        }
        else if(BC.battleState == BattleController.State.Player2Turn)
        {
            PC2 = GameObject.Find(BC.playerList[0].name).GetComponent<Player2Controller>();
            x = (int)BC.playerList[0].transform.position.x;
            z = (int)BC.playerList[0].transform.position.z;

            if (PC2.Mobil > 0 && BC.Board[x + 1, z + 2] == 0)
            {
                BC.playerList[0].transform.position += new Vector3(0f, 0f, 1.0f);
                PC2.Mobil--;
                PC2.UpdateStatus();
                BC.UpdateBoard();
            }
        }
    }


    public void MoveE()
    {
        if (BC.battleState == BattleController.State.Player1Turn)
        {
            PC1 = GameObject.Find(BC.playerList[0].name).GetComponent<Player1Controller>();
            x = (int)BC.playerList[0].transform.position.x;
            z = (int)BC.playerList[0].transform.position.z;

            if (PC1.Mobil > 0 && BC.Board[x + 2, z + 1] == 0)
            {
                BC.playerList[0].transform.position += new Vector3(1.0f, 0f, 0f);
                PC1.Mobil--;
                PC1.UpdateStatus();
                BC.UpdateBoard();
            }
        }
        else if (BC.battleState == BattleController.State.Player2Turn)
        {
            PC2 = GameObject.Find(BC.playerList[0].name).GetComponent<Player2Controller>();
            x = (int)BC.playerList[0].transform.position.x;
            z = (int)BC.playerList[0].transform.position.z;

            if (PC2.Mobil > 0 && BC.Board[x + 2, z + 1] == 0)
            {
                BC.playerList[0].transform.position += new Vector3(1.0f, 0f, 0f);
                PC2.Mobil--;
                PC2.UpdateStatus();
                BC.UpdateBoard();
            }
        }
    }


    public void MoveS()
    {
        if (BC.battleState == BattleController.State.Player1Turn)
        {
            PC1 = GameObject.Find(BC.playerList[0].name).GetComponent<Player1Controller>();
            x = (int)BC.playerList[0].transform.position.x;
            z = (int)BC.playerList[0].transform.position.z;

            if (PC1.Mobil > 0 && BC.Board[x + 1, z] == 0)
            {
                BC.playerList[0].transform.position += new Vector3(0f, 0f, -1f);
                PC1.Mobil--;
                PC1.UpdateStatus();
                BC.UpdateBoard();
            }
        }
        else if (BC.battleState == BattleController.State.Player2Turn)
        {
            PC2 = GameObject.Find(BC.playerList[0].name).GetComponent<Player2Controller>();
            x = (int)BC.playerList[0].transform.position.x;
            z = (int)BC.playerList[0].transform.position.z;

            if (PC2.Mobil > 0 && BC.Board[x + 1, z] == 0)
            {
                BC.playerList[0].transform.position += new Vector3(0f, 0f, -1f);
                PC2.Mobil--;
                PC2.UpdateStatus();
                BC.UpdateBoard();
            }
        }
    }


    public void MoveW()
    {
        if (BC.battleState == BattleController.State.Player1Turn)
        {
            PC1 = GameObject.Find(BC.playerList[0].name).GetComponent<Player1Controller>();
            x = (int)BC.playerList[0].transform.position.x;
            z = (int)BC.playerList[0].transform.position.z;

            if (PC1.Mobil > 0 && BC.Board[x, z + 1] == 0)
            {
                BC.playerList[0].transform.position += new Vector3(-1f, 0f, 0f);
                PC1.Mobil--;
                PC1.UpdateStatus();
                BC.UpdateBoard();
            }
        }
        else if (BC.battleState == BattleController.State.Player2Turn)
        {
            PC2 = GameObject.Find(BC.playerList[0].name).GetComponent<Player2Controller>();
            x = (int)BC.playerList[0].transform.position.x;
            z = (int)BC.playerList[0].transform.position.z;

            if (PC2.Mobil > 0 && BC.Board[x, z + 1] == 0)
            {
                BC.playerList[0].transform.position += new Vector3(-1f, 0f, 0f);
                PC2.Mobil--;
                PC2.UpdateStatus();
                BC.UpdateBoard();
            }
        }
    }
}
