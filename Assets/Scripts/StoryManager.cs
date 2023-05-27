using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StoryManager : MonoBehaviour
{
    private static StoryManager _instance;

    [SerializeField] private GameObject nextButton;
    [SerializeField] private GameObject skipButton;
    [SerializeField] private int typewriterWordsPerMinute = 400;

    private GameObject _activeStoryPanelGameObject;
    private Coroutine _activeTypewriterCoroutine;
    private int _activeStoryPanelNumber = 0;

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
            nextButton.SetActive(false);
            skipButton.SetActive(false);
            GameManager.Instance.Play2D();
        }
        else if (_activeStoryPanelNumber == 8)
        {

        }
        else if (_activeStoryPanelNumber == 9)
        {

        }
        else
        {
            _activeStoryPanelNumber++;
            GoToStoryPanel(_activeStoryPanelNumber);
        }
    }

    public void SkipButtonClicked()
    {
        if (_activeTypewriterCoroutine != null)
        {
            StopCoroutine(_activeTypewriterCoroutine);
        }
        _activeStoryPanelGameObject.SetActive(false);
        nextButton.SetActive(false);
        skipButton.SetActive(false);
    }

    public void StartStory()
    {
        _activeStoryPanelNumber = 1;
        GoToStoryPanel(_activeStoryPanelNumber);
    }

    public void GoToStoryPanel(int storyPanelNumber)
    {
        var storyPanel = transform.Find($"Panel{_activeStoryPanelNumber}");
        if (storyPanel != null)
        {
            _activeStoryPanelGameObject = storyPanel.gameObject;
            _activeStoryPanelGameObject.SetActive(true);
            nextButton.SetActive(true);
            skipButton.SetActive(true);
            var typewriterTextGameObject = storyPanel.Find("TypewriterText");
            if (typewriterTextGameObject == null) return;
            var typewriterText = typewriterTextGameObject.GetComponent<TextMeshProUGUI>();
            if (typewriterText == null) return;
            _activeTypewriterCoroutine = StartCoroutine(Typewriter(typewriterText, typewriterWordsPerMinute));
        }
    }
}
