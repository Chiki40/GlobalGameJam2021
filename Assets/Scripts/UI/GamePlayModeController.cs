using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GamePlayModeController : MonoBehaviour
{
	[SerializeField]
	private int _shovelUses = 3;
	[SerializeField]
	private float _rangeOfCellsToInteract = 3;
	[SerializeField]
	private GameObject _shovelUsePrefab = null;
	[SerializeField]
	private Sprite _shovelUseOnSprite = null;
	[SerializeField]
	private Sprite _shovelUseOffSprite = null;
	[SerializeField]
	private Transform _shovelUsesParent = null;
	[SerializeField]
	private ClueViewerManager _cluesViewer = null;

	private List<GameObject> _shovelUsesObjects = new List<GameObject>();
	private int _shovelUsesRemaining = 0;
	private List<ClueZone> _foundClueZones = new List<ClueZone>();
	private bool _shovelFound = false;

	private MapGenerator _mapGenerator = null;
	//private PhotoCamera _photoCam = null;

	private void Start()
	{
		for (int i = 0; i < _shovelUses; ++i)
		{
			GameObject go = Instantiate(_shovelUsePrefab, _shovelUsesParent);
			Image image = go.GetComponent<Image>();
			if (image != null)
			{
				image.sprite = _shovelUseOnSprite;
			}
			_shovelUsesObjects.Add(go);
		}
	}

	private void OnEnable()
	{
		_mapGenerator = FindObjectOfType<MapGenerator>();
		//_photoCam = FindObjectOfType<PhotoCamera>();
		_shovelUsesRemaining = _shovelUses;
		_foundClueZones.Clear();
		_shovelFound = false;
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.R))
		{
			Reset();
		}
	}

	public void OnInteract(Vector3 pos, AnimationManager animationManager)
	{
		Vector2Int cellPos = Vector2Int.zero;
		if (_mapGenerator.GetGridPos(pos, ref cellPos))
		{
			MapData.HintData[] hintData = _mapGenerator.mapData.hintsGridPos;
			// Did we already find every hint?
			if (_foundClueZones.Count < hintData.Length)
			{
				// Check every hint in MapData
				for (int i = 0; i < hintData.Length; ++i)
				{
					// Hint found here
					if (IsCloseEnough(hintData[i].gridPos, cellPos))
					{
						bool alreadyFound = false;
						for (int j = 0; j < _foundClueZones.Count; ++j)
						{
							// This hint was already found, skip
							if (hintData[i].gridPos.x == _foundClueZones[j].pos[0] && hintData[i].gridPos.y == _foundClueZones[j].pos[1])
							{
								alreadyFound = true;
								break;
							}
						}
						if (!alreadyFound)
						{
							ClueZone clueZone = new ClueZone();
							clueZone.clueInfo = new List<int>(hintData[i].symbols);
							clueZone.pos = new int[2] { cellPos.x, cellPos.y };
							// Add new clue
							_foundClueZones.Add(clueZone);
							_cluesViewer.Show(clueZone.clueInfo);
							Debug.Log("Clue found!");
							return;
						}
					}
				}
			}
			if (!_shovelFound)
			{
				Vector2Int shovelPos = _mapGenerator.mapData.shovelGridPos;
				// Shovel found here
				if (IsCloseEnough(shovelPos, cellPos))
				{
					_shovelFound = true;
					GameObject shovelObject = _mapGenerator.GetPalaObject();
					if (shovelObject != null)
					{
						shovelObject.SetActive(false);
					}
					Debug.Log("Shovel found!");
				}
			}
			else
			{
				if (animationManager != null)
				{
					animationManager.PerformDig();
				}
				Vector2Int treasurePos = _mapGenerator.mapData.tresureGridPos;
				// Treasure found here
				if (IsCloseEnough(treasurePos, cellPos))
				{
					_shovelFound = true;
					GameObject treasureObject = _mapGenerator.GetTreasureObject();
					if (treasureObject != null)
					{
						treasureObject.SetActive(true);
					}
					Debug.Log("Treasure found! You won!");
					StartCoroutine(ExitCoroutine());
				}
				else
				{
					Debug.Log("Missed! " + _shovelUsesRemaining + " tries remaining");
					--_shovelUsesRemaining;
					// Modify the sprite
					GameObject go = _shovelUsesObjects[_shovelUsesRemaining];
					Image image = go.GetComponent<Image>();
					if (image != null)
					{
						image.sprite = _shovelUseOffSprite;
					}
					if (_shovelUsesRemaining <= 0)
					{
						Debug.Log("You lost!");
						StartCoroutine(ResetCoroutine());
					}
				}
			}
		}
	}

	private bool IsCloseEnough(Vector2Int cell1, Vector2Int cell2)
	{
		return (cell1 - cell2).magnitude <= _rangeOfCellsToInteract;
	}

	private IEnumerator ResetCoroutine()
	{
		yield return new WaitForSeconds(2.0f);
		Reset();
	}

	private void Reset()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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
}
