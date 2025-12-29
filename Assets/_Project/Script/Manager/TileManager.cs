using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using VInspector;

public class TileManager : Singleton<TileManager>
{
    [Foldout("assignment")]
    [SerializeField] private Tile tilePrefab;
    [SerializeField] private Transform tileParent;
    [SerializeField] private int rowCount; //행 개수
    [SerializeField] private int halfCount; //열개수 /2
    [SerializeField] private float interval; // 타일간의 간격
    [SerializeField] private float rowInterval, columnInterval;
    [EndFoldout]
    private  Dictionary<Vector2,Tile> Tiles = new Dictionary<Vector2, Tile>();
    // vector2 에는 x,y형태로 들어감 {열,행} 순

    private void Awake()
    {
        InstanceTile();
    }
    

    /// <summary>
    /// 타일 소환
    /// </summary>
    private void InstanceTile()
    {
        for (int j = 0; j < rowCount; j++)
        {
            Vector2 indexVec = Vector2.zero;
            float zInterval = tileParent.position.z + (j*rowInterval); //z축 인터벌
            float startInterval = j%2==0?interval:0; //짝수 홀수 행 인터벌
            for (int i = 0; i < halfCount; i++)
            {
                var obj = Instantiate(tilePrefab, new Vector3(-(halfCount -i)-(interval*(halfCount -i))+startInterval+columnInterval*i,tileParent.position.y,zInterval), Quaternion.identity,tileParent);
                indexVec = new Vector2(i,j);
                Tiles[indexVec] = obj;
                obj.SetIndex(indexVec);
            } 
            var mid =Instantiate(tilePrefab, new Vector3(0+startInterval+columnInterval*halfCount,tileParent.position.y,zInterval), Quaternion.identity,tileParent);
            indexVec = new Vector2(halfCount,j);
            Tiles[indexVec] = mid;
            mid.SetIndex(indexVec);

            for (int i = halfCount - 1; i >= 0; i--)
            {
                indexVec = new Vector2(halfCount*2-i,j);
                var obj = Instantiate(tilePrefab, new Vector3((halfCount -i)+(interval*(halfCount -i)+columnInterval*(halfCount*2-i))+startInterval,tileParent.position.y,zInterval), Quaternion.identity,tileParent);
                Tiles[indexVec] = obj;
                obj.SetIndex(indexVec);
            }
        }
       
    }
    
    /// <summary>
    /// 타일 가져오기
    /// </summary>
    public Tile GetPlayerTile(Vector2 vec)=> Tiles[vec];
    public Tile GetEnemyTile(Vector2 vec) => Tiles[new Vector2(halfCount+1,0) + vec];
    public Tile GetTile(Vector2 vec)
    {
        
        return Tiles[vec];
    }

    public int GetHalfCount() => halfCount;
    public int GetRowCount() => rowCount;

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
