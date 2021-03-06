﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public struct ClueZone
{
    public List<int> clueInfo;
    public int[] pos;
}

public class EditorModeController : MonoBehaviour
{
    [SerializeField]
    private ClueSelectorManager _cluesSelector = null;
    [SerializeField]
    private CompartirMapaManager _compartirMapa = null;
    [SerializeField]
    private Button _backToGenerateButton = null;
    [SerializeField]
    private Button _buttonShovel = null;
    [SerializeField]
    private Button _buttonTreasure = null;
    [SerializeField]
    private Button _buttonUndo = null;
    [SerializeField]
    private Button _buttonFinish = null;
    [SerializeField]
    private GameObject _shovelPlacedPrefab = null;
    [SerializeField]
    private GameObject _treasurePlacedPrefab = null;
    [SerializeField]
    private GameObject _cluesPlacedPrefab = null;
    [SerializeField]
    private GameObject _editorUI = null;

    private GameObject _player = null;
    private MapGenerator _mapGenerator = null;
    private PhotoCamera _photoCam = null;

    private List<ClueZone> _clueZones = new List<ClueZone>();
    private int[] _shovelPos = new int[2] { -1, -1 };
    private Vector3 _shovelPosV3 = Vector3.zero;
    private int[] _treasurePos = new int[2] { -1, -1 };
    private bool _shovelPlaced = false;
    private bool _treasurePlaced = false;
    private bool _generatingMap = true;
    private List<GameObject> _placedPrefabs = new List<GameObject>();

    private void OnEnable()
	{
        _player = GameObject.FindGameObjectWithTag("Player");
        _mapGenerator = FindObjectOfType<MapGenerator>();
        _photoCam = FindObjectOfType<PhotoCamera>();
        Reset();
    }

    public void Reset()
    {
        _clueZones.Clear();
        _shovelPos[0] = -1;
        _shovelPos[1] = -1;
        _shovelPosV3 = Vector3.zero;
        _treasurePos[0] = -1;
        _treasurePos[1] = -1;
        _shovelPlaced = false;
        _treasurePlaced = false;
        for (int i = _placedPrefabs.Count - 1; i >= 0; --i)
        {
            Destroy(_placedPrefabs[i]);
        }
        _placedPrefabs.Clear();
        ToggleGenerateMap(true);
    }

    public void ToggleGenerateMap(bool generatingMap)
	{
        _generatingMap = generatingMap;
        _editorUI.SetActive(!generatingMap);
    }

	private void Update()
	{
        DetermineUIState();

        if (Input.GetKeyDown(KeyCode.Escape))
		{
            if (_generatingMap)
            {
                PlayExitMapSound();
                Exit();
            }
            else
            {
                if (_backToGenerateButton != null)
                {
                    PlayBackToGenerateMapSound();
                    _backToGenerateButton.onClick?.Invoke();
                }
            }
		}
	}

    private void DetermineUIState()
	{
        _cluesSelector.ChangeOpenEditorInteraction(_shovelPlaced && !_treasurePlaced && _clueZones.Count < GameManager.kMaxCluesZones);
        _buttonShovel.interactable = !_shovelPlaced;
        _buttonTreasure.interactable = _shovelPlaced && !_treasurePlaced;
        _buttonFinish.interactable = _clueZones.Count > 0 && _shovelPlaced && _treasurePlaced;
        _buttonUndo.interactable = _shovelPlaced;
    }

    private bool CanPlaceObject(Vector2Int pos)
	{
        if (_shovelPlaced && _shovelPos[0] == pos.x && _shovelPos[1] == pos.y && _clueZones.Count > 0)
		{
            return false;
		}
       //if (_treasurePlaced && _treasurePos[0] == pos.x && _treasurePos[1] == pos.y)
       //{
       //    return false;
       //}
        for (int i = 0; i < _clueZones.Count; ++i)
        {
            if (_clueZones[i].pos[0] == pos.x && _clueZones[i].pos[1] == pos.y)
            {
                return false;
            }
        }
        return true;
	}

    public void NewClueZoneAdded(List<int> clueInfo)
    {
        if (_clueZones.Count <= GameManager.kMaxCluesZones)
        {
            Vector2Int gridPos = new Vector2Int();
            if (_mapGenerator.GetGridPos(_player.transform.position, ref gridPos))
            {
                if (!CanPlaceObject(gridPos))
                {
                    Debug.LogError("[EditorModeController.NewClueZoneAdded] ERROR. Trying to place clue in an already used cell");
                    return;
                }

                ClueZone clueZone = new ClueZone();
                clueZone.clueInfo = new List<int>(clueInfo);
                clueZone.pos = new int[2] { gridPos.x, gridPos.y };
                if (Physics.Raycast(_player.transform.position, Vector3.down, out RaycastHit hitInfo) && !_treasurePlaced)
                {
                    _placedPrefabs.Add(Instantiate(_cluesPlacedPrefab, hitInfo.point + new Vector3(0.0f, 0.02f, 0.0f), Quaternion.Euler(270.0f, 0.0f, 0.0f)));
                }
                _clueZones.Add(clueZone);
                PlayPlaceClueSound();
            }
        }
        else
		{
            Debug.LogError("[EditorModeController.NewClueZoneAdded] ERROR. Tried to add a clue but " + GameManager.kMaxCluesZones + " clues have already been placed");
		}
    }

    /// <summary>
    /// Aqui entramos cuando hemos pulsado en cancelar una pista visual
    /// </summary>
    public void LastClueZoneCancelled()
    {
       //si era una pista standard, no pasa nada
       //si era la pista del tesoro => lo elimino y digo que no hay tesoro puesto

        if(_treasurePlaced)
        {
            _treasurePlaced = false;
            //elimino el cofre
            Destroy(_placedPrefabs[_placedPrefabs.Count - 1]);
            _placedPrefabs.RemoveAt(_placedPrefabs.Count - 1);
        }

    }

    public void OnAddShovelClicked()
    {
        if (!_shovelPlaced)
        {
            Vector2Int gridPos = new Vector2Int();
            if (_mapGenerator.GetGridPos(_player.transform.position, ref gridPos))
            {
                if (!CanPlaceObject(gridPos))
                {
                    Debug.LogError("[EditorModeController.OnAddShovelClicked] ERROR. Trying to place shovel in an already used cell");
                    return;
                }

                //_cluesSelector.AbrirEditor();

                _shovelPos[0] = gridPos.x;
                _shovelPos[1] = gridPos.y;
                _shovelPosV3 = _player.transform.position;
                Quaternion qt = Quaternion.Euler(0,90,0);
                if (Physics.Raycast(_shovelPosV3, Vector3.down, out RaycastHit hitInfo))
                {
                    _placedPrefabs.Add(Instantiate(_shovelPlacedPrefab, hitInfo.point + new Vector3(0.0f, 1.19f, 0.0f), qt));
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
                if (!CanPlaceObject(gridPos))
                {
                    Debug.LogError("[EditorModeController.OnAddTreasureClicked] ERROR. Trying to place treasure in an already used cell");
                    return;
                }
                _treasurePos[0] = gridPos.x;
                _treasurePos[1] = gridPos.y;
                if (Physics.Raycast(_player.transform.position, Vector3.down, out RaycastHit hitInfo))
                {
                    _placedPrefabs.Add(Instantiate(_treasurePlacedPrefab, hitInfo.point + new Vector3(0.0f, 0.02f, 0.0f), Quaternion.identity));
                }
                _treasurePlaced = true;
                _cluesSelector.AbrirEditor();

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
        _compartirMapa.ShowCompartirMapa(toTexture2D(rT), _mapGenerator.mapData, OnTweetPosted);
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

    public void OnUndo()
    {
        Destroy(_placedPrefabs[_placedPrefabs.Count - 1]);
        _placedPrefabs.RemoveAt(_placedPrefabs.Count - 1);
        if (_clueZones.Count == 0)
        {
            _shovelPlaced = false;
        }
        else
        {
            _clueZones.RemoveAt(_clueZones.Count - 1);
            if (_treasurePlaced)
            {
                _treasurePlaced = false;
            }
        }
    }

    public void PlayAddTreasureSound()
	{
        UtilSound.instance.PlaySound("BODY_Sniff_Long_mono");
    }

    public void PlayAddShovelSound()
    {
        UtilSound.instance.PlaySound("BUTTON_Medium_Click_mono");
    }

    public void PlayOpenClueSound()
    {
        UtilSound.instance.PlaySound("PAPER_Shake_01_mono");
    }

    public void PlayCloseClueSound()
    {
        UtilSound.instance.PlaySound("BUTTON_Click_Compressor_Small_02_stereo");
    }

    public void PlayUndoSound()
    {
        UtilSound.instance.PlaySound("UI_Click_Tap_Noisy_Long_mono");
    }

    public void PlayFinishSound()
    {
        UtilSound.instance.PlaySound("COINS_Rattle_01_mono");
    }

    private void PlayPlaceClueSound()
	{
        UtilSound.instance.PlaySound("PAPER_Crumble_01_mono");
    }


    public void PlayCancelShareSound()
    {
        UtilSound.instance.PlaySound("UI_Click_Tap_Noisy_Subtle_mono");
    }

    public void PlayConfirmShareSound()
    {
        UtilSound.instance.PlaySound("BUTTON_Light_Switch_02_stereo");
    }

    public void PlayGenerateMapSound()
    {
        UtilSound.instance.PlaySound("UI_Click_Aftertap_mono");
    }

    public void PlayExitMapSound()
    {
        UtilSound.instance.PlaySound("BUTTON_Click_Compressor_stereo");
    }

    public void PlayStartEditingMapSound()
    {
        UtilSound.instance.PlaySound("FABRIC_Movement_Fast_01_mono");
    }

    public void PlayBackToGenerateMapSound()
    {
        UtilSound.instance.PlaySound("FABRIC_Movement_Fast_02_mono");
    }

    public void PlayClueSymbolSound()
    {
        UtilSound.instance.PlaySound("BUTTON_Short_mono");
    }
}
