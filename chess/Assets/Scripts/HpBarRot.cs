using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HpBarRot : MonoBehaviour
{
    public BattleController BC;

    private void Start()
    {
        BC = GameObject.Find("BattleManager").GetComponent<BattleController>();
    }

    private void FixedUpdate()
    {
        if (BC.battleState == BattleController.State.Player2Turn)
        {          
            Quaternion TargetRot = Quaternion.Euler(60, 180, 0);     
            transform.rotation = TargetRot;
        }
        else
        {          
            Quaternion TargetRot = Quaternion.Euler(60, 0, 0);
            transform.rotation = TargetRot;
        }

    }
}
