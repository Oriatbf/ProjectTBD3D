using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using VInspector;
using Object = UnityEngine.Object;

public class TileController : BaseController
{
    [Foldout("assignment")]
    private Tile tilePrefab;
    private Transform tileParent; 
    private  int _rowCount=3; //행 개수
    private int _halfCount=5; //열개수 /2
    private float _interval=1; // 타일간의 간격
    private float _rowInterval=3, _columnInterval=0;
    [EndFoldout]
    private  Dictionary<Vector2,Tile> Tiles = new Dictionary<Vector2, Tile>();
    // vector2 에는 x,y형태로 들어감 {열,행} 순

    public override ControllerInfo ControllerInfo { get; } = new()
    {
        ContainSceneNames = new string[] {"GamePlay" },
        Priority = 0,
        UpdateInterval = 0,
        LateUpdateInterval = 0,
        FixedUpdateInterval = 0,
    };

    public override void OnInitialize()
    {
        base.OnInitialize();
        tileParent = new GameObject("TileParent").transform;
        tilePrefab = Resources.Load<Tile>("Tile");
    }

    /// <summary>
    /// 타일 소환
    /// </summary>
    public void InstanceTile()
    {
        for (int j = 0; j < _rowCount; j++)
        {
            Vector2 indexVec = Vector2.zero;
            float zInterval = tileParent.position.z + (j*_rowInterval); //z축 인터벌
            float startInterval = j%2==0?_interval:0; //짝수 홀수 행 인터벌
            for (int i = 0; i < _halfCount; i++)
            {
                var obj = Object.Instantiate(tilePrefab, new Vector3(-(_halfCount -i)-(_interval*(_halfCount -i))+startInterval+_columnInterval*i,tileParent.position.y,zInterval), Quaternion.identity,tileParent);
                indexVec = new Vector2(i,j);
                Tiles[indexVec] = obj;
                obj.SetIndex(indexVec);
            } 
            var mid =Object.Instantiate(tilePrefab, new Vector3(0+startInterval+_columnInterval*_halfCount,tileParent.position.y,zInterval), Quaternion.identity,tileParent);
            indexVec = new Vector2(_halfCount,j);
            Tiles[indexVec] = mid;
            mid.SetIndex(indexVec);

            for (int i = _halfCount - 1; i >= 0; i--)
            {
                indexVec = new Vector2(_halfCount*2-i,j);
                var obj = Object.Instantiate(tilePrefab, new Vector3((_halfCount -i)+(_interval*(_halfCount -i)+_columnInterval*(_halfCount*2-i))+startInterval,tileParent.position.y,zInterval), Quaternion.identity,tileParent);
                Tiles[indexVec] = obj;
                obj.SetIndex(indexVec);
            }
        }
       
    }
    
    /// <summary>
    /// 타일 가져오기
    /// </summary>
    public Tile GetPlayerTile(Vector2 vec)=> Tiles[vec];
    public Tile GetEnemyTile(Vector2 vec) => Tiles[new Vector2(_halfCount+1,0) + vec];
    public Tile GetTile(Vector2 vec)
    {
        
        return Tiles[vec];
    }

    public int GetHalfCount() => _halfCount;
    public int GetRowCount() => _rowCount;

    public List<Tile> GetTiles(Vector2 targetIndex, int rowCount, int columnCount)
    {
        Tile targetTile = GetTile(targetIndex);
        if (targetTile == null)
        {
            Debug.LogError("해당 인덱스의 타일이 없음");
            return null;
        }
        return GetTiles(targetTile,rowCount,columnCount);
    }

    public List<Tile> GetTiles(Tile targetTile, int rowCount, int columnCount)
    {
        rowCount = Mathf.Clamp(rowCount,1,rowCount);
        columnCount = Mathf.Clamp(columnCount,1,columnCount);
        
        if(targetTile == null)Debug.LogError("targetTile is null");
        HashSet<Tile> _tiles = new HashSet<Tile>();
        int xPos = (int)targetTile.GetIndex().x;
        int yPos = (int)targetTile.GetIndex().y;
        for (int i = xPos; i < xPos + rowCount; i++)
        {
            for (int j = yPos; j< yPos + columnCount; j++)
            {
                if(Tiles.ContainsKey(new Vector2Int(i,j)))
                    _tiles.Add(Tiles[new Vector2Int(i,j)]);
            }
        }
        
        return _tiles.ToList();
    }

    public List<Tile> GetAllTiles()
    {
        List<Tile> _tiles = new List<Tile>();
        foreach (var tile in Tiles.Values)
        {
            _tiles.Add(tile);
        }
        return _tiles;
    }
}
