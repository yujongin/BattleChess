using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManage : MonoBehaviour
{
    public void LoadMain()
    {
        SceneManager.LoadScene(0);
    }
    public void LoadGame()
    {
        SceneManager.LoadScene(1);
    }
 
    public void EndGame()
    {
        Application.Quit();
    }

    public GameObject RulePanel;
    public GameObject RulePanel2;

    public void OpenRule()
    {
        RulePanel.SetActive(true);
        RulePanel2.SetActive(false);
    }
    public void Next()
    {
        RulePanel.SetActive(false);
        RulePanel2.SetActive(true);
    }
    public void OK()
    {
        RulePanel2.SetActive(false);
    }

}
