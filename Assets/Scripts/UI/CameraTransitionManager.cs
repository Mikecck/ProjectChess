using UnityEngine;
using Cinemachine;
using System.Collections;

public class CameraTransitionManager : MonoBehaviour
{
    [Header("Virtual Cameras")]
    [SerializeField] private CinemachineVirtualCamera menuCamera;
    [SerializeField] private CinemachineVirtualCamera gameCamera;
    
    [Header("Transition Settings")]
    [SerializeField] private float transitionTime = 1.5f;
    [SerializeField] private AnimationCurve transitionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        
    private void Awake()
    {
        cinemachineBrain = Camera.main.GetComponent<CinemachineBrain>();
        if (cinemachineBrain == null)
        {
            Debug.LogError("No CinemachineBrain found on Main Camera!");
        }
    }
    
    public void TransitionToGame()
    {
        StartCoroutine(TransitionToGameRoutine());
    }
    
    public void TransitionToMenu()
    {
        StartCoroutine(TransitionToMenuRoutine());
    }
    
    private IEnumerator TransitionToGameRoutine()
    {
        gameCamera.Priority = 30;
        menuCamera.Priority = 10;
        
        yield return new WaitForSeconds(transitionTime);
        
    }
    
    private IEnumerator TransitionToMenuRoutine()
    {
        menuCamera.Priority = 30;
        gameCamera.Priority = 10;
        
        yield return new WaitForSeconds(transitionTime);
        
    }
}