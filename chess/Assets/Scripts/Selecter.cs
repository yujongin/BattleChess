using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selecter : MonoBehaviour
{
    public GameObject Shadow;
    public Transform Position;
    public BattleController BC;
    public MeshFilter MF;
    public Mesh Pawn;
    public Mesh Queen;
    public Mesh Bishop;
    public Mesh King;
    public Mesh Rook;
    public Mesh Knight;

    public SoundManager SM;
    void Start()
    {
        BC = GameObject.Find("BattleManager").GetComponent<BattleController>();
        SM = GameObject.Find("SoundManager").GetComponent<SoundManager>();
    }

    private void OnMouseEnter()
    {
        GameObject Object = Instantiate(Shadow);
        MF = Object.GetComponent<MeshFilter>();
        if (BC.useSkill2)
        {
            if(BC.Target.tag == "Pawn")
            {
                MF.mesh = Pawn;
            }
            if (BC.Target.tag == "Bishop")
            {
                MF.mesh = Bishop;
            }
            if (BC.Target.tag == "Queen")
            {
                MF.mesh = Queen;
            }
            if (BC.Target.tag == "Rook")
            {
                MF.mesh = Rook;
            }
            if (BC.Target.tag == "Knight")
            {
                MF.mesh = Knight;
            }

        }
        
        Object.transform.position = Position.transform.position;

    }

    void OnMouseExit()
    {
        Destroy(GameObject.Find("Shadow(Clone)"));
    }
    ParticleSystem PS;
    ParticleSystem PS2;
    private void OnMouseDown()
    {
        if (!BC.useSkill2)
        {
            GameObject Object = Instantiate(BC.KingSkillEffect);
            BC.EffectList.Add(Object);
            Object.transform.position = new Vector3(this.gameObject.transform.position.x, this.gameObject.transform.position.y + 0.2f, this.gameObject.transform.position.z);
            PS = Object.GetComponent<ParticleSystem>();
            PS.Play();
            GameObject Object2 = Instantiate(BC.KingSkillEffect);
            BC.EffectList.Add(Object2);
            BC.DestroySelecter();
            Destroy(GameObject.Find("Shadow(Clone)"));
            BC.ResetTargetSwitch();
            
            BC.King = false;
            if (BC.battleState == BattleController.State.Player1Turn)
            {
                Skill skill = BC.Player1.Skill[0].GetComponent<Skill>();
                BC.player1Cost -= skill.SkillCost;
                BC.UpdateCost();
                Object2.transform.position = new Vector3(BC.Player1.transform.position.x, BC.Player1.transform.position.y + 0.2f, BC.Player1.transform.position.z);
                PS2 = Object2.GetComponent<ParticleSystem>();
                PS2.Play();
                SoundManager.instance.PlaySoundOneTime(SM.MagicSkill);
                BC.Player1.transform.position = Position.transform.position;
                BC.UpdateBoard();
                BC.Player1.checkAttacked = false;
            }
            else
            {
                Skill skill = BC.Player2.Skill[0].GetComponent<Skill>();
                BC.player2Cost -= skill.SkillCost;
                BC.UpdateCost();
                Object2.transform.position = new Vector3(BC.Player2.transform.position.x, BC.Player2.transform.position.y + 0.2f, BC.Player2.transform.position.z);
                PS2 = Object2.GetComponent<ParticleSystem>();
                PS2.Play();
                SoundManager.instance.PlaySoundOneTime(SM.MagicSkill);
                BC.Player2.transform.position = Position.transform.position;
                BC.UpdateBoard();
                BC.Player2.checkAttacked = false;
            }
        }
        else
        {
            GameObject Object = Instantiate(BC.KingSkillEffect);
            BC.EffectList.Add(Object);
            Object.transform.position = new Vector3(BC.Target.transform.position.x, BC.Target.transform.position.y + 0.2f, BC.Target.transform.position.z);
            PS = Object.GetComponent<ParticleSystem>();
            PS.Play();
            BC.DestroySelecter();
            Destroy(GameObject.Find("Shadow(Clone)"));
            BC.ResetTargetSwitch();
            BC.King = false;
            BC.Target.transform.position = Position.transform.position;
            GameObject Object2 = Instantiate(BC.KingSkillEffect);
            BC.EffectList.Add(Object2);
            Object2.transform.position = new Vector3(BC.Target.transform.position.x, BC.Target.transform.position.y + 0.2f, BC.Target.transform.position.z);
            PS2 = Object.GetComponent<ParticleSystem>();
            PS2.Play();
            SoundManager.instance.PlaySoundOneTime(SM.MagicSkill);
            BC.UpdateBoard();
            if (BC.battleState == BattleController.State.Player1Turn)
            {
                Skill skill = BC.Player1.Skill[1].GetComponent<Skill>();
                BC.player1Cost -= skill.SkillCost;
                BC.UpdateCost();
                BC.Player1.checkAttacked = false;
            }                    
            else
            {
                Skill skill = BC.Player2.Skill[1].GetComponent<Skill>();
                BC.player2Cost -= skill.SkillCost;
                BC.UpdateCost();
                BC.Player2.checkAttacked = false;
            }

        }
       
    } 
}
