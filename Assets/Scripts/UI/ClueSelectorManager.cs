using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClueSelectorManager : MonoBehaviour
{
    [SerializeField]
    private EditorModeController _editorModeController = null;
    [SerializeField]
    private Button _acceptButton = null;

    public Button[] _buttonsClues;
    private int _totalCluesAdded = 0;

    List<int> _clueIds;
    const int MaxClues = 3;

    public List<GameObject> objectsEditor;
    public GameObject _buttonOpen;

    private bool _editorVisible = false;

    private GameObject _player = null;

    private void Start()
    {
        _clueIds = new List<int>();
        CerrarEditor();
    }

	private void Update()
	{
        _acceptButton.interactable = _totalCluesAdded > 0;
    }

	private void OnEnable()
	{
        _player = GameObject.FindGameObjectWithTag("Player");
    }

	public void ChangeOpenEditorInteraction(bool interaction)
	{
        _buttonOpen.GetComponent<Button>().interactable = interaction;
    }

    public void AddClue(GameObject go)
    {
        if (_totalCluesAdded < MaxClues)
        {
            Sprite sp = go.GetComponent<Image>().sprite;
            int id = int.Parse(go.name);

            //activamos el boton
            _buttonsClues[_totalCluesAdded].gameObject.SetActive(true);
            _buttonsClues[_totalCluesAdded].GetComponent<Image>().sprite = sp;

            //guardamos la clave
            _clueIds.Add(id);

            ++_totalCluesAdded;
        }
    }

    public void RemoveClue(int id)
    {
        if(_totalCluesAdded <= 0)
        {
            return;
        }
        if(_totalCluesAdded == 1)
        {
            _clueIds.Clear();
            _buttonsClues[0].gameObject.SetActive(false);
            --_totalCluesAdded;
        }
        else
        {
            int maximo = Mathf.Min(_totalCluesAdded, MaxClues) -1;
            for(int i = id; i < maximo; ++i)
            {
                _buttonsClues[i].gameObject.SetActive(true);
                _buttonsClues[i].GetComponent<Image>().sprite = _buttonsClues[i +1].GetComponent<Image>().sprite;
                _clueIds[i] = _clueIds[i + 1];
            }
            _buttonsClues[maximo].gameObject.SetActive(false);
            _clueIds.RemoveAt(maximo);
            --_totalCluesAdded;
        }
    }   

    public void ShowOrHideEditor()
    {
        if(_editorVisible)
        {
            CerrarEditor();
        }
        else
        {
            AbrirEditor();
        }
    }

    public void AbrirEditor()
    {
        _totalCluesAdded = 0;
        _clueIds.Clear();
        _editorVisible = true;

        for (int i = 0; i < objectsEditor.Count; ++i)
        {
            objectsEditor[i].SetActive(true);
        }

        // Disable player
        _player.GetComponent<CharacterController>().EnableInput(false);

        _buttonOpen.SetActive(false);
    }

    public void CerrarEditor()
    {
        _totalCluesAdded = 0;
        _clueIds.Clear();
        _editorVisible = false;

        for (int i = 0; i < objectsEditor.Count; ++i)
        {
            objectsEditor[i].SetActive(false);
        }

        for (int i = 0; i < _buttonsClues.Length; ++i)
        {
            _buttonsClues[i].gameObject.SetActive(false);
        }

        // Enable player
        _player.GetComponent<CharacterController>().EnableInput(true);

        _buttonOpen.SetActive(true);
    }

    public void OnAceptar()
    {
        // At least 1 clue must be selected
        if (_totalCluesAdded <= 0)
		{
            return;
		}

        Debug.Log("he pulsado en aceptar");
        //en _clueIds tenemos los valores que queremos guardar
        if (_editorModeController != null)
		{
            _editorModeController.NewClueZoneAdded(_clueIds);
        }
        CerrarEditor();
    }

    public void OnCancel()
    {
        Debug.Log("he pulsado en cancelar");

        if (_editorModeController != null)
        {
            _editorModeController.LastClueZoneCancelled();
        }

        CerrarEditor();
    }

}
