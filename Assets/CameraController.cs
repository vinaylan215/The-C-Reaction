using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController instance;
    public CinemachineVirtualCamera cam1;
    private CinemachineVirtualCamera cam2;
    private CinemachineVirtualCamera cam3;
    public float transitionDuration = 1f;
    private CinemachineBrain cinemachineBrain;
    public List<GameObject> Player = new List<GameObject>();
    private int currentCamIndex = 1;
    public Vector3 cam1NewPosition;
    GameObject lastActiveGameObject;
    GameObject firstActiveGameObject;

    private void Awake()
    {
        instance = this;
    }

    public void Init()
    {
        // Reset the currentCamIndex
        currentCamIndex = 1;

        // Ensure the CinemachineBrain component is attached to the main camera
        if (cinemachineBrain == null)
        {
            cinemachineBrain = Camera.main.GetComponent<CinemachineBrain>();
        }

        // Deactivate all cameras initially
        foreach (var player in Player)
        {
            if (player != null)
            {
                Transform camTransform = player.transform.GetChild(3);
                if (camTransform != null)
                {
                    camTransform.gameObject.SetActive(false);
                }
            }
        }

        // Find the last active game object
        lastActiveGameObject = FindLastActiveGameObject();
        if (lastActiveGameObject != null)
        {
            Debug.Log("The last active game object is: " + lastActiveGameObject.name);
            Transform camTransform = lastActiveGameObject.transform.GetChild(3);
            if (camTransform != null)
            {
                cam3 = camTransform.GetComponent<CinemachineVirtualCamera>();
                if (cam3 != null)
                {
                    cam3.Priority = 20;
                    cam3.gameObject.SetActive(true);
                }
                else
                {
                    Debug.LogError("No CinemachineVirtualCamera component found on last active game object's child.");
                }
            }
            else
            {
                Debug.LogError("The last active game object does not have a third child.");
            }
        }
        else
        {
            Debug.LogError("No active game objects found.");
        }

        // Find the first active game object
        firstActiveGameObject = FindFirstActiveGameObject();
        if (firstActiveGameObject != null)
        {
            Debug.Log("The first active game object is: " + firstActiveGameObject.name);
            Transform camTransform = firstActiveGameObject.transform.GetChild(3);
            if (camTransform != null)
            {
                cam2 = camTransform.GetComponent<CinemachineVirtualCamera>();
                if (cam2 != null)
                {
                    cam2.Priority = 0;
                    cam2.gameObject.SetActive(true);
                }
                else
                {
                    Debug.LogError("No CinemachineVirtualCamera component found on first active game object's child.");
                }
            }
            else
            {
                Debug.LogError("The first active game object does not have a third child.");
            }
        }
        else
        {
            Debug.LogError("No active game objects found.");
        }

        // Turn off all other cameras in the list
        for (int i = 0; i < Player.Count; i++)
        {
            if (Player[i] != null && Player[i] != firstActiveGameObject && Player[i] != lastActiveGameObject)
            {
                Transform camTransform = Player[i].transform.GetChild(3);
                if (camTransform != null)
                {
                    camTransform.gameObject.SetActive(false);
                }
                else
                {
                    Debug.LogError($"Player[{i}] does not have a third child.");
                }
            }
        }

        // Set initial priorities to ensure cam1 starts focused
        cam1.Priority = 10;
        cam1.gameObject.SetActive(true);

        // Start initial transition if cam3 is not null
        if (cam3 != null)
        {
            StartCoroutine(InitialTransition());
        }
    }

    GameObject FindLastActiveGameObject()
    {
        for (int i = Player.Count - 1; i >= 0; i--)
        {
            if (Player[i] != null && Player[i].activeInHierarchy)
            {
                return Player[i];
            }
        }
        return null;
    }

    GameObject FindFirstActiveGameObject()
    {
        for (int i = 0; i < Player.Count; i++)
        {
            if (Player[i] != null && Player[i].activeInHierarchy)
            {
                return Player[i];
            }
        }
        return null;
    }

    private IEnumerator InitialTransition()
    {
        Debug.Log("Starting initial transition to cam3...");
        yield return StartCoroutine(SwitchCamera(cam1, cam3, transitionDuration));

        Debug.Log("Waiting for 2 seconds at cam3...");
        yield return new WaitForSeconds(2f);

        Debug.Log("Transitioning back to cam1...");
        UIManager.Instance.SetStartUpText("");
        yield return StartCoroutine(SwitchCamera(cam3, cam1, transitionDuration));

        Debug.Log("First transition complete");

        // Now start the next transitions
        StartCoroutine(TransitionFrom1To2AndBack());
    }

    private IEnumerator TransitionFrom1To2AndBack()
    {
        Debug.Log("Starting transition from cam1 to cam2...");
        yield return StartCoroutine(SwitchCamera(cam1, cam2, transitionDuration));

        Debug.Log("Second Transition from cam1 to cam2 Complete");
        UIManager.Instance.EnableButtonPanel(true);
        // Simulate receiving a callback after some action
        //yield return StartCoroutine(WaitForCallback());
        //StartCoroutine(WaitForCallback());
    }

    public void NextCameraMove(float value)
    {
        Debug.LogError("Next Move");
        StartCoroutine(WaitForCallback(value));
    }

    private IEnumerator WaitForCallback(float value)
    {
        yield return new WaitForSeconds(value);

        if (currentCamIndex <= Player.Count - 1)
        {
            Player[currentCamIndex].transform.GetChild(3).gameObject.SetActive(true);
            CinemachineVirtualCamera nextCam = Player[currentCamIndex].transform.GetChild(3).GetComponent<CinemachineVirtualCamera>();

            yield return StartCoroutine(SwitchCamera(cam2, nextCam, transitionDuration));

            Debug.Log("Transition to next camera complete");

            currentCamIndex++;
        }
    }

    private IEnumerator SwitchCamera(CinemachineVirtualCamera fromCam, CinemachineVirtualCamera toCam, float duration)
    {
        fromCam.Priority = 10;
        toCam.Priority = 20;

        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime * 2.0f;
            yield return null;
        }
        while (cinemachineBrain.IsBlending)
        {
            yield return null;
        }

        Debug.Log($"Completed transition from {fromCam.name} to {toCam.name}");
    }
}
