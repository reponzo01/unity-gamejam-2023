using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CanvasManager : MonoBehaviour
{
    private static CanvasManager _instance;

    public static CanvasManager Instance
    {
        get
        {
            if (_instance is null)
                Debug.LogError("Canvas Manager is NULL");

            return _instance;
        }
    }

    [SerializeField] private GameObject threeDControls;
    [SerializeField] private GameObject powerupMatchIcon;
    [SerializeField] private GameObject powerupMultiselectIcon;
    [SerializeField] private GameObject twoDLevelProgressGameObject;
    [SerializeField] private GameObject switchTo2DButton;
    [SerializeField] private GameObject switchTo3DButton;
    [SerializeField] private GameObject mainMenuButton;
    [SerializeField] private GameObject scoresGameObject;
    [SerializeField] private Sprite twoDLevel1Sprite;
    [SerializeField] private Sprite twoDLevel2Sprite;
    [SerializeField] private Sprite twoDLevel3Sprite;
    [SerializeField] private Sprite twoDLevel3CompleteSprite;
    [SerializeField] private GameObject twoDLevelInstructionsImage;
    [SerializeField] private TextMeshProUGUI twoDLevelInstructionsText;
    [SerializeField] private TextMeshProUGUI multiselectTilesAvailableText;
    [SerializeField] private TextMeshProUGUI multiselectInstructionsText;
    [SerializeField] private TextMeshProUGUI instructionsFlashText;
    [SerializeField] private TextMeshProUGUI persistentLevelText;
    [SerializeField] private TextMeshProUGUI bestAttemptsText;
    [SerializeField] private TextMeshProUGUI currentAttemptsText;

    private Vector3 _lerpInstructionsFlashTextStartScale = new Vector3(0.5f, 0.5f, 0.5f);
    private Vector3 _lerpInstructionsFlashTextEndScale = new Vector3(2f, 2f, 2f);
    private float _lerpInstructionsFlashTextDuration = 0.2f;
    private float _instructionsFlashTextDuration = 2f;
    private bool _is2D = false;

    private void Awake()
    {
        _instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private IEnumerator LerpInstructionsFlashText(string text)
    {
        ShowPersistentLevelInstructionsText(false);
        float timeElapsed = 0f;
        instructionsFlashText.SetText(text);
        instructionsFlashText.gameObject.SetActive(true);
        instructionsFlashText.transform.localScale = _lerpInstructionsFlashTextStartScale;
        while (timeElapsed < _lerpInstructionsFlashTextDuration)
        {
            instructionsFlashText.transform.localScale = Vector3.Slerp(
                _lerpInstructionsFlashTextStartScale,
                _lerpInstructionsFlashTextEndScale,
                timeElapsed / _lerpInstructionsFlashTextDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        StartCoroutine(Utilities.CoroutineDeactivateGameObjectAfterSeconds(instructionsFlashText.gameObject, _instructionsFlashTextDuration));
        // TODO: This really shouldn't go here. But it works.
        StartCoroutine(Utilities.CoroutineActivateGameObjectAfterSeconds(persistentLevelText.gameObject, _instructionsFlashTextDuration));
    }

    public void Play3D(int level)
    {
        _is2D = false;
        Show3DControls(true);
        twoDLevelProgressGameObject.SetActive(false);
        if (!GameManager.Instance.isZenMode)
        {
            ShowScores(true);
            UpdatePersistentLevelText(level);
        }
    }

    public void Play2D(int level)
    {
        _is2D = true;
        Show3DControls(false);
        if (!GameManager.Instance.isZenMode)
        {
            ShowScores(true);
            ShowInstructionsFlashText($"Match the tiles!{System.Environment.NewLine}2D LEVEL {level}");
            UpdatePersistentLevelText(level);
            Update2DLevelProgress(level);
        }
    }

    public void ShowPowerupIcon(Utilities.PowerupEnum powerup, bool show)
    {
        switch (powerup)
        {
            case Utilities.PowerupEnum.match:
                powerupMatchIcon.SetActive(show);
                break;
            case Utilities.PowerupEnum.multiselect:
                powerupMultiselectIcon.SetActive(show);
                break;
            default:
                break;
        }
    }

    // Method name is used in UI for image click
    public void ToggleOverlayInstructions(GameObject overlayInstructions)
    {
        overlayInstructions.SetActive(!overlayInstructions.activeSelf);
        GameManager.Instance.isOverlayInstructionsActive = overlayInstructions.activeSelf;
    }

    // Method name is used in UI
    public void HideOverlayInstructions(GameObject overlayInstructions)
    {
        overlayInstructions.SetActive(false);
        GameManager.Instance.isOverlayInstructionsActive = false;
    }

    public void UpdateMultiselectTilesAvailableText(int tilesAvailable)
    {
        multiselectTilesAvailableText.SetText(tilesAvailable.ToString());
        multiselectInstructionsText.SetText($"Select up to {tilesAvailable} tiles at once!");
    }

    public void UpdatePersistentLevelText(int level)
    {
        var levelText = _is2D ? "2D" : "3D";
        var totalLevels = _is2D ? 3 : 5;
        persistentLevelText.SetText($"{levelText}{System.Environment.NewLine}LEVEL {level}/{totalLevels}");
    }

    public void Update2DLevelProgress(int level)
    {
        var levelProgressImage = twoDLevelProgressGameObject.GetComponent<Image>();
        var levelInstructionsBackgroundImage = twoDLevelInstructionsImage.GetComponent<Image>();
        if (levelProgressImage == null || levelInstructionsBackgroundImage == null) return;

        switch (level)
        {
            case 1:
                levelProgressImage.sprite = twoDLevel1Sprite;
                levelInstructionsBackgroundImage.sprite = twoDLevel1Sprite;
                twoDLevelInstructionsText.SetText("Beat this level 3 times to advance!");
                break;
            case 2:
                levelProgressImage.sprite = twoDLevel2Sprite;
                levelInstructionsBackgroundImage.sprite = twoDLevel2Sprite;
                twoDLevelInstructionsText.SetText("Beat this level 2 more times to advance!");
                break;
            case 3:
                levelProgressImage.sprite = twoDLevel3Sprite;
                levelInstructionsBackgroundImage.sprite = twoDLevel3Sprite;
                twoDLevelInstructionsText.SetText("Beat this level 1 more time to advance!");
                break;
            default:
                break;
        }

        twoDLevelProgressGameObject.SetActive(true);
    }

    public void ShowInstructionsFlashText(string text)
    {
        StartCoroutine(LerpInstructionsFlashText(text));
    }

    public void Show3DControls(bool show)
    {
        threeDControls.SetActive(show);
    }

    public void ShowPersistentLevelInstructionsText(bool show)
    {
        persistentLevelText.gameObject.SetActive(show);
    }

    public void ShowScores(bool show)
    {
        scoresGameObject.SetActive(show);
    }

    public void SetBestAttemptsScore(int score)
    {
        bestAttemptsText.SetText(score.ToString());
    }

    public void SetCurrentAttemptsScore(int score)
    {
        currentAttemptsText.SetText(score.ToString());
    }

    public void ActivateZenMode()
    {
        switchTo2DButton.SetActive(true);
        switchTo3DButton.SetActive(true);
        mainMenuButton.SetActive(true);
    }
}
