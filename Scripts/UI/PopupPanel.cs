using TMPro;
using UnityEngine;

public class PopupPanel : MonoBehaviour
{
    QTEUI qteUI;
    OperatorUI operatorUI;
    TargetMarkerUI targetMarkerUI;
    GameMessageUI gameMessageUI;
    TutorialUI tutorialUI;

    public QTEUI QteUI { get { return qteUI; } }
    public OperatorUI OperatorUI { get {return operatorUI; } }
    public TargetMarkerUI TargetMarkerUI { get {return targetMarkerUI;} }
    public GameMessageUI GameMessageUI { get {return gameMessageUI;} }
    public TutorialUI TutorialUI { get {return tutorialUI;} }

    void Awake()
    {
        qteUI = GetComponentInChildren<QTEUI>();
        operatorUI = GetComponentInChildren<OperatorUI>();
        targetMarkerUI = GetComponentInChildren<TargetMarkerUI>();
        gameMessageUI = GetComponentInChildren<GameMessageUI>();
        tutorialUI = GetComponentInChildren<TutorialUI>();
        HideQTE();
    }

    public void ShowQTE()
    {
        qteUI.gameObject.SetActive(true);
        qteUI.SetQTE();
    }

    public void HideQTE()
    {
        qteUI.gameObject.SetActive(false);
    }

    public void ShowOperator(string dialogue)
    {
        operatorUI.ShowOperatorUI(dialogue);
    }
    public void HideOperator()
    {
       operatorUI.HideOperatorUI();
    }
}
