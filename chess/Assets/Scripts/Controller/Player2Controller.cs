using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player2Controller : MonoBehaviour
{
    public BattleController BC;
    public Warrior warrior;

    public List<GameObject> Skill = new List<GameObject>();

    public GameObject StatusPanel;
    public StatusText status;
    
    
    public GameObject SkillPanel;
    public SkillText skillText;
    public Skill skill;

    public int x;
    public int z;
    public float Mobil;

    public bool TargetSwitch = false;

    public bool checkMoved = true;
    public bool checkAttacked = true;
    public bool Pawn = false;
    public enum State
    {
        wait,
        turnEnd
    }
    public State player2;
    void Start()
    {
        BC = GameObject.Find("BattleManager").GetComponent<BattleController>();
        ResetPosition();
        Mobil = warrior.Mobility;
    }

    void Update()
    {
        UpdateHpBar();
        switch (player2)
        {
            case (State.wait):
                break;
            case (State.turnEnd):
                BC.HidePanel();
                BC.ResetState();
                UpdateStatus();
                Pawn = false;
                BC.battleState = BattleController.State.Wait;
                player2 = State.wait;
                break;
        }
    }
  
    void OnMouseDown()
    {
        if (BC.battleState == BattleController.State.Player2Turn && !TargetSwitch)
        {
            CreateStatus();
            BC.playerList.Add(this.gameObject);
            BC.ActionPanel.SetActive(true);
            BC.ResetTargetSwitch();
            if (BC.MovePanel.activeSelf)
            {
                BC.MovePanel.SetActive(false);
                BC.ActionPanel.SetActive(true);
            }
            else if (BC.SkillPanel.activeSelf)
            {
                BC.SkillPanel.SetActive(false);
                BC.ActionPanel.SetActive(true);
            }

        }
        if (TargetSwitch)
        {
            if (BC.Knight)
            {
                BC.Target = this.gameObject;
                BC.KnightAttack();
                BC.Knight = false;
                BC.ResetTargetSwitch();
            }
            else if (BC.Bishop)
            {
                BC.Target = this.gameObject;
                BC.BishopAttack();
                BC.Bishop = false;
                BC.ResetTargetSwitch();
            }
            else if (Pawn)
            {
                if (3 <= BC.player2Cost)
                {
                    BC.Target = this.gameObject;
                    BC.PawnHeal();
                    if (this.gameObject.name == "PawnDark")
                    {
                        CreateStatus();
                    }
                    BC.player2Cost -= 3;
                    BC.UpdateCost();
                    if (BC.player2Cost == 0)
                    {
                        BC.ResetTargetSwitch();
                    }
                    Pawn = false;
                }
                
            }
            else if (BC.Queen)
            {
                BC.Target = this.gameObject;
                BC.QueenAttack();
                BC.player1Cost -= 4;
                BC.UpdateCost();
                BC.Queen = false;
                BC.ResetTargetSwitch();
                BC.ResetList();
                BC.QUseClick = false;
            }
            else if (BC.King)
            {
                if (!BC.useSkill2)
                {
                    BC.Target = this.gameObject;
                    BC.DestroySelecter();
                    if (this.gameObject.name != "KingDark")
                        BC.Appearance();
                }
                else
                {
                    BC.DestroySelecter();
                    if (this.gameObject.name != "KingDark")
                    {
                        BC.Target = this.gameObject;
                        BC.Sommoning();
                    }
                }

            }
            else
            {
                BC.Target = this.gameObject;
                BC.Attack();
            }
        
        }

    }

/*    MeshRenderer MR;
    private void OnMouseEnter()
    {
        if (BC.Queen&& TargetSwitch)
        {
                if(BC.Player1.transform.position.z - this.gameObject.transform.position.z < 0)
                {
                    for (int i = 0; i < BC.N.Count; i++)
                    {
                        MR = BC.N[i].GetComponent<MeshRenderer>();
                        MR.material = BC.TargetColor;
                    }
                }
                else if (BC.Player1.transform.position.z - this.gameObject.transform.position.z > 0)
                {
                    for (int i = 0; i < BC.S.Count; i++)
                    {
                        MR = BC.S[i].GetComponent<MeshRenderer>();
                        MR.material = BC.TargetColor;
                    }
                }

                else if (BC.Player1.transform.position.x - this.gameObject.transform.position.x < 0)
                {
                    for (int i = 0; i < BC.E.Count; i++)
                    {
                        MR = BC.E[i].GetComponent<MeshRenderer>();
                        MR.material = BC.TargetColor;
                    }
                }
                else if (BC.Player1.transform.position.x - this.gameObject.transform.position.x > 0)
                {
                    for (int i = 0; i < BC.W.Count; i++)
                    {
                        MR = BC.W[i].GetComponent<MeshRenderer>();
                        MR.material = BC.TargetColor;
                    }
                }
            
            
        }
    }

    private void OnMouseExit()
    {
        if (BC.Queen && TargetSwitch)
        {
            if (BC.Player1.transform.position.z - this.gameObject.transform.position.z < 0)
            {
                for (int i = 0; i < BC.N.Count; i++)
                {
                    MR = BC.N[i].GetComponent<MeshRenderer>();
                    MR.material = BC.DarkPawn;
                }
            }
            else if (BC.Player1.transform.position.z - this.gameObject.transform.position.z > 0)
            {
                for (int i = 0; i < BC.S.Count; i++)
                {
                    MR = BC.S[i].GetComponent<MeshRenderer>();
                    MR.material = BC.DarkPawn;
                }
            }

            else if (BC.Player1.transform.position.x - this.gameObject.transform.position.x < 0)
            {
                for (int i = 0; i < BC.E.Count; i++)
                {
                    MR = BC.E[i].GetComponent<MeshRenderer>();
                    MR.material = BC.DarkPawn;
                }
            }
            else if (BC.Player1.transform.position.x - this.gameObject.transform.position.x > 0)
            {
                for (int i = 0; i < BC.W.Count; i++)
                {
                    MR = BC.W[i].GetComponent<MeshRenderer>();
                    MR.material = BC.DarkPawn;
                }
            }
        }
    }*/

    public void CreateSkillPanel()
    {
        skillText = SkillPanel.GetComponent<SkillText>();
        skill = Skill[0].GetComponent<Skill>();
        skillText.skillName.text = skill.SkillName;
        skillText.explain.text = skill.Explain;
        skillText.Cost.text = "소모값: " + skill.SkillCost;
    }
    public void KingSkillPanel()
    {
        skillText = SkillPanel.GetComponent<SkillText>();
        skill = Skill[1].GetComponent<Skill>();
        skillText.skillName.text = skill.SkillName;
        skillText.explain.text = skill.Explain;
        skillText.Cost.text = "소모값: " + skill.SkillCost;
    }

    void CreateStatus()
    {
        status = StatusPanel.GetComponent<StatusText>();
        status.Name.text = warrior.theName;
        status.HP.text = "체력: " + warrior.curHP;
        status.Attack.text = "공격력: " + warrior.curATK;
        if (warrior.theName == "Bishop")        
            status.Range.text = "사거리: 대각선 2";       
        else
            status.Range.text = "사거리: " + warrior.Range;
        status.Mobility.text = "이동력: " + Mobil;
    }

    void ResetPosition()
    {
        BC.player2Pos.Add(this.gameObject);
    }

    public void UpdateStatus()
    {
        status.Mobility.text = "이동력: " + Mobil;    
    }

    public Image hpBar;
    public Text hpText;
    public Text NameText;
    public void UpdateHpBar()
    {
        float curHP = warrior.curHP;
        float baseHP = warrior.baseHP;
        hpBar.fillAmount = curHP / baseHP;
        hpText.text = curHP + "/" + baseHP;
        NameText.text = warrior.theName;
    }
}
