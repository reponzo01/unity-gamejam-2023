using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    public static GameManager Instance
    {
        get
        {
            if (_instance is null)
                Debug.LogError("Game Manager is NULL");

            return _instance;
        }
    }

    [SerializeField]
    private List<Material> emojis;

    [SerializeField]
    private GameObject mainCube4x4;

    [SerializeField]
    private Canvas canvas;

    [SerializeField]
    private int maxFlippedTilesAllowed = 2;

    private List<Tile> _flippedTiles;
    private List<int> _3DPanelNumberList = new List<int> {2, 3, 4, 5, 6};
    private int _tilesInPlay = 0;
    private bool _is2D = false;
    private MainCube _mainCube4x4Component;
    private CameraMovement _cameraMovementComponent;
    private CanvasManager _canvasManagerComponent;

    private void Awake()
    {
        _instance = this;

        QualitySettings.vSyncCount = 0;  // VSync must be disabled
        Application.targetFrameRate = 60;
    }

    // Start is called before the first frame update
    private void Start()
    {
        _mainCube4x4Component = mainCube4x4.GetComponent<MainCube>();
        _cameraMovementComponent = Camera.main.GetComponent<CameraMovement>();
        _canvasManagerComponent = canvas.GetComponent<CanvasManager>();

        SwitchTo2D();
        PopulateActiveTiles();
    }

    // Update is called once per frame
    private void Update()
    {
        if (!_is2D)
        {
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                Up();
            }
            else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                Down();
            }
            else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                Left();
            }
            else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                Right();
            }
        }
    }

    private void PopulateActiveTiles()
    {
        _flippedTiles = new List<Tile>();
        _tilesInPlay = 0;
        List<GameObject> allActiveTiles = GetAllActiveTiles();

        if (allActiveTiles.Count < 0) return;
        _tilesInPlay = allActiveTiles.Count;

        var numEmojisToSelect = Mathf.Ceil(allActiveTiles.Count / 2);
        if (numEmojisToSelect > 0)
        {
            emojis.Shuffle();
            var tileIndecies = new List<int>();
            for (var i = 0; i < allActiveTiles.Count; i++)
            {
                tileIndecies.Add(i);
            }
            tileIndecies.Shuffle();

            var emojiIndex = 0;
            foreach (var index in tileIndecies)
            {
                if (emojiIndex >= numEmojisToSelect)
                {
                    emojiIndex = 0;
                }
                var tile = allActiveTiles[index].GetComponent<Tile>();
                if (tile != null)
                {
                    tile.SetEmojiMaterial(emojis[emojiIndex]);
                }
                emojiIndex++;
            }
        }
    }

    private List<GameObject> GetAllActiveTiles()
    {
        var allActiveTiles = new List<GameObject>();
        List<GameObject> activePanels = GetActivePanels(mainCube4x4);
        if (activePanels.Count > 0)
        {
            foreach (var panel in activePanels)
            {
                allActiveTiles.AddRange(GetActiveTilesByPanel(panel));
            }
        }
        return allActiveTiles;
    }

    private List<GameObject> GetActivePanels(GameObject cube)
    {
        var activePanels = new List<GameObject>();
        for (var i = 0; i < cube.transform.childCount; i++)
        {
            var child = cube.transform.GetChild(i);
            if (child.CompareTag("Panel") && child.gameObject.activeSelf)
            {
                activePanels.Add(child.gameObject);
            }
        }
        return activePanels;
    }

    private List<GameObject> GetActiveTilesByPanel(GameObject panel)
    {
        var activeTiles = new List<GameObject>();
        for (var i = 0; i < panel.transform.childCount; i++)
        {
            var child = panel.transform.GetChild(i);
            if (child.CompareTag("Tile") && child.gameObject.activeSelf)
            {
                activeTiles.Add(child.gameObject);
            }
        }
        return activeTiles;
    }

    private IEnumerator RestartScene()
    {
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private IEnumerator ResetTiles()
    {
        yield return new WaitForSeconds(1f);
        _flippedTiles[0].HideTile();
        _flippedTiles[1].HideTile();
        _flippedTiles.Clear();
    }

    private IEnumerator ExplodeTiles()
    {
        yield return new WaitForSeconds(0.5f);
        _flippedTiles[0].ExplodeTile();
        _flippedTiles[1].ExplodeTile();
        _tilesInPlay = _tilesInPlay - 2;
        _flippedTiles.Clear();

        if (_tilesInPlay <= 0)
        {
            StartCoroutine(RestartScene());
        }
    }

    public void TileClicked(Tile tile)
    {
        if (_flippedTiles.Count < maxFlippedTilesAllowed)
        {
            tile.ShowTile();
            _flippedTiles.Add(tile);
            // TODO: Technically, the following code does not support more than 2 max tiles flipped allowed
            if (_flippedTiles.Count == maxFlippedTilesAllowed)
            {
                if (!_flippedTiles[0].GetEmojiName().Equals(_flippedTiles[1].GetEmojiName()))
                {
                    StartCoroutine(ResetTiles());
                }
                else
                {
                    StartCoroutine(ExplodeTiles());
                }
            }
        }
    }

    public void SwitchTo3D()
    {
        _is2D = false;
        if (_mainCube4x4Component != null)
        {
            _mainCube4x4Component.SwitchTo3D();
        }

        if (_cameraMovementComponent != null)
        {
            _cameraMovementComponent.SwitchTo3D();
        }

        if (_canvasManagerComponent != null)
        {
            _canvasManagerComponent.SwitchTo3D();
        }
    }

    public void SwitchTo2D()
    {
        _is2D = true;
        if (_mainCube4x4Component != null)
        {
            _mainCube4x4Component.SwitchTo2D();
        }

        if (_cameraMovementComponent != null)
        {
            _cameraMovementComponent.SwitchTo2D();
        }

        if (_canvasManagerComponent != null)
        {
            _canvasManagerComponent.SwitchTo2D();
        }

        EnableRandomPanels(1);
    }

    public void Up()
    {
        _cameraMovementComponent.LookUp();
    }

    public void Down()
    {
        _cameraMovementComponent.LookDown();
    }

    public void Left()
    {
        _mainCube4x4Component.TurnLeft();
    }

    public void Right()
    {
        _mainCube4x4Component.TurnRight();
    }

    public void EnableRandomPanels(int numberOfPanelsToEnable)
    {
        _mainCube4x4Component.DisablePanel(2);
        _mainCube4x4Component.DisablePanel(3);
        _mainCube4x4Component.DisablePanel(4);
        _mainCube4x4Component.DisablePanel(5);
        _mainCube4x4Component.DisablePanel(6);
        _3DPanelNumberList.Shuffle();

        _mainCube4x4Component.EnablePanel(1);
        for (int i = 0; i < numberOfPanelsToEnable - 1; i++)
        {
            _mainCube4x4Component.EnablePanel(_3DPanelNumberList[i]);
        }

        PopulateActiveTiles();
    }
}
