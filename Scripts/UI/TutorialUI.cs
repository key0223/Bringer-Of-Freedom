using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI tutorialTitleText;
    [SerializeField] TextMeshProUGUI tutorialPageTitleText;
    [SerializeField] TextMeshProUGUI descriptionText;
    [SerializeField] Image descriptionImage;

    [Header("Page Settings")]
    [SerializeField] GameObject pageButtonRoot;
    [SerializeField] TextMeshProUGUI pageIndexText;
    [SerializeField] Button prevButton;
    [SerializeField] Button nextButton;
    [SerializeField] Button closeButton;

    TutorialData currentTutorial;
    int currentIndex;

    void Awake()
    {
        prevButton.onClick.AddListener(OnClickPrev);
        nextButton.onClick.AddListener(OnClickNext);
        closeButton.onClick.AddListener(OnClickClose);

    }

    void Start()
    {
        gameObject.SetActive(false);
        currentIndex = 0;
        Refresh();
    }

    void OnEnable()
    {
        currentIndex = 0;
        Refresh();
    }

    public void ShowTutorial(TutorialData tutorial)
    {
        currentTutorial = tutorial;
        gameObject.SetActive(true);
        Refresh();
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    void Refresh()
    {
        if (currentTutorial == null || currentTutorial.pages.Length == 0) return;

        TutorialDataBase page = currentTutorial.pages[currentIndex];
        tutorialTitleText.text = currentTutorial.tutorialTitle;
        tutorialPageTitleText.text = page.tutorialPageTitle;
        descriptionText.text = page.description;
        descriptionImage.sprite = page.tutorialImage;
        descriptionImage.gameObject.SetActive(page.tutorialImage !=null);

        pageIndexText.text = $"{currentIndex + 1:00}/ {currentTutorial.pages.Length:00}";

        bool hasMultiplePages = currentTutorial.pages.Length > 1;
        pageButtonRoot.SetActive(hasMultiplePages);

        if(hasMultiplePages )
        {
            prevButton.interactable = currentIndex > 0;
            nextButton.interactable = currentIndex < currentTutorial.pages.Length - 1;
        }
    }


    void OnClickPrev()
    {
        if (currentIndex <= 0) return;
        currentIndex--;
        Refresh();
    }

    void OnClickNext()
    {
        if (currentIndex >= currentTutorial.pages.Length - 1) return;
        currentIndex++;
        Refresh();
    }

    void OnClickClose()
    {
        gameObject.SetActive(false);
        Time.timeScale = 1f;
        //Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    [Space(10)]
    [SerializeField] string testTutorialName;

    [ContextMenu("Tutorial Test")]
    public void TutorialTest()
    {
        TutorialData tutorialData;
        DataManager.Instance.TutorialDict.TryGetValue(testTutorialName, out tutorialData);

        if (tutorialData == null) return;

        ShowTutorial(tutorialData);
    }
}
