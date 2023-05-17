using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Script : MonoBehaviour
{
    [SerializeField] private GameObject UI;
    [SerializeField] private GameObject _menuButton;
    private Animator _anim;
    private bool isDisplayed;

    private void Awake()
    {
        _anim = GetComponent<Animator>();
    }

    private bool isGeneratedFirstTime;
    public void DisplayMenuButton()
    {
        if (!isGeneratedFirstTime)
        {
            isGeneratedFirstTime = true;
            _menuButton.SetActive(true);
        }
    }

    public void FoldUnfold_UI()
    {
        if (!isDisplayed)
        {
            isDisplayed = true;
            _anim.Play("Fold_UI");
        }
        else
        {
            isDisplayed = false;
            _anim.Play("Unfold_UI");
        }
    }
}
