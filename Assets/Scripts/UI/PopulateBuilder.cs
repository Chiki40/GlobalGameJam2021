using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEditor.Events;
using System;

public class PopulateBuilder : MonoBehaviour
{
    [System.Serializable]
    public struct SymbolsInfo
    {
        public string _BaseName;
        public Texture2D _image;
        int _ancho;
        int _alto;
    }
    public GameObject _parent;
    public ClueSelectorManager _clueManager;
    public List<SymbolsInfo> _symbols;

    public void Populate()
    {
        Remove();
        for (int symbolsIdx = 0; symbolsIdx < _symbols.Count; ++ symbolsIdx)
        {
            Populate(_symbols[symbolsIdx]);
        }
    }

    private void Populate(SymbolsInfo info)
    {
        string spriteSheet = AssetDatabase.GetAssetPath(info._image);
        Sprite[] sprites = AssetDatabase.LoadAllAssetsAtPath(spriteSheet).OfType<Sprite>().ToArray();

        for(int spriteIdx = 0; spriteIdx < sprites.Length; ++spriteIdx)
        {
            Sprite sp = sprites[spriteIdx];
            string name = info._BaseName + "_" + spriteIdx.ToString();
            GameObject go = new GameObject(name);
            Image mg = go.AddComponent<Image>();
            mg.sprite = sp;
            Button b = go.AddComponent<Button>();

            UnityAction<GameObject> callback = new UnityAction<GameObject>(_clueManager.AddClue);
            UnityEventTools.AddObjectPersistentListener<GameObject>(b.onClick, callback, go);

            go.transform.SetParent(_parent.transform);
        }
    }

    public void Remove()
    {
        List<GameObject> listToDestroy = new List<GameObject>();
       
        for(int i = 0; i< _parent.transform.childCount; ++i)
        {
            listToDestroy.Add(_parent.transform.GetChild(i).gameObject);
        }

        for(int i = 0; i< listToDestroy.Count; ++i)
        {
            DestroyImmediate(listToDestroy[i]);
        }
    }
}
