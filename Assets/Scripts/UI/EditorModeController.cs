using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditorModeController : MonoBehaviour
{
    private const int kMaxCluesZones = 3;

    [SerializeField]
    private ClueSelectorManager _cluesSelector = null;
    [SerializeField]
    private Button _buttonShovel = null;
    [SerializeField]
    private Button _buttonTreasure = null;
    [SerializeField]
    private Button _buttonFinish = null;

    private GameObject _player = null;

    private List<List<int>> _clueZones = new List<List<int>>();
    private bool _shovelPlaced = false;
    private bool _treasurePlaced = false;

    private void OnEnable()
	{
        _clueZones.Clear();
        _shovelPlaced = false;
        _treasurePlaced = false;
        _player = GameObject.FindGameObjectWithTag("Player");
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

    public void NewClueZoneAdded(List<int> hintZone)
    {
        if (_clueZones.Count < kMaxCluesZones)
        {
            _clueZones.Add(hintZone);
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
            _shovelPlaced = true;
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
            _treasurePlaced = true;
        }
        else
        {
            Debug.LogError("[EditorModeController.OnAddTreasureClicked] ERROR. Tried to add treasure but it has been already added");
        }
    }

    public void OnFinishClicked()
    {

    }
}
