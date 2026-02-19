using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using VInspector;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class TileController : BaseController
{
    [Foldout("assignment")]
    private Tile tilePrefab;
    private Transform tileParent; 
    private  int _rowCount=3; //행 개수
    private int _halfCount=4; //열개수 /2
    private float _interval=1.2f; // 타일간의 간격
    private float _rowInterval=3f, _columnInterval=.2f;
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
        int totalColumn = _halfCount * 2 + 1;
        float tileSpacing = 1f + _interval + _columnInterval;

        for (int j = 0; j < _rowCount; j++)
        {
            float z = tileParent.position.z + j * _rowInterval;
            float rowOffset = j % 2 == 0 ? _interval : 0;

            float totalWidth = (totalColumn - 1) * tileSpacing;
            float startX = -totalWidth / 2f;

            for (int i = 0; i < totalColumn; i++)
            {
                float x = startX + i * tileSpacing + rowOffset;

                Vector2 index = new Vector2(i, j);
                var tile = Object.Instantiate(
                    tilePrefab,
                    new Vector3(x, tileParent.position.y, z),
                    Quaternion.identity,
                    tileParent
                );

                Tiles[index] = tile;
                tile.SetIndex(index);
            }
        }
    }
    
    /// <summary>
    /// 타일 가져오기
    /// </summary>
    public Tile GetPlayerTile(Vector2 vec)=> Tiles[vec];
    public Tile GetEnemyTile(Vector2 vec) => Tiles[new Vector2(_halfCount+1,0) + vec];

    public Tile GetRandomTile(Team team)
    {
        if (team == Team.PlayerTeam)
        {
            var x = Random.Range(0,_halfCount+1);
            var y = Random.Range(0,_rowCount);
            return Tiles[new Vector2(x,y)];
        }
        else
        {
            var x = Random.Range(_halfCount+1,_halfCount*2);
            var y = Random.Range(0,_rowCount);
            return Tiles[new Vector2(x,y)];
        }
        
    }
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

    public List<Tile> GetAllTeamTiles(Team team)
    {
        List<Tile> _tiles = new List<Tile>();
        if (team == Team.PlayerTeam)
        {
            foreach (var tile in Tiles.Values)
            {
                if(tile.GetIndex().x <= _halfCount)
                    _tiles.Add(tile);
            }
        }
        else if (team == Team.EnemyTeam)
        {
            foreach (var tile in Tiles.Values)
            {
                if(tile.GetIndex().x > _halfCount)
                    _tiles.Add(tile);
            }
        }
       
        return _tiles;
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

    public Tile GetRandomTile()
    {
        var random = Random.Range(0,Tiles.Count);
        var tiles = Tiles.Values.ToArray();
        return tiles[random];
    }
}
