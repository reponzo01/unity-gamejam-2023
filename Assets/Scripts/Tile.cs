using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    private Quaternion _originalTileRotation;
    private MeshRenderer _tileMeshRenderer;
    private MeshRenderer _emojiMeshRenderer;
    private string _tileMaterialName;
    private string _emojiGameObjectName = "Emoji";
    private string _explosionGameObjectName = "Explosion";
    private bool _isExploding;
    private bool _rotating = false;

    public Utilities.PowerupEnum powerup = Utilities.PowerupEnum.none;
    public bool isShown;
    public int tileIndex;

    // Start is called before the first frame update
    private void Start()
    {
        isShown = false;
        _originalTileRotation = transform.localRotation;
        _tileMeshRenderer = transform.GetComponent<MeshRenderer>();
        _emojiMeshRenderer = transform.Find(_emojiGameObjectName).GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    private void Update()
    {
        // 0 = facing front. 180 = facing back.
        if (_rotating)
        {
            if (isShown)
            {
                if (transform.localRotation.eulerAngles.y > 0)
                {
                    transform.localRotation =
                    Quaternion.RotateTowards(transform.localRotation, Quaternion.Euler(transform.localRotation.eulerAngles.x, 0, transform.localRotation.eulerAngles.z), 600f * Time.deltaTime);
                }
                else
                {
                    _rotating = false;
                }
            }

            if (!isShown)
            {
                if (transform.localRotation.eulerAngles.y < 180)
                {
                    transform.localRotation =
                    Quaternion.RotateTowards(transform.localRotation, _originalTileRotation, 600f * Time.deltaTime);
                }
                else
                {
                    _rotating = false;
                }
            }
        }

    }

    private void OnMouseDown()
    {
        if (!_isExploding)
        {
            if (!isShown)
            {
                GameManager.Instance.TileClicked(this);
            }
        }
    }

    private IEnumerator DisableTile()
    {
        yield return new WaitForSeconds(1.1f);
        _isExploding = false;
        transform.localRotation = _originalTileRotation;
        gameObject.SetActive(false);
        if (_emojiMeshRenderer != null) _emojiMeshRenderer.enabled = true;
        if (_tileMeshRenderer != null) _tileMeshRenderer.enabled = true;
    }

    private void SetTileMaterial(Material mat)
    {
        var renderer = transform.Find(_emojiGameObjectName).GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material = mat;
            _tileMaterialName = mat.name.Replace("(Instance)", "");
        }
    }

    public string GetEmojiName()
    {
        return _tileMaterialName;
    }

    public void SetTileEmoji(Material emojiMat)
    {
        SetTileMaterial(emojiMat);
    }

    public void SetTilePowerup(Material powerupMat)
    {
        SetTileMaterial(powerupMat);
        // TODO: Not the prettiest way to do this
        if (_tileMaterialName.Equals("PowerupBomb"))
        {
            powerup = Utilities.PowerupEnum.bomb;
        }
        else if (_tileMaterialName.Equals("PowerupMatch"))
        {
            powerup = Utilities.PowerupEnum.match;
        }
        else if (_tileMaterialName.Equals("PowerupMultiselect"))
        {
            powerup = Utilities.PowerupEnum.multiselect;
        }
    }

    public void ShowTile()
    {
        isShown = true;
        _rotating = true;
    }

    public void HideTile()
    {
        isShown = false;
        _rotating = true;
    }

    public void ExplodeTile()
    {
        _isExploding = true;
        var explosion = transform.Find(_explosionGameObjectName).GetComponent<ParticleSystem>();
        if (explosion != null)
        {
            explosion.Play();
        }

        if (_emojiMeshRenderer != null) _emojiMeshRenderer.enabled = false;
        if (_tileMeshRenderer != null) _tileMeshRenderer.enabled = false;
        StartCoroutine(DisableTile());
    }

    public void ResetTilePowerup()
    {
        powerup = Utilities.PowerupEnum.none;
    }

    public bool IsPowerup()
    {
        return powerup != Utilities.PowerupEnum.none;
    }
}
