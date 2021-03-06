﻿using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class ClueViewerManager : MonoBehaviour
{
    public Image[] _images;
    public Texture2D _spriteSheet;
    private GameObject _player = null;
    public Sprite[] sprites;

    public void OnClose()
    {
        this.gameObject.SetActive(false);
        for(int i = 0; i < _images.Length; ++i)
        {
            _images[i].gameObject.SetActive(false);
        }

        // Enable player
        _player.GetComponent<CharacterController>().EnableInput(true);
    }

    public void Start()
    {
        OnClose();
    }

    private void OnEnable()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
    }

    public void Show(List<int> ids)
    {
        this.gameObject.SetActive(true);

        //populate
        for (int i = 0; i < ids.Count;++i)
        {
            for (int j = 0; j < sprites.Length; ++j)
            {
                if (sprites[j].name == "Simbolos_" + ids[i])
                {
                    _images[i].sprite = sprites[j];
                    break;
                }
            }
            _images[i].gameObject.SetActive(true);
        }

        // Disable player
        _player.GetComponent<CharacterController>().EnableInput(false);
    }
}
