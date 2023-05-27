using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    [SerializeField] private TextMeshProUGUI multiselectTilesAvailableText;
    [SerializeField] private TextMeshProUGUI multiselectInstructionsText;
    [SerializeField] private TextMeshProUGUI powerupFlashText;

    private float _lerpPowerupFlashTextDuration = 0.2f;
    private Vector3 _lerpPowerupFlashTextStartScale = new Vector3(0.5f, 0.5f, 0.5f);
    private Vector3 _lerpPwerupFlashTExtEndScale = new Vector3(3f, 3f, 3f);

    private void Awake()
    {
        _instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        Play2D();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private IEnumerator LerpPowerupFlashText()
    {
        float timeElapsed = 0f;
        powerupFlashText.transform.localScale = _lerpPowerupFlashTextStartScale;
        while (timeElapsed < _lerpPowerupFlashTextDuration)
        {
            powerupFlashText.transform.localScale = Vector3.Slerp(
                _lerpPowerupFlashTextStartScale,
                _lerpPwerupFlashTExtEndScale,
                timeElapsed / _lerpPowerupFlashTextDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        StartCoroutine(Utilities.CoroutineDeactivateGameObjectAfterSeconds(powerupFlashText.gameObject, .5f));
    }

    public void Play3D()
    {
        threeDControls.SetActive(true);
    }

    public void Play2D()
    {
        threeDControls.SetActive(false);
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
    public void TogglePowerupInstructions(GameObject powerupInstructions)
    {
        powerupInstructions.SetActive(!powerupInstructions.activeSelf);
        GameManager.Instance.isPowerupInstructionsActive = powerupInstructions.activeSelf;
    }

    // Method name is used in UI
    public void HidePowerupInstructions(GameObject powerupInstructions)
    {
        powerupInstructions.SetActive(false);
        GameManager.Instance.isPowerupInstructionsActive = false;
    }

    public void UpdateMultiselectTilesAvailableText(int tilesAvailable)
    {
        multiselectTilesAvailableText.SetText(tilesAvailable.ToString());
        multiselectInstructionsText.SetText($"Select up to {tilesAvailable} tiles at once!");
    }

    public void ShowPowerupFlashText(Utilities.PowerupEnum powerup)
    {
        switch (powerup)
        {
            case Utilities.PowerupEnum.match:
                powerupFlashText.SetText("Automatch!");
                break;
            case Utilities.PowerupEnum.multiselect:
                powerupFlashText.SetText("Multiselect!");
                break;
            default:
                break;
        }
        powerupFlashText.gameObject.SetActive(true);
        StartCoroutine(LerpPowerupFlashText());
    }

    // // TESTING
    // public void StartTypewriter()
    // {
    //     string text = "This is a test of the emergency broadcast system. This is only a test.";
    //     StartCoroutine(Typewriter(text, typewriterWordsPerMinute));
    // }
}
