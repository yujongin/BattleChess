using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

public class BattleController : MonoBehaviour
{
    public int player1Cost = 0;
    public int player2Cost = 0;
    public List<GameObject> playerList = new List<GameObject>();

    public List<GameObject> player1Pos = new List<GameObject>();
    public List<GameObject> player2Pos = new List<GameObject>();

    public List<GameObject> EffectList = new List<GameObject>();
    public Player1Controller Player1;
    public Player2Controller Player2;
    public int[,] Board = new int[10, 10];
    public Material choiceLight;
    public Material LightPawn;
    public Material DarkPawn;
    public Material TargetColor;
    private MeshRenderer MR;

    public GameObject StatusPanel;
    public GameObject ActionPanel;
    public GameObject SkillPanel;
    public GameObject MovePanel;
    public GameObject OXPanel;

    public Button Skill1;
    public Button Skill2;

    public GameObject AttackEffect;

    public SoundManager SM;

    public Text WhiteCost;
    public Text BlackCost;

    public enum State
    {
        Wait,
        AddCost,
        Player1Turn,
        Player2Turn,

    }

    public State battleState;

    void Start()
    {
        SM = GameObject.Find("SoundManager").GetComponent<SoundManager>();
        UpdateBoard();
        ActionPanel.SetActive(false);
        StatusPanel.SetActive(false);
        MovePanel.SetActive(false);
        SkillPanel.SetActive(false);
        AttackEffect.SetActive(false);
        SoundManager.instance.PlaySoundLoop();
    }


    void Update()
    {
        Timer();
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ESCKEY();
        }
        switch (battleState)
        {
            case (State.Wait):
                HidePanel();
                battleState = State.AddCost;
                break;
            case (State.AddCost):
                Timer();
                player1Cost = player1Cost + 8;
                player2Cost = player2Cost + 8;
                UpdateCost();
                battleState = State.Player1Turn;
                break;
            case (State.Player1Turn):
                if (playerList.Count > 0)
                {
                    Player1 = GameObject.Find(playerList[0].name).GetComponent<Player1Controller>();
                    ChangeColor();
                    StatusPanel.SetActive(true);
                }
                break;
            case (State.Player2Turn):
                if (playerList.Count > 0)
                {
                    Player2 = GameObject.Find(playerList[0].name).GetComponent<Player2Controller>();
                    ChangeColor();
                    StatusPanel.SetActive(true);
                }
                break;
        }
    }


    //Timer
    public Text TimerText;
    public Image TimerBar;
    public float baseTime = 60;
    public float curTime = 60;
    public void Timer()
    {            
        curTime -= Time.deltaTime;
        TimerText.text = Mathf.Floor(curTime).ToString();
        TimerBar.fillAmount = curTime / baseTime; 
        if(curTime <= 0)
        {
            curTime = baseTime;
            SkillCancel();
            TurnEndButton();
        }
    }

    //자신이 움직이고 싶은 말을 클릭했을 경우 색깔 변경
    void ChangeColor()
    {
        if (battleState == State.Player1Turn)
        {
            if (playerList.Count == 1)
            {
                MR = playerList[0].GetComponent<MeshRenderer>();
                MR.material = choiceLight;
            }
            else if (playerList.Count == 2)
            {
                MR = playerList[0].GetComponent<MeshRenderer>();
                MR.material = LightPawn;
                playerList.RemoveAt(0);
            }
        }
        if (battleState == State.Player2Turn)
        {
            if (playerList.Count == 1)
            {
                MR = playerList[0].GetComponent<MeshRenderer>();
                MR.material = choiceLight;
            }
            else if (playerList.Count == 2)
            {
                MR = playerList[0].GetComponent<MeshRenderer>();
                MR.material = DarkPawn;
                playerList.RemoveAt(0);
            }
        }
    }

    //코스트 업데이트
    public void UpdateCost()
    {
        WhiteCost.text = "백) 행동력: " + player1Cost;
        BlackCost.text = "흑) 행동력: " + player2Cost;
    }
    public void HidePanel()
    {
        StatusPanel.SetActive(false);
        ActionPanel.SetActive(false);
        MovePanel.SetActive(false);
        SkillPanel.SetActive(false);
    }

    public void HideMovePanel()
    {
        MovePanel.SetActive(false);
        ActionPanel.SetActive(true);
    }

    //end 버튼 
    public void TurnEndButton()
    {
        if (battleState == State.Player1Turn)
        {
           
            ResetTargetSwitch();
            DestroySkillEffect();
            if (playerList.Count != 0)
            {
                MR = playerList[0].GetComponent<MeshRenderer>();
                MR.material = LightPawn;
                playerList.RemoveAt(0);
                Player1.player1 = Player1Controller.State.turnEnd;
            }
            else
                battleState = State.Player2Turn;
            HidePanel();
            curTime = baseTime;
                     
        }
        else if (battleState == State.Player2Turn)
        {
            
            DestroySkillEffect();
            ResetTargetSwitch();
            if (playerList.Count != 0)
            {
                MR = playerList[0].GetComponent<MeshRenderer>();
                MR.material = DarkPawn;
                playerList.RemoveAt(0);
                Player2.player2 = Player2Controller.State.turnEnd;
            }
            else
                battleState = State.Wait;
            HidePanel();
            curTime = baseTime;
                  
        }
    }
    public void UpdateBoard()
    {
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                if (i == 0 || i == 9 || j == 0 || j == 9)
                {
                    Board[i, j] = 3;
                }
                else
                {
                    Board[i, j] = 0;
                }


            }
        }
        int x;
        int z;
        for (int i = 0; i < player1Pos.Count; i++)
        {
            x = (int)player1Pos[i].transform.position.x;
            z = (int)player1Pos[i].transform.position.z;
            Board[x + 1, z + 1] = 1;
        }

        for (int i = 0; i < player2Pos.Count; i++)
        {
            x = (int)player2Pos[i].transform.position.x;
            z = (int)player2Pos[i].transform.position.z;
            Board[x + 1, z + 1] = 2;
        }
    }

    //턴이 다시 돌아왔을 때 사용 , 모든 행동력 초기화
    public void ResetState()
    {
        for (int i = 0; i < player1Pos.Count; i++)
        {
            Player1 = GameObject.Find(player1Pos[i].name).GetComponent<Player1Controller>();
            Player1.Mobil = Player1.warrior.Mobility;
            Player1.checkMoved = true;
            Player1.checkAttacked = true;
        }
        for (int i = 0; i < player2Pos.Count; i++)
        {
            Player2 = GameObject.Find(player2Pos[i].name).GetComponent<Player2Controller>();
            Player2.Mobil = Player2.warrior.Mobility;
            Player2.checkMoved = true;
            Player2.checkAttacked = true;
        }
    }
    //------------------------이동-----------------------------
    private int x;
    private int z;

    //이동 버튼이 있는 패널 생성
    //코스트 차감
    public void CreateMovePanel()
    {
        if (battleState == State.Player1Turn && Player1.checkMoved&&player1Cost>=1)
        {
            MovePanel.SetActive(true);
            ActionPanel.SetActive(false);
            
        }
        else if (battleState == State.Player2Turn && Player2.checkMoved&&player2Cost>=1)
        {
            MovePanel.SetActive(true);
            ActionPanel.SetActive(false);
           
        }


    }
    //북
    public void MoveN()
    {
        if(battleState == State.Player1Turn)
        {
            Moving(1, 2, 0f, 1f);
        }
        else
        {
            Moving(1, 0, 0f, -1f);
        }
        SoundManager.instance.PlaySoundOneTime(SM.MoveSound3);
    }
    //동
    public void MoveE()
    {
        if (battleState == State.Player1Turn)
        {
            Moving(2, 1, 1f, 0f);
        }
        else
        {
            Moving(0, 1, -1f, 0f);
        }
        SoundManager.instance.PlaySoundOneTime(SM.MoveSound3);
    }
    //남
    public void MoveS()
    {
        if (battleState == State.Player1Turn)
        {
            Moving(1, 0, 0f, -1f);
        }
        else
        {
            Moving(1, 2, 0f, 1f);
        }
        SoundManager.instance.PlaySoundOneTime(SM.MoveSound3);
    }
    //서
    public void MoveW()
    {
        if (battleState == State.Player1Turn)
        {
            Moving(0, 1, -1f, 0f);
        }
        else
        {
            Moving(2, 1, 1f, 0f);
        }
        SoundManager.instance.PlaySoundOneTime(SM.MoveSound3);
    }
    //오브젝트를 어떤 방향으로 움직일지 결정하는 함수
    public void Moving(int AddX, int AddZ, float PosX, float PosZ)
    {
        if (battleState == BattleController.State.Player1Turn)
        {

            x = (int)playerList[0].transform.position.x;
            z = (int)playerList[0].transform.position.z;

            if (Player1.Mobil > 0 && Board[x + AddX, z + AddZ] == 0)
            {
                playerList[0].transform.position += new Vector3(PosX, 0f, PosZ);
                Player1.Mobil--;
                if (Player1.Mobil == 0)
                {
                    ActionPanel.SetActive(true);
                    MovePanel.SetActive(false);

                }
                Player1.UpdateStatus();
                UpdateBoard();
                if(Player1.checkMoved == true)
                {
                    player1Cost--;
                    UpdateCost();
                    Player1.checkMoved = false;
                }
                
            }
        }
        else if (battleState == BattleController.State.Player2Turn)
        {
            x = (int)playerList[0].transform.position.x;
            z = (int)playerList[0].transform.position.z;

            if (Player2.Mobil > 0 && Board[x + AddX, z + AddZ] == 0)
            {
                playerList[0].transform.position += new Vector3(PosX, 0f, PosZ);
                Player2.Mobil--;
                if (Player2.Mobil == 0)
                {
                    ActionPanel.SetActive(true);
                    MovePanel.SetActive(false);

                }
                Player2.UpdateStatus();
                UpdateBoard();
                if (Player2.checkMoved == true)
                {
                    player2Cost--;
                    UpdateCost();
                    Player2.checkMoved = false;
                }
            }
        }
    }


    //---------------------------기본 공격------------------------------------
    public GameObject Target;
    public Player2Controller TargetComponent;
    public Player1Controller TargetComponent2;

    //공격하지 않고 어택버튼을 다시 눌렀을때 원상태로 돌리기 위해서 
    public bool AttackButton = false;


    //attack버튼을 눌렀을 때 공격 가능한 상대 말을 노란색으로 변경
    //비숍은 사거리계산 방식이 다르기 때문에 따로 적용 
    public void Attackable()
    {
        if (!AttackButton)
        {
            int AttackerX = (int)playerList[0].transform.position.x;
            int AttackerZ = (int)playerList[0].transform.position.z;
            if (battleState == State.Player1Turn && Player1.checkAttacked && player1Cost >= 1)
            {

                for (int i = 0; i < player2Pos.Count; i++)
                {
                    int TargetX = (int)player2Pos[i].transform.position.x;
                    int TargetZ = (int)player2Pos[i].transform.position.z;
                    if (Math.Abs(AttackerX - TargetX) + Math.Abs(AttackerZ - TargetZ) <= Player1.warrior.Range)
                    {
                        MR = player2Pos[i].GetComponent<MeshRenderer>();
                        MR.material = TargetColor;
                        TargetComponent = player2Pos[i].GetComponent<Player2Controller>();
                        TargetComponent.TargetSwitch = true;
                        AttackButton = true;                      
                        if (playerList[0].name == "BishopLight")
                        {
                            if (Math.Abs(AttackerX - TargetX) != Math.Abs(AttackerZ - TargetZ) || AttackerX == TargetX || AttackerZ == TargetZ)
                            {
                                MR = player2Pos[i].GetComponent<MeshRenderer>();
                                MR.material = DarkPawn;
                                TargetComponent = player2Pos[i].GetComponent<Player2Controller>();
                                TargetComponent.TargetSwitch = false;
                            }
                        }
                    }
                }
            }
            else if (battleState == State.Player2Turn && Player2.checkAttacked && player2Cost >= 1)
            {

                for (int i = 0; i < player1Pos.Count; i++)
                {
                    int TargetX = (int)player1Pos[i].transform.position.x;
                    int TargetZ = (int)player1Pos[i].transform.position.z;
                    if (Math.Abs(AttackerX - TargetX) + Math.Abs(AttackerZ - TargetZ) <= Player2.warrior.Range)
                    {
                        MR = player1Pos[i].GetComponent<MeshRenderer>();
                        MR.material = TargetColor;
                        TargetComponent2 = player1Pos[i].GetComponent<Player1Controller>();
                        TargetComponent2.TargetSwitch = true;
                        AttackButton = true;
                        
                        if (playerList[0].name == "BishopDark")
                        {
                            if (Math.Abs(AttackerX - TargetX) != Math.Abs(AttackerZ - TargetZ) || AttackerX == TargetX || AttackerZ == TargetZ)
                            {
                                MR = player1Pos[i].GetComponent<MeshRenderer>();
                                MR.material = LightPawn;
                                TargetComponent2 = player1Pos[i].GetComponent<Player1Controller>();
                                TargetComponent2.TargetSwitch = false;
                            }
                        }
                    }

                }
            }
        }
        else
        {
            AttackButton = false;
            ResetTargetSwitch();
        }

    }


    public ParticleSystem PS;
    public ParticleSystem PS2;
    GameObject Object;
    GameObject Object2;

    //AttackSwitch가 true인 적 말을 클릭했을 경우 클릭한 말의 체력을 attacker의 공격력 만큼 차감
    //행동한 팀의 코스트 차감
    public void Attack()
    {
        if (battleState == State.Player1Turn)
        {
            TargetComponent = Target.GetComponent<Player2Controller>();
            TargetComponent.warrior.curHP -= Player1.warrior.curATK;
            DeadCheck();
            AttackEffect.transform.position = new Vector3(Target.transform.position.x, Target.transform.position.y + 0.5f, Target.transform.position.z);
            AttackEffect.SetActive(true);
            PS = AttackEffect.GetComponent<ParticleSystem>();
            PS.Play();
            SoundManager.instance.PlaySoundOneTime(SM.BasicAttack);
            Player1.checkAttacked = false;
            player1Cost--;
            UpdateCost();
            ResetTargetSwitch();
        }
        else if (battleState == State.Player2Turn)
        {
            TargetComponent2 = Target.GetComponent<Player1Controller>();
            TargetComponent2.warrior.curHP -= Player2.warrior.curATK;
            DeadCheck();
            AttackEffect.transform.position = new Vector3(Target.transform.position.x, Target.transform.position.y + 0.5f, Target.transform.position.z);
            AttackEffect.SetActive(true);
            PS = AttackEffect.GetComponent<ParticleSystem>();
            PS.Play();
            SoundManager.instance.PlaySoundOneTime(SM.BasicAttack);
            Player2.checkAttacked = false;
            player2Cost--;
            UpdateCost();
            ResetTargetSwitch();
        }
    }


    //Target이 된 상대팀의 말을 Target이 아니도록 변경
    public void ResetTargetSwitch()
    {
        for (int i = 0; i < player2Pos.Count; i++)
        {
            MR = player2Pos[i].GetComponent<MeshRenderer>();
            MR.material = DarkPawn;
            TargetComponent = player2Pos[i].GetComponent<Player2Controller>();
            TargetComponent.TargetSwitch = false;
            AttackButton = false;
            TargetComponent.Pawn = false;
            QUseClick = false;
        }
        for (int i = 0; i < player1Pos.Count; i++)
        {
            MR = player1Pos[i].GetComponent<MeshRenderer>();
            MR.material = LightPawn;
            TargetComponent2 = player1Pos[i].GetComponent<Player1Controller>();
            TargetComponent2.TargetSwitch = false;
            AttackButton = false;
            TargetComponent2.Pawn = false;
            QUseClick = false;
        }

    }


    //-----------------스킬-------------------------
    public void SkillPanelOn()
    {
        ResetTargetSwitch();
        ActionPanel.SetActive(false);
        SkillPanel.SetActive(true);
        if (battleState == State.Player1Turn)
        {
            Player1.CreateSkillPanel();
            if (Player1.name == "KingLight")
            {
                Skill1.GetComponent<Button>().interactable = true;
                Skill2.GetComponent<Button>().interactable = true;
                if (useSkill2)
                {
                    Player1.KingSkillPanel();
                }
            }
            else
            {
                Skill1.GetComponent<Button>().interactable = false;
                Skill2.GetComponent<Button>().interactable = false;
            }
        }

        else if (battleState == State.Player2Turn)
        {
            Player2.CreateSkillPanel();
            if (Player2.name == "KingDark")
            {
                Skill1.GetComponent<Button>().interactable = true;
                Skill2.GetComponent<Button>().interactable = true;
                if (useSkill2)
                {
                    Player2.KingSkillPanel();
                }
            }
            else
            {
                Skill1.GetComponent<Button>().interactable = false;
                Skill2.GetComponent<Button>().interactable = false;
            }
        }
    }

    //킹 스킬 변경
    public void Button1()
    {
        if (battleState == State.Player1Turn)
        {
            Player1.CreateSkillPanel();
            useSkill2 = false;
        }
        else
        {
            Player2.CreateSkillPanel();
            useSkill2 = false;
        }            
    }
    public void Button2()
    {
        if (battleState == State.Player1Turn)
        {
            Player1.KingSkillPanel();
            useSkill2 = true;
        }
        else
        {
            Player2.KingSkillPanel();
            useSkill2 = true;
        }
    }

    //스킬 사용버튼
    public void SkillUseButton()
    {
        if (battleState == State.Player1Turn )
        {
            Skill Player1Skill = Player1.Skill[0].GetComponent<Skill>();
            if (player1Cost >= Player1Skill.SkillCost) {
                if (Player1.name == "RookLight")
                {
                    RookSkill();
                }
                else if (Player1.name == "KnightLight")
                {
                    KnightSkill();
                }
                else if (Player1.name == "BishopLight")
                {
                    BishopSkill();
                }
                else if (Player1.name == "PawnLight")
                {
                    PawnSkill();
                }
                else if (Player1.name == "QueenLight")
                {
                    if (Player1.checkAttacked)                    
                        QueenSkill();              
                }
                else if (Player1.name == "KingLight")
                {
                    if (!useSkill2)
                        KingSkill();

                    else
                    {
                        Player1Skill = Player1.Skill[1].GetComponent<Skill>();
                        if (player1Cost >= Player1Skill.SkillCost)
                            KingSkill();

                    }

                }

            }
        }

        else if (battleState == State.Player2Turn)
        {
            Skill Player2Skill = Player2.Skill[0].GetComponent<Skill>();
            if (player2Cost >= Player2Skill.SkillCost)
            {
                if (Player2.name == "RookDark")
                {                  
                    RookSkill();
                }
                else if (Player2.name == "KnightDark")
                {
                    KnightSkill();
                }
                else if (Player2.name == "BishopDark")
                {
                    BishopSkill();
                }
                else if (Player2.name == "PawnDark")
                {
                    PawnSkill();
                }
                else if (Player2.name == "QueenDark")
                {
                    if (Player2.checkAttacked)                   
                        QueenSkill();
                }
                else if (Player2.name == "KingDark")
                {
                    if (!useSkill2)
                        KingSkill();

                    else
                    {
                        Player2Skill = Player2.Skill[1].GetComponent<Skill>();
                        if (player2Cost >= Player2Skill.SkillCost)
                            KingSkill();

                    }
                }
            }
        }
    }


    //스킬 캔슬 버튼
    public void SkillCancel()
    {
        ResetTargetSwitch();
        Knight = false;
        Bishop = false;
        Pawn = false;
        Queen = false;
        King = false;
        SkillPanel.SetActive(false);
        ActionPanel.SetActive(true);
        DestroySelecter();
        ResetList();
    }

    public GameObject RookSkillEffect;
    public GameObject BishopSkillEffect;
    public GameObject QueenSkillEffect;
    public GameObject KingSkillEffect;
    public GameObject PawnSkillEffect;
    public GameObject KnightSkillEffect;


    //룩 스킬
    public bool Rook = false;

    void RookSkill()
    {
        if (battleState == State.Player1Turn && Player1.checkAttacked)
        {
            int AttackerX = (int)playerList[0].transform.position.x;
            int AttackerZ = (int)playerList[0].transform.position.z;
            Skill skill = Player1.Skill[0].GetComponent<Skill>();
            for (int i = 0; i < player2Pos.Count; i++)
            {
                int TargetX = (int)player2Pos[i].transform.position.x;
                int TargetZ = (int)player2Pos[i].transform.position.z;
                if (Math.Abs(AttackerX - TargetX) + Math.Abs(AttackerZ - TargetZ) <= skill.SkillRange)
                {
                    Player2Controller Target = player2Pos[i].GetComponent<Player2Controller>();
                    GameObject Object = Instantiate(RookSkillEffect);
                    EffectList.Add(Object);
                    Object.transform.position = new Vector3(Target.transform.position.x, Target.transform.position.y + 0.5f, Target.transform.position.z);
                    PS = Object.GetComponent<ParticleSystem>();
                    PS.Play();
                    SoundManager.instance.PlaySoundOneTime(SM.RookSkill);
                    Target.warrior.curHP -= skill.SkillDamage;
                    DeadCheck();
                    Player1.checkAttacked = false;
                    Rook = true;        
                }
            }
            if (Rook)
            {
                player1Cost -= skill.SkillCost;
                UpdateCost();
                Rook = false;
            }
        }
        else if (battleState == State.Player2Turn && Player2.checkAttacked)
        {
            int AttackerX = (int)playerList[0].transform.position.x;
            int AttackerZ = (int)playerList[0].transform.position.z;
            Skill skill = Player2.Skill[0].GetComponent<Skill>();

            for (int i = 0; i < player1Pos.Count; i++)
            {
                int TargetX = (int)player1Pos[i].transform.position.x;
                int TargetZ = (int)player1Pos[i].transform.position.z;
                if (Math.Abs(AttackerX - TargetX) + Math.Abs(AttackerZ - TargetZ) <= skill.SkillRange)
                {
                    Player1Controller Target = player1Pos[i].GetComponent<Player1Controller>();
                    Target.warrior.curHP -= skill.SkillDamage;
                    GameObject Object = Instantiate(RookSkillEffect);
                    EffectList.Add(Object);
                    Object.transform.position = new Vector3(Target.transform.position.x, Target.transform.position.y + 0.5f, Target.transform.position.z);
                    PS = Object.GetComponent<ParticleSystem>();
                    PS.Play();
                    SoundManager.instance.PlaySoundOneTime(SM.RookSkill);
                    DeadCheck();
                    Player2.checkAttacked = false;
                    Rook = true;
                }
            }
            if (Rook)
            {
                player2Cost -= skill.SkillCost;
                UpdateCost();
                Rook = false;
            }
        }
    }


    //나이트 스킬
    public bool Knight = false;
    void KnightSkill()
    {
        Knight = true;
        Attackable();
    }
    public void KnightAttack()
    {
        if (battleState == State.Player1Turn)
        {
            Skill skill = Player1.Skill[0].GetComponent<Skill>();
            TargetComponent = Target.GetComponent<Player2Controller>();
            TargetComponent.warrior.curHP -= skill.SkillDamage;

            GameObject Object = Instantiate(KnightSkillEffect);
            EffectList.Add(Object);
            Object.transform.position = new Vector3(Target.transform.position.x, Target.transform.position.y + 0.5f, Target.transform.position.z);
            PS = Object.GetComponent<ParticleSystem>();
            PS.Play();
            SoundManager.instance.PlaySoundOneTime(SM.KnightSkill);

            DeadCheck();
            player1Cost -= skill.SkillCost;
            UpdateCost();
            ResetTargetSwitch();
            Player1.checkAttacked = false;
        }
        else if (battleState == State.Player2Turn)
        {
            Skill skill = Player2.Skill[0].GetComponent<Skill>();
            TargetComponent2 = Target.GetComponent<Player1Controller>();
            TargetComponent2.warrior.curHP -= skill.SkillDamage;
            GameObject Object = Instantiate(KnightSkillEffect);
            EffectList.Add(Object);
            Object.transform.position = new Vector3(Target.transform.position.x, Target.transform.position.y + 0.5f, Target.transform.position.z);
            PS = Object.GetComponent<ParticleSystem>();
            PS.Play();
            SoundManager.instance.PlaySoundOneTime(SM.KnightSkill);

            DeadCheck();
            player2Cost -= skill.SkillCost;
            UpdateCost();
            ResetTargetSwitch();
            Player2.checkAttacked = false;
        }

    }


    //비숍 스킬    
    public bool Bishop = false;
    void BishopSkill()
    {
        int AttackerX;
        int AttackerZ;
        int TargetX;
        int TargetZ;
        Skill skill = Player1.Skill[0].GetComponent<Skill>();
        if (battleState == State.Player1Turn && Player1.checkAttacked)
        {
            AttackerX = (int)playerList[0].transform.position.x;
            AttackerZ = (int)playerList[0].transform.position.z;
 
            for (int i = 0; i < player2Pos.Count; i++)
            {
                TargetX = (int)player2Pos[i].transform.position.x;
                TargetZ = (int)player2Pos[i].transform.position.z;
                if (Math.Abs(AttackerX - TargetX) + Math.Abs(AttackerZ - TargetZ) <= skill.SkillRange)
                {
                    MR = player2Pos[i].GetComponent<MeshRenderer>();
                    MR.material = TargetColor;
                    TargetComponent = player2Pos[i].GetComponent<Player2Controller>();
                    TargetComponent.TargetSwitch = true;
                    Bishop = true;
                }
            }
        }
        else if (battleState == State.Player2Turn && Player2.checkAttacked)
        {
            AttackerX = (int)playerList[0].transform.position.x;
            AttackerZ = (int)playerList[0].transform.position.z;
            for (int i = 0; i < player1Pos.Count; i++)
            {
                TargetX = (int)player1Pos[i].transform.position.x;
                TargetZ = (int)player1Pos[i].transform.position.z;
                if (Math.Abs(AttackerX - TargetX) + Math.Abs(AttackerZ - TargetZ) <= skill.SkillRange)
                {
                    MR = player1Pos[i].GetComponent<MeshRenderer>();
                    MR.material = TargetColor;
                    TargetComponent2 = player1Pos[i].GetComponent<Player1Controller>();
                    TargetComponent2.TargetSwitch = true;
                    Bishop = true;
                }
            }
        }
    }
    public void BishopAttack()
    {
        int TargetPosX = 0;
        int TargetPosZ = 0;
        Skill skill = Player1.Skill[0].GetComponent<Skill>();
        if (battleState == State.Player1Turn && Player1.checkAttacked)
        {
            player1Cost -= skill.SkillCost;
            UpdateCost();
            EffectPlay(BishopSkillEffect, Object, PS, 0.1f);
            SoundManager.instance.PlaySoundOneTime(SM.MagicSkill);
            if (Player1.transform.position.x == Target.transform.position.x)
            {             
                if (Player1.transform.position.z - Target.transform.position.z < 0)
                {  
                    BishopTarget(TargetPosX, TargetPosZ, 0, 1);
                    EffectPlay(BishopSkillEffect, Object2,PS2, 0.1f);
                }

                else
                {                   
                    BishopTarget(TargetPosX, TargetPosZ, 0, -1);
                    EffectPlay(BishopSkillEffect, Object2, PS2, 0.1f);
                }
            }
            else
            {
                if (Player1.transform.position.x - Target.transform.position.x < 0)
                {                   
                    BishopTarget(TargetPosX, TargetPosZ, 1, 0);
                    EffectPlay(BishopSkillEffect, Object2, PS2, 0.1f);
                }
                else
                {             
                    BishopTarget(TargetPosX, TargetPosZ, -1, 0);
                    EffectPlay(BishopSkillEffect, Object2, PS2, 0.1f);
                }
            }
            Player1.checkAttacked = false;
        }
        else if (battleState == State.Player2Turn && Player2.checkAttacked)
        {
            player2Cost -= skill.SkillCost;
            UpdateCost();
            EffectPlay(BishopSkillEffect, Object, PS, 0.1f);
            SoundManager.instance.PlaySoundOneTime(SM.MagicSkill);
            if (Player2.transform.position.x == Target.transform.position.x)
            {             
                if (Player2.transform.position.z - Target.transform.position.z < 0)
                {                 
                    BishopTarget(TargetPosX, TargetPosZ, 0, 1);
                    EffectPlay(BishopSkillEffect, Object2, PS2, 0.1f);
                }
                else
                {
                    BishopTarget(TargetPosX, TargetPosZ, 0, -1);
                    EffectPlay(BishopSkillEffect, Object2, PS2, 0.1f);
                }
            }
            else
            {                
                if (Player2.transform.position.x - Target.transform.position.x < 0)
                {
                    BishopTarget(TargetPosX, TargetPosZ, 1, 0);
                    EffectPlay(BishopSkillEffect, Object2, PS2, 0.1f);
                }
                else
                {             
                    BishopTarget(TargetPosX, TargetPosZ, -1, 0);
                    EffectPlay(BishopSkillEffect,Object2, PS2, 0.1f);
                }
            }
            Player2.checkAttacked = false;
        }

    }

    public void BishopTarget(int TargetX, int TargetZ, int Addx, int Addz)
    {
        for (int i = 0; i < 3; i++)
        {
            Target.transform.position = new Vector3(Target.transform.position.x + Addx, Target.transform.position.y, Target.transform.position.z + Addz);
            TargetX = (int)Target.transform.position.x;
            TargetZ = (int)Target.transform.position.z;
            if (Board[TargetX + 1, TargetZ + 1] != 0)
            {
                Target.transform.position = new Vector3(Target.transform.position.x - Addx, Target.transform.position.y, Target.transform.position.z - Addz);
                UpdateBoard();
           
            }     
        }
    }



    //폰 스킬
    public bool Pawn = false;
    void PawnSkill()
    {
        int AttackerX;
        int AttackerZ;
        int TargetX;
        int TargetZ;
        
        if (battleState == State.Player1Turn && Player1.checkAttacked)
        {
            Skill skill = Player1.Skill[0].GetComponent<Skill>();
            AttackerX = (int)playerList[0].transform.position.x;
            AttackerZ = (int)playerList[0].transform.position.z;
            for (int i = 0; i < player1Pos.Count; i++)
            {
                TargetX = (int)player1Pos[i].transform.position.x;
                TargetZ = (int)player1Pos[i].transform.position.z;

                if (Math.Abs(AttackerX - TargetX) + Math.Abs(AttackerZ - TargetZ) <= skill.SkillRange)
                {
                    TargetComponent2 = player1Pos[i].GetComponent<Player1Controller>();
                    if (TargetComponent2.warrior.curHP < TargetComponent2.warrior.baseHP)
                    {
                        MR = player1Pos[i].GetComponent<MeshRenderer>();
                        MR.material = TargetColor;
                        TargetComponent2.TargetSwitch = true;
                        TargetComponent2.Pawn = true;
                    }
                }
            }
        }
        else if (battleState == State.Player2Turn && Player2.checkAttacked)
        {
            Skill skill = Player2.Skill[0].GetComponent<Skill>();
            AttackerX = (int)playerList[0].transform.position.x;
            AttackerZ = (int)playerList[0].transform.position.z;
            UpdateCost();
            for (int i = 0; i < player2Pos.Count; i++)
            {
                TargetX = (int)player2Pos[i].transform.position.x;
                TargetZ = (int)player2Pos[i].transform.position.z;
                if (Math.Abs(AttackerX - TargetX) + Math.Abs(AttackerZ - TargetZ) <= skill.SkillRange)
                {
                    TargetComponent = player2Pos[i].GetComponent<Player2Controller>();
                    if (TargetComponent.warrior.curHP < TargetComponent.warrior.baseHP)
                    {
                        MR = player2Pos[i].GetComponent<MeshRenderer>();
                        MR.material = TargetColor;
                        TargetComponent.TargetSwitch = true;
                        TargetComponent.Pawn = true;
                    }

                }
            }
        }
    }
    public void PawnHeal()
    {
        if (battleState == State.Player1Turn)
        {
            Player1Controller PawnsTarget = Target.GetComponent<Player1Controller>();
            if (PawnsTarget.warrior.baseHP - PawnsTarget.warrior.curHP < 40)
            {
                PawnsTarget.warrior.curHP = PawnsTarget.warrior.baseHP;
                EffectPlay(PawnSkillEffect, Object, PS, 0.5f);
                SoundManager.instance.PlaySoundOneTime(SM.PawnSkill);
            }
            else
            {
                PawnsTarget.warrior.curHP += 40;
                EffectPlay(PawnSkillEffect, Object, PS, 0.5f);
                SoundManager.instance.PlaySoundOneTime(SM.PawnSkill);
            }
            PawnsTarget.TargetSwitch = false;
            MR = Target.GetComponent<MeshRenderer>();
            MR.material = LightPawn;
            Player1.checkAttacked = false;
        }
        else
        {
            Player2Controller PawnsTarget = Target.GetComponent<Player2Controller>();
            if (PawnsTarget.warrior.baseHP - PawnsTarget.warrior.curHP < 40)
            {
                PawnsTarget.warrior.curHP = PawnsTarget.warrior.baseHP;
                EffectPlay(PawnSkillEffect, Object, PS, 0.5f);
                SoundManager.instance.PlaySoundOneTime(SM.PawnSkill);
            }
            else
            {
                PawnsTarget.warrior.curHP += 40;
                EffectPlay(PawnSkillEffect, Object, PS, 0.5f);
                SoundManager.instance.PlaySoundOneTime(SM.PawnSkill);
            }
            PawnsTarget.TargetSwitch = false;
            MR = Target.GetComponent<MeshRenderer>();
            MR.material = DarkPawn;
            Player2.checkAttacked = false;
        }
    }

    //퀸 스킬
    public bool Queen;
    public bool QUseClick = false;
    public List<GameObject> N = new List<GameObject>();
    public List<GameObject> E = new List<GameObject>();
    public List<GameObject> W = new List<GameObject>();
    public List<GameObject> S = new List<GameObject>();
    public void QueenSkill()
    {
        int AttackerX = (int)playerList[0].transform.position.x;
        int AttackerZ = (int)playerList[0].transform.position.z;
        if (battleState == State.Player1Turn && Player1.checkAttacked && !QUseClick)
        {
            for (int i = 0; i < player2Pos.Count; i++)
            {
                int TargetX = (int)player2Pos[i].transform.position.x;
                int TargetZ = (int)player2Pos[i].transform.position.z;
                Skill skill = Player1.Skill[0].GetComponent<Skill>();
                if (Math.Abs(AttackerX - TargetX) + Math.Abs(AttackerZ - TargetZ) <= skill.SkillRange)
                {
                    if (AttackerX == TargetX || AttackerZ == TargetZ)
                    {
                        Queen = true;
                        TargetComponent = player2Pos[i].GetComponent<Player2Controller>();
                        TargetComponent.TargetSwitch = true;
                        MR = player2Pos[i].GetComponent<MeshRenderer>();
                        MR.material = TargetColor;
                        if (AttackerX - TargetX < 0)
                        {
                            E.Add(player2Pos[i]);
                        }
                        else if (AttackerX - TargetX > 0)
                        {
                            W.Add(player2Pos[i]);
                        }
                        else if (AttackerZ - TargetZ < 0)
                        {
                            N.Add(player2Pos[i]);
                        }
                        else if (AttackerZ - TargetZ > 0)
                        {
                            S.Add(player2Pos[i]);
                        }
                    }          
                }
            }
        }
        if (battleState == State.Player2Turn && Player2.checkAttacked && !QUseClick)
        {
            for (int i = 0; i < player1Pos.Count; i++)
            {
                int TargetX = (int)player1Pos[i].transform.position.x;
                int TargetZ = (int)player1Pos[i].transform.position.z;
                Skill skill = Player2.Skill[0].GetComponent<Skill>();
                if (Math.Abs(AttackerX - TargetX) + Math.Abs(AttackerZ - TargetZ) <= skill.SkillRange)
                {
                    if (AttackerX == TargetX || AttackerZ == TargetZ)
                    {
                        Queen = true;
                        TargetComponent2 = player1Pos[i].GetComponent<Player1Controller>();
                        TargetComponent2.TargetSwitch = true;
                        MR = player1Pos[i].GetComponent<MeshRenderer>();
                        MR.material = TargetColor;
                        if (AttackerX - TargetX < 0)
                        {
                            E.Add(player1Pos[i]);
                        }
                        else if (AttackerX - TargetX > 0)
                        {
                            W.Add(player1Pos[i]);
                        }
                        else if (AttackerZ - TargetZ < 0)
                        {
                            N.Add(player1Pos[i]);
                        }
                        else if (AttackerZ - TargetZ > 0)
                        {
                            S.Add(player1Pos[i]);
                        }
                    }
                }
            }
        }
        QUseClick = true;
    }
    public void DamageApplyPlayer1(List<GameObject> d)
    {
        for (int i = 0; i < d.Count; i++)
        {
            Skill skill = Player1.Skill[0].GetComponent<Skill>();
            Player2Controller Targets = d[i].GetComponent<Player2Controller>();
            Targets.warrior.curHP -= skill.SkillDamage;
            GameObject Object = Instantiate(QueenSkillEffect);
            EffectList.Add(Object);
            Object.transform.position = new Vector3(Targets.transform.position.x, Targets.transform.position.y + 0.5f, Targets.transform.position.z);
            PS = Object.GetComponent<ParticleSystem>();
            PS.Play();
            SoundManager.instance.PlaySoundOneTime(SM.QueenSkill);
            DeadCheck();
        }
    }
    public void DamageApplyPlayer2(List<GameObject> d)
    {
        for (int i = 0; i < d.Count; i++)
        {
            Skill skill = Player2.Skill[0].GetComponent<Skill>();
            Player1Controller Targets = d[i].GetComponent<Player1Controller>();
            Targets.warrior.curHP -= skill.SkillDamage;
            GameObject Object = Instantiate(QueenSkillEffect);
            EffectList.Add(Object);
            Object.transform.position = new Vector3(Targets.transform.position.x, Targets.transform.position.y + 0.5f, Targets.transform.position.z);
            PS = Object.GetComponent<ParticleSystem>();
            PS.Play();
            SoundManager.instance.PlaySoundOneTime(SM.QueenSkill);
            DeadCheck();
           
        }
    }
    public void QueenAttack()
    {
        int AttackerX = (int)playerList[0].transform.position.x;
        int AttackerZ = (int)playerList[0].transform.position.z;
        int TargetX = (int)Target.transform.position.x;
        int TargetZ = (int)Target.transform.position.z;
        if (battleState == State.Player1Turn && Player1.checkAttacked)
        {
            if (AttackerX - TargetX < 0)
            {
                DamageApplyPlayer1(E);
            }
            else if (AttackerX - TargetX > 0)
            {
                DamageApplyPlayer1(W);
            }
            else if (AttackerZ - TargetZ < 0)
            {
                DamageApplyPlayer1(N);
            }
            else if (AttackerZ - TargetZ > 0)
            {
                DamageApplyPlayer1(S);
            }
            Player1.checkAttacked = false;
        
        }
        if (battleState == State.Player2Turn && Player2.checkAttacked)
        {
            if (AttackerX - TargetX < 0)
            {
                DamageApplyPlayer2(E);
            }
            else if (AttackerX - TargetX > 0)
            {
                DamageApplyPlayer2(W);
            }
            else if (AttackerZ - TargetZ < 0)
            {
                DamageApplyPlayer2(N);
            }
            else if (AttackerZ - TargetZ > 0)
            {
                DamageApplyPlayer2(S);
            }
            Player2.checkAttacked = false;
        }
    }
    public void ResetList()
    {
        N.Clear();
        E.Clear();
        W.Clear();
        S.Clear();
    }


    //킹 스킬
    public GameObject Selecter;
    public List<GameObject> Selecters = new List<GameObject>();
    public bool King;
    public bool useSkill2 = false;
    public void KingSkill()
    {   
        if (battleState == State.Player1Turn && Player1.checkAttacked)
        {              
            for (int i = 0; i < player1Pos.Count; i++)
            {

                TargetComponent2 = player1Pos[i].GetComponent<Player1Controller>();
                MR = player1Pos[i].GetComponent<MeshRenderer>();
                MR.material = TargetColor;
                TargetComponent2.TargetSwitch = true;
                King = true;
            }
        }
        else if(battleState == State.Player2Turn && Player2.checkAttacked)
        {
          
            for (int i = 0; i < player2Pos.Count; i++)
            {
                TargetComponent = player2Pos[i].GetComponent<Player2Controller>();
                MR = player2Pos[i].GetComponent<MeshRenderer>();
                MR.material = TargetColor;
                TargetComponent.TargetSwitch = true;
                King = true;
            }
        }
    }
    public void Appearance()
    {
        CreateSelecter(Target);       
    }
    public void Sommoning()
    {
        CreateSelecter(playerList[0]);
    }
    public void CreateSelecter(GameObject Target)
    {
        int x = (int)Target.transform.position.x;
        int z = (int)Target.transform.position.z;
        if(Board[x + 1, z + 2] == 0) 
        {
            GameObject Object = Instantiate(Selecter);
            Object.transform.position = new Vector3(x, Target.transform.position.y, z + 1);
            Selecters.Add(Object);
        }
        if (Board[x + 1, z] == 0)
        {
            GameObject Object = Instantiate(Selecter);
            Object.transform.position = new Vector3(x, Target.transform.position.y, z - 1);
            Selecters.Add(Object);
        }
        if (Board[x + 2, z + 1] == 0)
        {
            GameObject Object = Instantiate(Selecter);
            Object.transform.position = new Vector3(x + 1, Target.transform.position.y, z);
            Selecters.Add(Object);
        }
        if (Board[x, z + 1] == 0)
        {
            GameObject Object = Instantiate(Selecter);
            Object.transform.position = new Vector3(x - 1, Target.transform.position.y, z);
            Selecters.Add(Object);
        }


    }
    public void DestroySelecter()
    {
        foreach(GameObject Selecter in Selecters)
        {
            Destroy(Selecter.gameObject);
        }      
    }

    //죽음 확인
    //게임이 끝났을 때.
    public GameObject Player1Win;
    public GameObject Player2Win;

    public void DeadCheck()
    {
        if (battleState == State.Player1Turn)
        {
            for (int i = 0; i < player2Pos.Count; i++)
            {
                Player2Controller Player2 = player2Pos[i].GetComponent<Player2Controller>();
                if (Player2.warrior.curHP <= 0)
                {
                    Destroy(player2Pos[i]);
                    player2Pos.Remove(player2Pos[i]);
                    UpdateBoard();
                    if (player2Pos.Count == 0)
                    {
                        HidePanel();
                        Player1Win.SetActive(true);
                    }
                }
            }

        }
        else
        {
            for (int i = 0; i < player1Pos.Count; i++)
            {
                Player1Controller Player1 = player1Pos[i].GetComponent<Player1Controller>();
                if (Player1.warrior.curHP <= 0)
                {
                    Destroy(player1Pos[i]);
                    player1Pos.Remove(player1Pos[i]);
                    UpdateBoard();
                    if (player1Pos.Count == 0)
                    {
                        HidePanel();
                        Player2Win.SetActive(true);
                    }
                }
            }
        }

    }
    //Effect 시작과 삭제
    public void EffectPlay(GameObject SkillEffect, GameObject EffectObject, ParticleSystem particleSystem, float Height)
    {
        EffectObject = Instantiate(SkillEffect);
        EffectList.Add(EffectObject);
        EffectObject.transform.position = new Vector3(Target.transform.position.x, Target.transform.position.y + Height, Target.transform.position.z);
        particleSystem = EffectObject.GetComponent<ParticleSystem>();
        particleSystem.Play();
    }
    public void DestroySkillEffect()
    {
        foreach (GameObject effect in EffectList)
        {
            Destroy(effect);
        }
        
    }

    public void LoadScene()
    {
        SceneManager.LoadScene(1);
    }
    public void LoadMain()
    {
        SceneManager.LoadScene(0);
    }


    public GameObject ESCPanel;
    public bool ESC = false;
    public void ESCKEY()
    {
        if (!ESC)
        {
            ESCPanel.SetActive(true);
            ESC = true;
        }
        else
        {
            ESCPanel.SetActive(false);
            ESC = false;
        }
        
    }

    public void NoButton()
    {
        ESCPanel.SetActive(false);
        ESC = false;
    }

}
