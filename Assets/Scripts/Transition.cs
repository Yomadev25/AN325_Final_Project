using UnityEngine;
using UnityEngine.SceneManagement;

public class Transition : MonoBehaviour
{
    public static Transition instance;
    public CanvasGroup transition;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void FadeIn(string scene, float duration = 1f)
    {
        transition.alpha = 0f;
        transition.blocksRaycasts = true;

        transition.LeanAlpha(1, duration).setOnComplete(() => SceneManager.LoadScene(scene));
    }

    public void FadeOut(float duration = 1f)
    {
        transition.alpha = 1f;
        transition.blocksRaycasts = false;

        transition.LeanAlpha(0, duration);
    }
}
