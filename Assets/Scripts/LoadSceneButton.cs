using UnityEngine;

public class LoadSceneButton : MonoBehaviour
{
    [SerializeField] private SceneType sceneToLoad;
    
    public void LoadScene()
    {
        TransitionManager.Instance.StartFadeOutIn(sceneToLoad);   
    }
}
