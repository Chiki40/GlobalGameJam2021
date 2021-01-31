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
	private Button _showPictureButton = null;
	[SerializeField]
	private GameObject _photo = null;
	[SerializeField]
	private ClueViewerManager _cluesViewer = null;
	[SerializeField]
	private VerTesoroManager _verTesoroManager = null;
	[SerializeField]
	private GameObject _cluesPlacedPrefab = null;

	private List<GameObject> _shovelUsesObjects = new List<GameObject>();
	private int _shovelUsesRemaining = 0;
	private List<ClueZone> _foundClueZones = new List<ClueZone>();
	private bool _shovelFound = false;
	private bool _treasureFound = false;

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
		_treasureFound = false;
		// Set Photo Sprite
		_photo.transform.GetComponentInChildren<Image>().sprite = GameManager.SpritePhoto;
		ShowPicture(false);
	}

	private void Update()
	{
		if (!_treasureFound && Input.GetKeyDown(KeyCode.R))
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
					// Show first hint
					ClueZone clueZone = new ClueZone();
					clueZone.clueInfo = new List<int>(hintData[0].symbols);
					clueZone.pos = new int[2] { cellPos.x, cellPos.y };
					// Add new clue
					_foundClueZones.Add(clueZone);
					_cluesViewer.Show(clueZone.clueInfo);

					PlayOpenClueSound();
					ShowPicture(false);

					if (_cluesPlacedPrefab != null)
					{
						if (Physics.Raycast(pos, Vector3.down, out RaycastHit hitInfo))
						{
							Instantiate(_cluesPlacedPrefab, hitInfo.point + new Vector3(0.0f, 0.02f, 0.0f), Quaternion.Euler(270.0f, 0.0f, 0.0f));
						}
					}
				}
			}
			else
			{
				// Did we already find every hint?
				if (_foundClueZones.Count < hintData.Length)
				{
					// Check every hint in MapData (except the first one, which is shown when the shovel is found)
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
								clueZone.clueInfo = new List<int>(hintData[i + 1].symbols);
								clueZone.pos = new int[2] { cellPos.x, cellPos.y };

								// Add new clue
								_foundClueZones.Add(clueZone);
								_cluesViewer.Show(clueZone.clueInfo);
								Debug.Log("Clue found!");

								if (_cluesPlacedPrefab != null)
								{
									if (Physics.Raycast(pos, Vector3.down, out RaycastHit hitInfo))
									{
										Instantiate(_cluesPlacedPrefab, hitInfo.point + new Vector3(0.0f, 0.02f, 0.0f), Quaternion.Euler(270.0f, 0.0f, 0.0f));
									}
								}

								return;
							}
						}
					}
				}

				if (animationManager != null)
				{
					animationManager.PerformDig();
				}
				Vector2Int treasurePos = _mapGenerator.mapData.tresureGridPos;
				// Treasure found here
				if (IsCloseEnough(treasurePos, cellPos))
				{
					_treasureFound = true;
					GameObject treasureObject = _mapGenerator.GetTreasureObject();
					if (treasureObject != null)
					{
						treasureObject.SetActive(true);
					}
					Debug.Log("Treasure found! You won!");
					StartCoroutine(TreasureFound());
					
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

	private IEnumerator TreasureFound()
	{
		yield return new WaitForSeconds(2.0f);
		_verTesoroManager.ShowTesoro(_mapGenerator.mapData.message, GameManager.IdTweet);
	}

	public void Exit()
	{
		SceneManager.LoadScene("MainMenu");
	}

	public void ShowPicture(bool active)
	{
		if(!_shovelFound)
        {
			_photo.SetActive(active);
        }
		else
        {
			_cluesViewer.Show(_foundClueZones[_foundClueZones.Count -1].clueInfo);
		}

	}

	public void PlayConfirmShareSound()
	{
		UtilSound.instance.PlaySound("BUTTON_Light_Switch_02_stereo");
	}

	public void PlayCancelShareSound()
	{
		UtilSound.instance.PlaySound("UI_Click_Tap_Noisy_Subtle_mono");
	}

	public void PlayExitMapSound()
	{
		UtilSound.instance.PlaySound("BUTTON_Click_Compressor_stereo");
	}

	public void PlayShowPhotoSound()
	{
		UtilSound.instance.PlaySound("PAPER_Shake_01_mono");
	}

	public void PlayClosePhotoSound()
	{
		UtilSound.instance.PlaySound("BUTTON_Click_Compressor_Small_02_stereo");
	}

	private void PlayOpenClueSound()
	{
		UtilSound.instance.PlaySound("PAPER_Shake_01_mono");
	}

	public void PlayCloseClueSound()
	{
		UtilSound.instance.PlaySound("BUTTON_Click_Compressor_Small_02_stereo");
	}
}
