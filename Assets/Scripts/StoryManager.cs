using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StoryManager : MonoBehaviour
{
    private static StoryManager _instance;

    [SerializeField] private GameObject nextButton;
    [SerializeField] private GameObject skipButton;
    [SerializeField] private GameObject panel9EffectGameObject;
    [SerializeField] private GameObject panel9FlashForegroundGameObject;
    [SerializeField] private GameObject character;
    [SerializeField] private int typewriterWordsPerMinute = 400;

    private GameObject _activeStoryPanelGameObject;
    private Coroutine _activeTypewriterCoroutine;
    private Coroutine _panel9EffectCoroutine; // TODO: Delete if I don't need to stop this.
    private int _activeStoryPanelNumber = 0;
    private int _finalStoryPanelNumber = 10;
    private float _panel9EffectDuration = 4f;
    private float _panel9FlashForegroundEffectDuration = 2f;
    private Vector3 _panel9EffectStartSize = new Vector3(0.5f, 0.5f, 0.5f);
    private Vector3 _panel9EffectEndSize = new Vector3(4.5f, 4.5f, 4.5f);

    public static StoryManager Instance
    {
        get
        {
            if (_instance is null)
                Debug.LogError("Story Manager is NULL");

            return _instance;
        }
    }

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

    private IEnumerator StartPanel9Effect()
    {
        panel9EffectGameObject.SetActive(true);
        float timeElapsed = 0;
        while (timeElapsed < _panel9EffectDuration)
        {
            panel9EffectGameObject.transform.localScale = Vector3.Slerp(
                _panel9EffectStartSize,
                _panel9EffectEndSize,
                timeElapsed / _panel9EffectDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator StartPanel9FlashForegroundEffect()
    {
        var flashForegroundImage = panel9FlashForegroundGameObject.GetComponent<Image>();
        if (flashForegroundImage != null)
        {
            AudioManager.Instance.PlaySFXMagicTransform();
            float timeElapsed = 0;
            while (timeElapsed < _panel9FlashForegroundEffectDuration)
            {
                var newColor = new Color(1f, 1f, 1f, Mathf.Lerp(0f, 1f, timeElapsed / _panel9FlashForegroundEffectDuration));
                flashForegroundImage.color = newColor;
                timeElapsed += Time.deltaTime;
                yield return null;
            }
        }
        _activeStoryPanelGameObject.SetActive(false);
        PlayFinalPanel();
    }

    private IEnumerator Typewriter(TextMeshProUGUI textObject, int wordsPerMinute)
    {
        if (textObject == null) yield return null;

        var text = textObject.text;
        var timeElapsed = 0f;
        var secondsPerCharacter = 1 / ((wordsPerMinute * 5) / 60f);
        var charactersProcessed = 0;
        var textOnScreen = string.Empty;
        var colorTop = new Color(194f/255f, 232f/255f, 212f/255f);
        var colorBottom = new Color(95f/255f, 152f/255f, 201f/255f);
        textObject.colorGradient = new VertexGradient(colorTop, colorTop, colorBottom, colorBottom);
        textObject.SetText(string.Empty);
        while (charactersProcessed < text.Length)
        {
            if (timeElapsed >= secondsPerCharacter)
            {
                textOnScreen = text.Substring(0, charactersProcessed + 1);
                charactersProcessed++;
                textObject.SetText(textOnScreen);
                timeElapsed = 0;
            }
            timeElapsed += Time.deltaTime;
            yield return null;
        }
    }

    private void Play2D()
    {
        nextButton.SetActive(false);
        skipButton.SetActive(false);
        GameManager.Instance.Play2D();
    }

    private void Play3D()
    {
        nextButton.SetActive(false);
        skipButton.SetActive(false);
        GameManager.Instance.Play3D();
    }

    private void PlayFinalPanel()
    {
        nextButton.SetActive(false);
        skipButton.SetActive(false);
        _activeStoryPanelNumber = _finalStoryPanelNumber;
        GoToStoryPanel(_activeStoryPanelNumber);
    }

    public void NextButtonClicked()
    {
        // Panels with special cases:
        // 5 -> Switch to 2D
        // 8 -> Switch to 3D
        // 9 -> Explosion -> 3D Character -> Panel 10
        if (_activeTypewriterCoroutine != null)
        {
            StopCoroutine(_activeTypewriterCoroutine);
        }
        _activeStoryPanelGameObject.SetActive(false);
        if (_activeStoryPanelNumber == 5)
        {
            Play2D();
        }
        else if (_activeStoryPanelNumber == 8)
        {
            Play3D();
        }
        else if (_activeStoryPanelNumber == 9)
        {
            _activeStoryPanelGameObject.SetActive(true);
            nextButton.SetActive(false);
            skipButton.SetActive(false);
            StartCoroutine(StartPanel9FlashForegroundEffect());
        }
        else
        {
            _activeStoryPanelNumber++;
            GoToStoryPanel(_activeStoryPanelNumber);
        }
    }

    // TODO: Still need skip button functionality
    public void SkipButtonClicked()
    {
        if (_activeTypewriterCoroutine != null)
        {
            StopCoroutine(_activeTypewriterCoroutine);
        }
        _activeStoryPanelGameObject.SetActive(false);
        if (_activeStoryPanelNumber <= 5)
        {
            Play2D();
        }
        else if (_activeStoryPanelNumber <= 8)
        {
            Play3D();
        }
        else if (_activeStoryPanelNumber <= 9)
        {
            PlayFinalPanel();
        }
    }

    public void StartStory()
    {
        PlayerPrefs.SetInt("IsFirstTimePlay", 0);
        _activeStoryPanelNumber = 1;
        GoToStoryPanel(_activeStoryPanelNumber);
        AudioManager.Instance.StopAllAudio();
        AudioManager.Instance.PlaySFXBirds();
    }

    public void GoToStoryPanel(int storyPanelNumber)
    {
        GameManager.Instance.isStoryMode = true;
        CanvasManager.Instance.ShowScores(false);
        CanvasManager.Instance.ShowMainMenuButton(false);
        var storyPanel = transform.Find($"Panel{storyPanelNumber}");
        if (storyPanel != null)
        {
            _activeStoryPanelGameObject = storyPanel.gameObject;
            _activeStoryPanelGameObject.SetActive(true);
            _activeStoryPanelNumber = storyPanelNumber;
            // TODO: If there was more time and this was a bigger game,
            // find a way to better contain story panel logic
            if (_activeStoryPanelNumber == 2)
            {
                AudioManager.Instance.PlaySFXExclamation();
            }
            if (_activeStoryPanelNumber == 3)
            {
                AudioManager.Instance.PlayMusicPanels3To5();
            }
            if (_activeStoryPanelNumber == 6)
            {
                AudioManager.Instance.PlayMusicPanels6To8();
            }
            if (_activeStoryPanelNumber == 9)
            {
                AudioManager.Instance.StopAllAudio();
                AudioManager.Instance.PlaySFXRumble();
                _panel9EffectCoroutine = StartCoroutine(StartPanel9Effect());
            }
            if (_activeStoryPanelNumber == 10)
            {
                AudioManager.Instance.PlayMusicPanelFinal();
                CanvasManager.Instance.ShowScores(true);
                CanvasManager.Instance.ShowTilesRemainingText(false);
                character.SetActive(true);
            }
            else
            {
                nextButton.SetActive(true);
                skipButton.SetActive(true);
            }
            var typewriterTextGameObject = storyPanel.Find("TypewriterText");
            if (typewriterTextGameObject == null) return;
            var typewriterText = typewriterTextGameObject.GetComponent<TextMeshProUGUI>();
            if (typewriterText == null) return;
            _activeTypewriterCoroutine = StartCoroutine(Typewriter(typewriterText, typewriterWordsPerMinute));
        }
    }
}
