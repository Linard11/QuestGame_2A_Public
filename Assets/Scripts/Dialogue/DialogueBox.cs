using System;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

public class DialogueBox : MonoBehaviour
{
    public static event Action<DialogueBox> DialogueContinued;
    
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

    #endregion

    #region Unity Event Functions

    private void Awake()
    {
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
        // TODO Set dialogueSpeaker
        dialogueText.SetText(dialogueLine.text);
        
        // Read out other information such as speaker images.
    }
}
