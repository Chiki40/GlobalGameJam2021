using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class EditorModeController : MonoBehaviour
{
    public struct ClueZone
	{
        public List<int> clueInfo;
        public int[] pos;
	}

    private const int kMaxCluesZones = 3;

    [SerializeField]
    private ClueSelectorManager _cluesSelector = null;
    [SerializeField]
    private CompartirMapaManager _compartirMapa = null;
    [SerializeField]
    private Button _buttonShovel = null;
    [SerializeField]
    private Button _buttonTreasure = null;
    [SerializeField]
    private Button _buttonFinish = null;
    [SerializeField]
    private GameObject _shovelPlacedPrefab = null;
    [SerializeField]
    private GameObject _treasurePlacedPrefab = null;
    [SerializeField]
    private GameObject _cluesPlacedPrefab = null;

    private GameObject _player = null;
    private MapGenerator _mapGenerator = null;
    private PhotoCamera _photoCam = null;

    private List<ClueZone> _clueZones = new List<ClueZone>();
    private int[] _shovelPos = new int[2] { -1, -1 };
    private Vector3 _shovelPosV3 = Vector3.zero;
    private int[] _treasurePos = new int[2] { -1, -1 };
    private bool _shovelPlaced = false;
    private bool _treasurePlaced = false;

    private void OnEnable()
	{
        _clueZones.Clear();
        _shovelPos[0] = -1;
        _shovelPos[1] = -1;
        _shovelPosV3 = Vector3.zero;
        _treasurePos[0] = -1;
        _treasurePos[1] = -1;
        _shovelPlaced = false;
        _treasurePlaced = false;
        _player = GameObject.FindGameObjectWithTag("Player");
        _mapGenerator = FindObjectOfType<MapGenerator>();
        _photoCam = FindObjectOfType<PhotoCamera>();
    }

	private void Update()
	{
        DetermineUIState();
	}

    private void DetermineUIState()
	{
        _cluesSelector.ChangeOpenEditorInteraction(_clueZones.Count < kMaxCluesZones);
        _buttonShovel.interactable = !_shovelPlaced;
        _buttonTreasure.interactable = !_treasurePlaced;
        _buttonFinish.interactable = _clueZones.Count > 0 && _shovelPlaced && _treasurePlaced;
    }

    public void NewClueZoneAdded(List<int> clueInfo)
    {
        if (_clueZones.Count < kMaxCluesZones)
        {
            Vector2Int gridPos = new Vector2Int();
            if (_mapGenerator.GetGridPos(_player.transform.position, ref gridPos))
            {
                ClueZone clueZone = new ClueZone();
                clueZone.clueInfo = clueInfo;
                clueZone.pos = new int[2] { gridPos.x, gridPos.y };
                if (Physics.Raycast(_player.transform.position, Vector3.down, out RaycastHit hitInfo))
                {
                    Instantiate(_cluesPlacedPrefab, hitInfo.point + new Vector3(0.0f, 0.02f, 0.0f), Quaternion.Euler(270.0f, 0.0f, 0.0f));
                }
                _clueZones.Add(clueZone);
            }
        }
        else
		{
            Debug.LogError("[EditorModeController.NewClueZoneAdded] ERROR. Tried to add a clue but " + kMaxCluesZones + " clues have already been placed");
		}
    }

    public void OnAddShovelClicked()
    {
        if (!_shovelPlaced)
        {
            Vector2Int gridPos = new Vector2Int();
            if (_mapGenerator.GetGridPos(_player.transform.position, ref gridPos))
            {
                _shovelPos[0] = gridPos.x;
                _shovelPos[1] = gridPos.y;
                _shovelPosV3 = _player.transform.position;
                if (Physics.Raycast(_shovelPosV3, Vector3.down, out RaycastHit hitInfo))
                {
                    Instantiate(_shovelPlacedPrefab, hitInfo.point + new Vector3(0.0f, 0.02f, 0.0f), Quaternion.Euler(270.0f, 0.0f, 0.0f));
                }
                _shovelPlaced = true;
            }
        }
        else
        {
            Debug.LogError("[EditorModeController.OnAddShovelClicked] ERROR. Tried to add shovel but it has been already added");
        }
    }

    public void OnAddTreasureClicked()
    {
        if (!_treasurePlaced)
        {
            Vector2Int gridPos = new Vector2Int();
            if (_mapGenerator.GetGridPos(_player.transform.position, ref gridPos))
            {
                _treasurePos[0] = gridPos.x;
                _treasurePos[1] = gridPos.y;
                if (Physics.Raycast(_player.transform.position, Vector3.down, out RaycastHit hitInfo))
                {
                    Instantiate(_treasurePlacedPrefab, hitInfo.point + new Vector3(0.0f, 0.02f, 0.0f), Quaternion.Euler(270.0f, 0.0f, 0.0f));
                }
                _treasurePlaced = true;
            }
        }
        else
        {
            Debug.LogError("[EditorModeController.OnAddTreasureClicked] ERROR. Tried to add treasure but it has been already added");
        }
    }

    public void OnFinishClicked()
    {
        void OnTweetPosted()
        {
            StartCoroutine(ExitCoroutine());
        }

        _mapGenerator.mapData.hintsGridPos = new MapData.HintData[_clueZones.Count];
        for (int i = 0; i < _clueZones.Count; ++i)
		{
            _mapGenerator.mapData.hintsGridPos[i].gridPos = new Vector2Int(_clueZones[i].pos[0], _clueZones[i].pos[1]);
            _mapGenerator.mapData.hintsGridPos[i].symbols = _clueZones[i].clueInfo.ToArray();
            _mapGenerator.mapData.shovelGridPos.x = _shovelPos[0];
            _mapGenerator.mapData.shovelGridPos.y = _shovelPos[1];
            _mapGenerator.mapData.tresureGridPos.x = _treasurePos[0];
            _mapGenerator.mapData.tresureGridPos.y = _treasurePos[1];
        }
        RenderTexture rT = _photoCam.TakePictureOfArea(_shovelPosV3);
        _compartirMapa.ShowCompartirMapa(toTexture2D(rT), Serializator.XmlSerialize<MapData>(_mapGenerator.mapData), OnTweetPosted);
    }

    private IEnumerator ExitCoroutine()
	{
        yield return new WaitForSeconds(2.0f);
        Exit();

    }

    public void Exit()
	{
        SceneManager.LoadScene("MainMenu");
    }

    private Texture2D toTexture2D(RenderTexture rTex)
    {
        Texture2D tex = new Texture2D(rTex.width, rTex.height, TextureFormat.RGB24, false);
        RenderTexture.active = rTex;
        tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
        tex.Apply();
        return tex;
    }
}
