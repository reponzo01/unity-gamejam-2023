using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    private string _emojiName;
    private bool _isShown;
    private bool _isExploding;
    private Quaternion _originalTileRotation;
    private string _emojiGameObjectName = "Emoji";
    private string _explosionGameObjectName = "Explosion";

    // Start is called before the first frame update
    private void Start()
    {
        _isShown = false;
        _originalTileRotation = transform.localRotation;
    }

    // Update is called once per frame
    private void Update()
    {
        // 0 = facing front. 180 = facing back.
        if (_isShown && transform.localRotation.eulerAngles.y > 0)
        {
            transform.localRotation =
                Quaternion.RotateTowards(transform.localRotation, Quaternion.Euler(transform.localRotation.eulerAngles.x, 0, transform.localRotation.eulerAngles.z), 600f * Time.deltaTime);
        }

        if (!_isShown && transform.localRotation.eulerAngles.y < 180)
        {
            transform.localRotation =
                Quaternion.RotateTowards(transform.localRotation, _originalTileRotation, 600f * Time.deltaTime);
        }
    }

    private void OnMouseDown()
    {
        if (!_isExploding)
        {
            if (!_isShown)
            {
                GameManager.Instance.TileClicked(this);
            }
        }
    }

    private IEnumerator DisableTile()
    {
        yield return new WaitForSeconds(1.1f);
        gameObject.SetActive(false);
    }

    public string GetEmojiName()
    {
        return _emojiName;
    }

    public void SetEmojiMaterial(Material emojiMat)
    {
        var renderer = transform.Find(_emojiGameObjectName).GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material = emojiMat;
            _emojiName = emojiMat.name.Replace("(Instance)", "");
        }
    }

    public void ShowTile()
    {
        _isShown = true;
    }

    public void HideTile()
    {
        _isShown = false;
    }

    public void ExplodeTile()
    {
        _isExploding = true;
        var tileMeshRenderer = transform.GetComponent<MeshRenderer>();
        var emojiMeshRenderer = transform.Find(_emojiGameObjectName).GetComponent<MeshRenderer>();
        var explosion = transform.Find(_explosionGameObjectName).GetComponent<ParticleSystem>();
        if (explosion != null)
        {
            explosion.Play();
        }

        if (emojiMeshRenderer != null) emojiMeshRenderer.enabled = false;
        if (tileMeshRenderer != null) tileMeshRenderer.enabled = false;
        StartCoroutine(DisableTile());
    }
}
