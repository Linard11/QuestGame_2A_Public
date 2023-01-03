using System;
using System.Collections;
using System.Collections.Generic;

using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;

using Ink.Runtime;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

public class DialogueBox : MonoBehaviour
{
    public static event Action<DialogueBox> DialogueContinued;
    public static event Action<DialogueBox, int> ChoiceSelected;
    
    #region Inspector

    [Tooltip("Text component that displays the currently speaking actor.")]
    [SerializeField] private TextMeshProUGUI dialogueSpeaker;
    
    [Tooltip("Text component that contains the displayed dialogue.")]
    [SerializeField] private TextMeshProUGUI dialogueText;

    [Tooltip("Button to continue the dialogue.")]
    [SerializeField] private Button continueButton;

    [Header("Choices")]
    [Tooltip("Transform container that holds buttons for each available choice.")]
    [SerializeField] private Transform choiceContainer;

    [Tooltip("Prefab for the choice buttons.")]
    [SerializeField] private Button choiceButtonPrefab;
    
    // TODO Add dotween parameter

    #endregion

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

    #region Unity Event Functions

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        
        continueButton.onClick.AddListener(
            () =>
            {
                DialogueContinued?.Invoke(this);
            }
        );
    }

    private void OnEnable()
    {
        dialogueSpeaker.SetText(string.Empty);
        dialogueText.SetText(string.Empty);
    }

    #endregion

    public void DisplayText(DialogueLine dialogueLine)
    {
        if (dialogueLine.speaker != null)
        {
            dialogueSpeaker.SetText(dialogueLine.speaker);
        }
        dialogueText.SetText(dialogueLine.text);
        
        // Read out other information such as speaker images.
        
        DisplayButtons(dialogueLine.choices);
    }

    private void DisplayButtons(List<Choice> choices)
    {
        Selectable newSelection;
        
        // If DialogueLine has no Choices show continueButton.
        if (choices == null || choices.Count == 0)
        {
            ShowContinueButton(true);
            ShowChoice(false);
            newSelection = continueButton;
        }
        else // Show the Choices.
        {
            ClearChoices();
            List<Button> choiceButtons = GenerateChoices(choices);
            
            ShowContinueButton(false);
            ShowChoice(true);
            newSelection = choiceButtons[0];
        }

        StartCoroutine(DelayedSelect(newSelection));
    }

    private List<Button> GenerateChoices(List<Choice> choices)
    {
        List<Button> choiceButtons = new List<Button>();

        for (int i = 0; i < choices.Count; i++)
        {
            Choice choice = choices[i];

            Button button = Instantiate(choiceButtonPrefab, choiceContainer);

            int index = i;
            button.onClick.AddListener(
                () =>
                {
                    ChoiceSelected?.Invoke(this, index);
                }
            );
            
            TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
            buttonText.SetText(choice.text);
            button.name = choice.text;
            
            choiceButtons.Add(button);
        }
        
        return choiceButtons;
    }

    private void ClearChoices()
    {
        foreach (Transform child in choiceContainer)
        {
            Destroy(child.gameObject);
        }
    }

    private void ShowContinueButton(bool show)
    {
        continueButton.gameObject.SetActive(show);
    }

    private void ShowChoice(bool show)
    {
        choiceContainer.gameObject.SetActive(show);
    }

    private IEnumerator DelayedSelect(Selectable newSelection)
    {
        yield return null;
        
        newSelection.Select();
    }

    #region Animations

    public Tween DOShow()
    {
        float height = rectTransform.rect.height;

        this.DOKill();
        return DOTween.Sequence(this)
                      .Append(DOMove(Vector2.zero).From(new Vector2(0, -height)))
                      .Join(DOFade(1).From(0));
    }

    public Tween DOHide()
    {
        float height = rectTransform.rect.height;

        this.DOKill();
        Sequence sequence = DOTween.Sequence(this);

        sequence.Append(DOMove(new Vector2(0, -height)).From(Vector2.zero));
        sequence.Join(DOFade(0).From(1));

        return sequence;
    }

    private TweenerCore<Vector2, Vector2, VectorOptions> DOMove(Vector2 targetPosition)
    {
        return rectTransform.DOAnchorPos(targetPosition, 0.75f).SetEase(Ease.OutBack);
    }

    private TweenerCore<float, float, FloatOptions> DOFade(float targetAlpha)
    {
        return canvasGroup.DOFade(targetAlpha, 0.75f).SetEase(Ease.InOutSine);
    }

    #endregion
}
