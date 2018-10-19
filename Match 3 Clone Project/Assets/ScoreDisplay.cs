using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreDisplay : Singleton<ScoreDisplay> {

    [SerializeField]
    Animator animator;
    [SerializeField]
    TextMeshPro scoreText;

    public override void Awake() {
        base.Awake();
        scoreText.enabled = false;
    }

    public void DisplayScore(int popCount) 
    {
        scoreText.enabled = true;
        scoreText.text = "Popped " + popCount;
        animator.SetTrigger("trigger_display_score");
    }

    public void HideText() 
    {
        scoreText.enabled = false;
    }
}
