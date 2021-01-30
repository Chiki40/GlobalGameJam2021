using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClueSelectorManager : MonoBehaviour
{
    public Image[] _images;
    private int _totalImagesAdded = 0;

    struct dataToSave
    {
        public string key;
        public string id;
    }

    List<dataToSave> _dataToSave;
    const int MaxClues = 3;

    public List<GameObject> objectsEditor;
    public GameObject _buttonEditor;

    private bool _editorVisible = false;

    private void Start()
    {
        _dataToSave = new List<dataToSave>();
        CerrarEditor();
    }

    public void AddClue(GameObject go)
    {
        if (_totalImagesAdded < MaxClues)
        {
            _images[_totalImagesAdded].sprite = go.GetComponent<Image>().sprite;
            _images[_totalImagesAdded].gameObject.SetActive(true);

            string name = go.name;
            string[] subStr = name.Split('_');

            dataToSave d = new dataToSave();
            d.key = subStr[0];
            d.id = subStr[1];

            _dataToSave.Add(d);
            ++_totalImagesAdded;
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
        _totalImagesAdded = 0;
        _dataToSave.Clear();
        _editorVisible = true;

        for (int i = 0; i < objectsEditor.Count; ++i)
        {
            objectsEditor[i].SetActive(true);
        }

        _buttonEditor.GetComponentInChildren<TextMeshProUGUI>().text = "Cerrar editor";
    }

    public void CerrarEditor()
    {
        _totalImagesAdded = 0;
        _dataToSave.Clear();
        _editorVisible = false;

        for (int i = 0; i < objectsEditor.Count; ++i)
        {
            objectsEditor[i].SetActive(false);
        }

        for (int i = 0; i < _images.Length; ++i)
        {
            _images[i].gameObject.SetActive(false);
        }

        _buttonEditor.GetComponentInChildren<TextMeshProUGUI>().text = "Abrir editor";
    }

    public void OnAceptar()
    {
        Debug.Log("he pulsado en aceptar");
        CerrarEditor();
    }

    public void OnCancel()
    {
        Debug.Log("he pulsado en cancelar");
        CerrarEditor();
    }

}
