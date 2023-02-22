using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Animator _hintAnimator;

    private int _score;

    private void Start()
    {
        
    }

    public void VisualizeHints(Toggle toggle)
    {
        _hintAnimator.SetBool("Visualize", toggle.isOn);
    }

    public void PauseGameplay()
    {
        Time.timeScale = 0f;
    }

    public void UnpauseGameplay()
    {
        Time.timeScale =  1f;
    }

    public void GoBackToMenu()
    {

    }

    public void Restart()
    {

    }
}
