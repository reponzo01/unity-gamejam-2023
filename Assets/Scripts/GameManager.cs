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

    [SerializeField] private List<Material> emojis;
    [SerializeField] private List<Material> powerupMaterials;
    [SerializeField] private GameObject mainCube4x4;
    [SerializeField] private GameObject cameraShake;
    [SerializeField] private GameObject cameraParent;
    [SerializeField] private Canvas canvas;
    [SerializeField] private int maxFlippedTilesAllowed = 2;
    [SerializeField] private int powerupMultiselectDefaultTiles = 4;
    [SerializeField] private int powerupsPerPanel = 4;

    private List<Tile> _flippedTiles;
    private List<int> _3DPanelNumberList = new List<int> {2, 3, 4, 5, 6};
    private MainCube _mainCube4x4Component;
    private CameraMovement _cameraMovementComponent;
    private bool _is2D = false;
    private bool _powerupMultiselectActive = false;
    private bool _powerupMatchActive = false;
    private int _tilesOnBoard = 0;
    private int _powerupMultiselectTilesAvailable = 0;
    private int _activeLevel = 0;

    public bool isOverlayInstructionsActive = false;
    public bool isStoryMode = false;
    public bool isZenMode = false;

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
        _cameraMovementComponent = cameraParent.GetComponent<CameraMovement>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (_is2D || isStoryMode) return;
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

    private void ShakeCamera()
    {
        cameraShake.SetActive(true);
        StartCoroutine(Utilities.CoroutineDeactivateGameObjectAfterSeconds(cameraShake, 1.1f));
    }

    private void PopulateActiveTiles()
    {
        EnableAllTilesInActivePanels();
        _flippedTiles = new List<Tile>();
        _tilesOnBoard = 0;
        List<GameObject> allActiveTiles = GetAllActiveTiles();
        var tileIndecies = new List<int>();

        if (allActiveTiles.Count <= 0) return;
        _tilesOnBoard = allActiveTiles.Count;

        var numEmojisToSelect = Mathf.Ceil(allActiveTiles.Count / 2);
        if (numEmojisToSelect > 0)
        {
            emojis.Shuffle();
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
                    tile.HideTile();
                    tile.SetTileEmoji(emojis[emojiIndex]);
                    tile.tileIndex = index;
                }
                emojiIndex++;
            }
        }

        // TODO: This would be better extracted into a method since it's setting the powerups
        if (allActiveTiles.Count > 16)
        {
            var numberOfPowerups = Mathf.Ceil(allActiveTiles.Count / 16 * powerupsPerPanel);
            tileIndecies.Shuffle();

            for (var i = 0; i < Mathf.Ceil(numberOfPowerups/2); i++)
            {
                var offset = 1;
                var tile = allActiveTiles[tileIndecies[i]].GetComponent<Tile>();
                while (tile.IsPowerup())
                {
                    tile = allActiveTiles[tileIndecies[i + offset]].GetComponent<Tile>();
                    offset++;
                }
                if (tile != null)
                {
                    var originalEmojiName = tile.GetEmojiName();
                    tile.SetTilePowerup(powerupMaterials[Random.Range(0, powerupMaterials.Count)]);
                    foreach (var activeTileGameObject in allActiveTiles)
                    {
                        var tilePair = activeTileGameObject.GetComponent<Tile>();
                        if (tilePair != null && tilePair.GetEmojiName().Equals(originalEmojiName))
                        {
                            tilePair.SetTilePowerup(powerupMaterials[Random.Range(0, powerupMaterials.Count)]);
                        }
                    }
                }
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
        return GetTilesByPanel(panel, true);
    }

    private List<GameObject> GetTilesByPanel(GameObject panel, bool activeTilesOnly)
    {
        var tiles = new List<GameObject>();
        for (var i = 0; i < panel.transform.childCount; i++)
        {
            var child = panel.transform.GetChild(i);
            if (child.CompareTag("Tile"))
            {
                if (activeTilesOnly && child.gameObject.activeSelf)
                {
                    tiles.Add(child.gameObject);
                }
                else if (!activeTilesOnly)
                {
                    tiles.Add(child.gameObject);
                }
            }
        }
        return tiles;
    }

    private void EnableAllTilesInActivePanels()
    {
        var panels = GetActivePanels(_mainCube4x4Component.gameObject);
        foreach (var panel in panels)
        {
            var tiles = GetTilesByPanel(panel, false);
            foreach (var tile in tiles)
            {
                tile.SetActive(true);
            }
        }
    }

    private IEnumerator ResetTiles(List<Tile> tiles)
    {
        yield return new WaitForSeconds(1f);
        var safeList = new List<Tile>();
        safeList.AddRange(tiles);
        foreach (var tile in safeList)
        {
            tile.HideTile();
            _flippedTiles.Remove(tile);
        }
    }

    private IEnumerator ExplodeTiles(List<Tile> tiles, bool shakeCamera = true)
    {
        yield return new WaitForSeconds(0.5f);
        var safeList = new List<Tile>();
        safeList.AddRange(tiles);
        foreach (var tile in safeList)
        {
            tile.ExplodeTile();
            _flippedTiles.Remove(tile);
        }
        _tilesOnBoard = _tilesOnBoard - tiles.Count;
        if (shakeCamera)
        {
            // ShakeCamera();
        }

        if (_tilesOnBoard <= 0)
        {
            if (isZenMode) StartCoroutine(RestartZen());
            else StartCoroutine(IncreaseLevel());
        }
    }

    private IEnumerator RestartZen()
    {
        yield return new WaitForSeconds(1.2f);
        if (_is2D) Play2D(1);
        else Play3D(5);
    }

    private IEnumerator IncreaseLevel()
    {
        yield return new WaitForSeconds(1.2f);
        _activeLevel++;
        if (_is2D)
        {
            if (_activeLevel == 4)
            {
                _is2D = false;
                _activeLevel = 1;
                _mainCube4x4Component.gameObject.SetActive(false);
                StoryManager.Instance.GoToStoryPanel(6);
            }
            else
            {
                Play2D(_activeLevel);
            }
        }
        else // 3D
        {
            if (_activeLevel == 6)
            {
                FinishGame();
            }
            else
            {
                Play3D(_activeLevel);
            }
        }
    }

    private void FinishGame()
    {
        _mainCube4x4Component.gameObject.SetActive(false);
        _cameraMovementComponent.Play2D();
        CanvasManager.Instance.Hide3DControls();
        CanvasManager.Instance.HidePersistentLevelInstructionsText();
        StoryManager.Instance.GoToStoryPanel(9);
        PlayerPrefs.SetInt("IsGameFinished", 1);
    }

    private void ActivatePowerup(Utilities.PowerupEnum powerupEnum)
    {
        switch (powerupEnum)
        {
            case Utilities.PowerupEnum.match:
                _powerupMatchActive = true;
                break;
            case Utilities.PowerupEnum.multiselect:
                _powerupMultiselectActive = true;
                break;
            default:
                break;
        }
        CanvasManager.Instance.ShowPowerupIcon(powerupEnum, true);
        PowerupInitialize(powerupEnum);
    }

    private void DeactivatePowerup(Utilities.PowerupEnum powerupEnum)
    {
        switch (powerupEnum)
        {
            case Utilities.PowerupEnum.match:
                if (!_powerupMultiselectActive || _powerupMultiselectTilesAvailable == 0)
                {
                    _powerupMatchActive = false;
                    CanvasManager.Instance.ShowPowerupIcon(powerupEnum, false);
                }
                break;
            case Utilities.PowerupEnum.multiselect:
                _powerupMultiselectActive = false;
                _powerupMultiselectTilesAvailable = 0;
                CanvasManager.Instance.ShowPowerupIcon(powerupEnum, false);
                break;
            default:
                break;
        }
    }

    private void PowerupInitialize(Utilities.PowerupEnum powerupEnum)
    {
        switch (powerupEnum)
        {
            case Utilities.PowerupEnum.match:
                if (_flippedTiles.Count > 0)
                {
                    AutomatchFlippedTiles(_flippedTiles);
                    DeactivatePowerup(powerupEnum);
                }
                break;
            case Utilities.PowerupEnum.multiselect:
                _powerupMultiselectTilesAvailable += powerupMultiselectDefaultTiles;
                CanvasManager.Instance.UpdateMultiselectTilesAvailableText(_powerupMultiselectTilesAvailable);
                break;
            default:
                break;
        }
    }

    private void AutomatchFlippedTiles(List<Tile> flippedTiles)
    {
        var tilesToExplode = new List<Tile>();
        tilesToExplode.AddRange(flippedTiles);

        List<GameObject> allActiveTilesGameObjects = GetAllActiveTiles();
        foreach (var flippedTile in flippedTiles)
        {
            foreach (var activeTileGameObject in allActiveTilesGameObjects)
            {
                var tile = activeTileGameObject.GetComponent<Tile>();
                if (tile != null && !tile.isShown)
                {
                    if (TilesMatch(tile, flippedTile))
                    {
                        tile.ShowTile();
                        tilesToExplode.Add(tile);
                    }
                }
            }
        }
        StartCoroutine(ExplodeTiles(tilesToExplode));
    }

    private void EliminateMatchedFlippedTiles(List<Tile> flippedTiles)
    {
        var tilesToExplode = new List<Tile>();
        for (var i = 0; i < flippedTiles.Count; i++)
        {
            for (var j = i + 1; j < flippedTiles.Count; j++)
            {
                if (TilesMatch(flippedTiles[i], flippedTiles[j]))
                {
                    tilesToExplode.Add(flippedTiles[i]);
                    tilesToExplode.Add(flippedTiles[j]);
                }
            }
        }
        if (tilesToExplode.Count > 0)
        {
            StartCoroutine(ExplodeTiles(tilesToExplode));
        }
    }

    private bool TilesMatch(Tile tile1, Tile tile2)
    {
        if (tile1.GetEmojiName().Equals(tile2.GetEmojiName()) &&
            tile1.tileIndex != tile2.tileIndex &&
            !tile1.IsPowerup() &&
            !tile2.IsPowerup())
        {
            return true;
        }
        return false;
    }

    private void DecrementMultiselectTilesAvailable()
    {
        if (_powerupMultiselectActive)
        {
            _powerupMultiselectTilesAvailable--;
            CanvasManager.Instance.UpdateMultiselectTilesAvailableText(_powerupMultiselectTilesAvailable);
        }
    }

    public void TileClicked(Tile tile)
    {
        if (isOverlayInstructionsActive || isStoryMode) return;

        DecrementMultiselectTilesAvailable();
        if (_powerupMatchActive)
        {
            tile.ShowTile();
            if (tile.IsPowerup())
            {
                ActivatePowerup(tile.powerup);
                StartCoroutine(ExplodeTiles(new List<Tile>{tile}, false));
            }
            else
            {
                // TODO: HandleAutomatch
                _flippedTiles.Add(tile);
                AutomatchFlippedTiles(_flippedTiles);
                DeactivatePowerup(Utilities.PowerupEnum.match);
            }
        }
        else if (_powerupMultiselectActive)
        {
            tile.ShowTile();
            if (tile.IsPowerup())
            {
                ActivatePowerup(tile.powerup);
                StartCoroutine(ExplodeTiles(new List<Tile>{tile}, false));
            }
            else
            {
                // TODO: HandleMultiselect
                _flippedTiles.Add(tile);
                if (_flippedTiles.Count > 1)
                {
                    EliminateMatchedFlippedTiles(_flippedTiles);
                }
            }
        }
        else
        {
            // Normal gameplay
            if (_flippedTiles.Count < maxFlippedTilesAllowed)
            {
                tile.ShowTile();
                if (tile.IsPowerup())
                {
                    ActivatePowerup(tile.powerup);
                    StartCoroutine(ExplodeTiles(new List<Tile>{tile}, false));
                }
                else
                {
                    _flippedTiles.Add(tile);

                    if (_flippedTiles.Count == maxFlippedTilesAllowed)
                    {
                        if (TilesMatch(_flippedTiles[0], _flippedTiles[1]))
                        {
                            StartCoroutine(ExplodeTiles(new List<Tile>{_flippedTiles[0], _flippedTiles[1]}));
                        }
                        else
                        {
                            StartCoroutine(ResetTiles(new List<Tile>{_flippedTiles[0], _flippedTiles[1]}));
                        }
                    }
                }
            }
        }
        if (_powerupMultiselectTilesAvailable == 0 && _powerupMultiselectActive)
        {
            if (!_powerupMatchActive)
            {
                StartCoroutine(ResetTiles(_flippedTiles));
            }
            DeactivatePowerup(Utilities.PowerupEnum.multiselect);
        }
    }

    public void Play3D(int level = 1)
    {
        isStoryMode = false;
        _is2D = false;
        _activeLevel = level;
        if (_mainCube4x4Component != null)
        {
            _mainCube4x4Component.gameObject.SetActive(true);
            _mainCube4x4Component.Play3D();
        }

        if (_cameraMovementComponent != null)
        {
            _cameraMovementComponent.Play3D();
        }

        CanvasManager.Instance.Play3D(_activeLevel);

        if (level > 1)
        {
            EnableRandomNumberOfPanels(level + 1);
        }
        else
        {
            PopulateActiveTiles();
        }
    }

    public void Play2D(int level = 1)
    {
        isStoryMode = false;
        _is2D = true;
        _activeLevel = level;
        if (_mainCube4x4Component != null)
        {
            _mainCube4x4Component.gameObject.SetActive(true);
            _mainCube4x4Component.Play2D();
        }

        if (_cameraMovementComponent != null)
        {
            _cameraMovementComponent.Play2D();
        }

        CanvasManager.Instance.Play2D(_activeLevel);

        EnableRandomNumberOfPanels(1);
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

    public int GetActiveLevel()
    {
        return _activeLevel;
    }

    // Method name is used in UI buttons (1-6) Do not rename.
    public void EnableRandomNumberOfPanels(int numberOfPanelsToEnable)
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

    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // TESTING
    public void ShowAllTiles()
    {
        var tiles = GetAllActiveTiles();
        foreach (var tileGameObject in tiles)
        {
            var tile = tileGameObject.GetComponent<Tile>();
            if (tile != null)
            {
                tile.ShowTile();
            }
        }
    }

    // TESTING
    public void HideAllTiles()
    {
        var tiles = GetAllActiveTiles();
        foreach (var tileGameObject in tiles)
        {
            var tile = tileGameObject.GetComponent<Tile>();
            if (tile != null)
            {
                tile.HideTile();
            }
        }
    }

    // TESTING
    public void BlowAllTiles()
    {
        var tileGameObjects = GetAllActiveTiles();
        var tiles = new List<Tile>();
        foreach (var tileGameObject in tileGameObjects)
        {
            var tile = tileGameObject.GetComponent<Tile>();
            if (tile != null)
            {
                tiles.Add(tile);
            }
        }
        StartCoroutine(ExplodeTiles(tiles));
    }
}
