using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class ClueViewerManager : MonoBehaviour
{
    public Image[] _images;
    public Texture2D _spriteSheet;

    public void OnClose()
    {
        this.gameObject.SetActive(false);
        for(int i = 0; i < _images.Length; ++i)
        {
            _images[i].gameObject.SetActive(false);
        }
    }

    public void Start()
    {
        OnClose();
    }

    public void Show(List<int> ids)
    {
        this.gameObject.SetActive(true);

        string spriteSheet = AssetDatabase.GetAssetPath(_spriteSheet);
        Sprite[] sprites = AssetDatabase.LoadAllAssetsAtPath(spriteSheet).OfType<Sprite>().ToArray();

        //populate
        for (int i = 0; i < ids.Count;++i)
        {
            _images[i].sprite = sprites[ids[i]];
            _images[i].gameObject.SetActive(true);
        }
    }
}
