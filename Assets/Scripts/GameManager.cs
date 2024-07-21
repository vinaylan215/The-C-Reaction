using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    int m_nTotalAnimations = 10;
    [SerializeField]
    Animator[] m_arrAnimators = null;
    [SerializeField]
    int m_nTotalCharacters = 3;
    [SerializeField]
    GameLevel[] m_arrGameLevels = null;
    [SerializeField]
    CameraController m_cameraController = null;

    public static GameManager Instance;

    ChainNode[] m_arrChainNodes;
    int m_nCharacterAnimationsDone = 0;
    int m_nLevelIndex = 0;
    ActionState m_actionStateFinal;
    ActionState m_actionStateCurrent;
    int m_nTotalLevels;

    void Awake()
    {
        Instance = this;

        // initialize chain reaction logic
        m_arrChainNodes = new ChainNode[m_nTotalAnimations];
        for (int l_nIndex = 0; l_nIndex < m_nTotalAnimations; l_nIndex++)
        {
            m_arrChainNodes[l_nIndex] = new ChainNode{
                m_nCurrentNode = l_nIndex,
                m_nNextNode = l_nIndex
            };
        }

        // randomize or shuffle chain
        ShuffleChain();
        for (int l_nIndex = 0; l_nIndex < m_nTotalAnimations; l_nIndex++)
        {
            Debug.LogError("m_arrChainNodes["+l_nIndex+"].m_nCurrentNode: "+m_arrChainNodes[l_nIndex].m_nCurrentNode);
            Debug.LogError("m_arrChainNodes["+l_nIndex+"].m_nNextNode: "+m_arrChainNodes[l_nIndex].m_nNextNode);
        }

        // Load level
        m_nTotalLevels = m_arrGameLevels.Length;
        LoadLevel();
    }

    void LoadLevel()
    {
        if (m_nLevelIndex >= m_nTotalLevels)
        {
            Debug.Log("GAME WIN");
            UIManager.Instance.EnableGameEndPanel(true);
            return;
        }
        m_nTotalCharacters = m_arrGameLevels[m_nLevelIndex].m_nTotalCharacters;
        m_actionStateFinal = m_arrGameLevels[m_nLevelIndex].m_actionStateFinal;
        Debug.Log("m_actionStateFinal "+ m_actionStateFinal);
        UIManager.Instance.SetStartUpText("");
        // disable unwanted players
        List<GameObject> l_listPlayers = m_cameraController.Player;
        foreach (GameObject l_player in l_listPlayers)
        {
            l_player.SetActive(false);
        }
        for (int l_nIndex = 0; l_nIndex < m_nTotalCharacters; l_nIndex++)
        {
            l_listPlayers[l_nIndex].SetActive(true);
        }
        m_cameraController.Init();
    }

    void ShuffleChain()
    {
        List<int> l_listUniqueNumbers = new List<int>();
        for (int l_nIndex = 0; l_nIndex < m_nTotalAnimations; l_nIndex++)
        {
            l_listUniqueNumbers.Add(l_nIndex);
        }
        int l_nFirstNumber = 0;
        for (int l_nIndex = 0; l_nIndex < m_nTotalAnimations; l_nIndex++)
        {
            int l_random = UnityEngine.Random.Range(0, l_listUniqueNumbers.Count);
            int l_nPickedNumber = l_listUniqueNumbers[l_random];
            if (l_nIndex == 0)
                l_nFirstNumber = l_nPickedNumber;
            l_listUniqueNumbers.Remove(l_nPickedNumber);
            m_arrChainNodes[l_nIndex].m_nCurrentNode = l_nPickedNumber;
            if (l_nIndex != 0)
                m_arrChainNodes[l_nIndex - 1].m_nNextNode = l_nPickedNumber;
        }
        m_arrChainNodes[m_nTotalAnimations - 1].m_nNextNode = l_nFirstNumber;
    }

    public void PlayAnimationInChain(ActionState a_animation)
    {
        if (m_nCharacterAnimationsDone >= m_nTotalCharacters)
        {
            m_nCharacterAnimationsDone = 0;
            if (m_actionStateCurrent == m_actionStateFinal)
            {
                Debug.Log("LEVEL WIN");
                m_nLevelIndex++;
                LoadLevel();
            }
            else
            {
                Debug.Log("LEVEL FAILED - RETRY");
                UIManager.Instance.EnableLevelFailedPanel(true);
                //m_cameraController.Init();
            }
            return;
        }
        
        switch (a_animation)
        {
            case ActionState.Wave:
                m_arrAnimators[m_nCharacterAnimationsDone].SetTrigger("Wave");
                break;
            case ActionState.Cheer:
                m_arrAnimators[m_nCharacterAnimationsDone].SetTrigger("Cheer");
                break;
            case ActionState.Dance:
                m_arrAnimators[m_nCharacterAnimationsDone].SetTrigger("Dance");
                break;
            case ActionState.Kick:
                m_arrAnimators[m_nCharacterAnimationsDone].SetTrigger("Kick");
                break;
            case ActionState.Punch:
                m_arrAnimators[m_nCharacterAnimationsDone].SetTrigger("Punch");
                break;
            case ActionState.Jump:
                m_arrAnimators[m_nCharacterAnimationsDone].SetTrigger("Jump");
                break;
            case ActionState.Pickup:
                m_arrAnimators[m_nCharacterAnimationsDone].SetTrigger("Pickup");
                break;
            case ActionState.Spin:
                m_arrAnimators[m_nCharacterAnimationsDone].SetTrigger("Spin");
                break;
        }
        int l_nAnimationIndex = (int)a_animation;
        m_actionStateCurrent = a_animation;
        int l_nNextAnimationIndex = GetNextChainIndex(l_nAnimationIndex);
        m_nCharacterAnimationsDone++;
        m_cameraController.NextCameraMove();
        StartCoroutine(WaitForSecond(3.0f, ()=>{
            PlayAnimationInChain((ActionState)l_nNextAnimationIndex);
        }));
    }

    int GetNextChainIndex(int a_nAnimationIndex)
    {
        for (int l_nIndex = 0; l_nIndex < m_nTotalAnimations; l_nIndex++)
        {
            if (m_arrChainNodes[l_nIndex].m_nCurrentNode == a_nAnimationIndex)
                return m_arrChainNodes[l_nIndex].m_nNextNode;
        }
        return 0;
    }

    IEnumerator WaitForSecond(float a_fWait, Action a_callback)
    {
        yield return new WaitForSeconds(a_fWait);
        a_callback?.Invoke();
    }

    public void RestartGame()
    {
        m_nLevelIndex = 0;
        m_nCharacterAnimationsDone = 0;
        LoadLevel();
    }
}

public class ChainNode
{
    public int m_nCurrentNode;
    public int m_nNextNode;
}

[Serializable]
public class GameLevel
{
    [SerializeField]
    public int m_nTotalCharacters;
    [SerializeField]
    public ActionState m_actionStateFinal;
}
