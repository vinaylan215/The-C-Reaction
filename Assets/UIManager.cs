using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private GameObject p_ActionPopup;
    [SerializeField]
    private GameObject p_LevelFailedPopup;
    [SerializeField]
    private GameObject p_GameEndPopup;
    [SerializeField]
    private TMP_Text p_StartUpText;

    public ActionState actionState;
    public static UIManager Instance;
    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        EnableActionCanvas(false);
        EnableLevelFailedPanel(false);
        EnableGameEndPanel(false);
    }

    private void EnableActionCanvas(bool state) 
    {
        p_ActionPopup.SetActive(state);
        p_LevelFailedPopup.SetActive(false);
        p_GameEndPopup.SetActive(false);
    }
    public void EnableLevelFailedPanel(bool state)
    {
        p_LevelFailedPopup.SetActive(state);
        p_ActionPopup.SetActive(false);
        p_GameEndPopup.SetActive(false);
    }
    public void EnableGameEndPanel(bool state)
    {
        p_GameEndPopup.SetActive(state);
        p_ActionPopup.SetActive(false);
        p_LevelFailedPopup.SetActive(false);
    }
    public void SetStartUpText(string value) 
    {
        p_StartUpText.text = "Make this character "+ value;
    }
    public void Wave() 
    {
        EnableActionCanvas(true);
        actionState = ActionState.Wave;
    }
    public void Dance()
    {
        EnableActionCanvas(true);
        actionState = ActionState.Dance;
    }
    public void Cheer()
    {
        EnableActionCanvas(true);
        actionState = ActionState.Cheer;
    }
    public void Kick()
    {
        EnableActionCanvas(true);
        actionState = ActionState.Kick;
    }
    public void Punch()
    {
        EnableActionCanvas(true);
        actionState = ActionState.Punch;
    }
    public void Spin()
    {
        EnableActionCanvas(true);
        actionState = ActionState.Spin;
    }
    public void Jump()
    {
        EnableActionCanvas(true);
        actionState = ActionState.Jump;
    }
    public void Pickup()
    {
        EnableActionCanvas(true);
        actionState = ActionState.Pickup;
    }
    public void YesAction() 
    {
        EnableActionCanvas(false);
        Debug.Log("Action to Perform "+actionState);
        GameManager.Instance.PlayAnimationInChain(actionState);
    }
    public void NoAction()
    {
        EnableActionCanvas(false);
        actionState = ActionState.None;
    }

    public void OnClick_Retry()
    {
        EnableActionCanvas(false);
        // TODO: transition camera to player 1 & enable button panel
    }

    public void OnClick_Restart()
    {
        EnableActionCanvas(false);
        GameManager.Instance.RestartGame();
    }
}
public enum ActionState 
{
    Wave,
    Dance,
    Cheer,
    Kick,
    Punch,
    Spin,
    Jump,
    Pickup,
    None
}

