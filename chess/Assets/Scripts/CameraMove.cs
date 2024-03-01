 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public BattleController BC;

    private void Start()
    {
        BC = GameObject.Find("BattleManager").GetComponent<BattleController>();
    }

    private void Update()
    {
/* 
        if (Input.GetMouseButton(0))
        {
            moveObjectFunc();
        }*/

        if (BC.battleState == BattleController.State.Player2Turn)
        {
            Vector3 TargetPos = new Vector3(3.5f, 14f, 12f);
            Quaternion TargetRot = Quaternion.Euler(60, 180, 0);
            transform.position = Vector3.Lerp(transform.position, TargetPos, Time.deltaTime * 2f);
            transform.rotation = Quaternion.Slerp(transform.rotation, TargetRot, Time.deltaTime * 2f);
        }
        else
        {
            Vector3 TargetPos = new Vector3(3.5f, 14f, -5f);
            Quaternion TargetRot = Quaternion.Euler(60, 0, 0);
            transform.position = Vector3.Lerp(transform.position, TargetPos, Time.deltaTime * 2f);
            transform.rotation = Quaternion.Slerp(transform.rotation, TargetRot, Time.deltaTime * 2f);
        }
        
       
    }

    private float speed_rota = 2.0f;

    void moveObjectFunc()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        transform.Rotate(Vector3.up * speed_rota * mouseX);
        transform.Rotate(Vector3.left * speed_rota * mouseY);
    }
}
