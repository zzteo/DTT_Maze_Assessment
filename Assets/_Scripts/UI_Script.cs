using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Script : MonoBehaviour
{
    [SerializeField] private GameObject _UI;
    [SerializeField] private GameObject _menuButton;
    [SerializeField] private GameObject _closeWindowButton;
    private Animator _anim;
    private bool isDisplayed;

    private void Awake()
    {
        _anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            FoldUnfold_UI();
        }
    }

    private bool isGeneratedFirstTime;
    public void DisplayMenuButton() //Displays the buttons that go to menu and exists menu, they are displyed after the generate button has been pressed the first time 
    {
        if (!isGeneratedFirstTime)
        {
            isGeneratedFirstTime = true;
            _menuButton.SetActive(true);
            _closeWindowButton.SetActive(true);
        }
    }

    public void FoldUnfold_UI()
    {
        SoundManager.Instance.PlayAudio(0, 1);

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
