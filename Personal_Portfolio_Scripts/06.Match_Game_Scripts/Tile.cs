using System.Collections;
using System.Collections.Generic;
using System.Data;

using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    public int tileID;
    public int layer;
    public int row;
    public int col;
    
    public Image tileImage;
    private Button button;
    public Image[] blockLine;

    private void Awake()
    {
        button=GetComponent<Button>();
        button.onClick.AddListener(OnClickTile);
    }
    private void Start()
    {
        SetLayerColor();
    }
    void SetLayerColor()
    {
        Color layerColor;
        switch(layer)
        {
           case 0:
           layerColor=Color.black;
           break;
           case 1:
           layerColor=Color.blue;
           break;
           case 2:
           layerColor=Color.green;
           break;
           default:
           layerColor=Color.black;
           break;
        }

        foreach(var line in blockLine)
        {
            if(line!=null)
            line.color=layerColor;
        }
    }
    void OnClickTile()
    {
       Debug.Log($"클릭 시도! tile.layer={layer}, currentLayer={MatchGameManager.Instance.CurrentLayer}");
        if(layer!=MatchGameManager.Instance.CurrentLayer)
        {Debug.Log("다른 층이라 클릭 불가!");
            return;
        }
          Debug.Log("타일 클릭 성공");
        MatchGameManager.Instance.SelectTile(this);
        MatchGameManager.Instance.SelectedTile(this);
    }
    public void SetImage(Sprite sprite)
    {
        tileImage.sprite=sprite;
    }
    public void Remove()
    {
        Debug.Log($"타일제거! row:{row},col{col},TileID{tileID}");
        gameObject.SetActive(false);
        MatchGameManager.Instance.OnTileRemoved();
    }

    public void SetHighlight(bool on)
    {
        Color highlihtColor=on ? Color.red:Color.black;

        foreach(var line in blockLine)
        {
            line.color=highlihtColor;
        }
    }

   

   
}
