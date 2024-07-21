using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private GameObject p_ActionPopup;

    public ActionState actionState;

    void Start()
    {
        EnableActionCanvas(false);
    }

    private void EnableActionCanvas(bool state) 
    {
        p_ActionPopup.SetActive(state);
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
    }
    public void NoAction()
    {
        EnableActionCanvas(false);
        actionState = ActionState.None;
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

