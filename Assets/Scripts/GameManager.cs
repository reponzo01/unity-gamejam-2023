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
    private int maxFlippedTilesAllowed = 2;

    private List<Tile> _flippedTiles;
    private int _tilesInPlay = 0;

    private void Awake()
    {
        _instance = this;

        QualitySettings.vSyncCount = 0;  // VSync must be disabled
        Application.targetFrameRate = 60;
    }

    // Start is called before the first frame update
    private void Start()
    {
        _flippedTiles = new List<Tile>();
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

    // Update is called once per frame
    private void Update()
    {

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

    IEnumerator RestartScene()
    {
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    IEnumerator ResetTiles()
    {
        yield return new WaitForSeconds(1f);
        _flippedTiles[0].HideTile();
        _flippedTiles[1].HideTile();
        _flippedTiles.Clear();
    }

    IEnumerator ExplodeTiles()
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
}
