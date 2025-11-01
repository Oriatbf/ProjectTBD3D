using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VInspector;

public class TileManager : Singleton<TileManager>
{
    [Foldout("assignment")]
    [SerializeField] private Tile tilePrefab;
    [SerializeField] private Transform tileParent;
    [SerializeField] private int rowCount; //행 개수
    [SerializeField] private int halfCount; //열개수 /2
    [SerializeField] private float interval; // 타일간의 간격
    [EndFoldout]
    private  Dictionary<Vector2,Tile> Tiles = new Dictionary<Vector2, Tile>();

    private void Awake()
    {
        DIContainer.RegisterService(this);
        InstanceTile();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F3))
        {
            TBDLogger.CommandLog(KeyCode.F3,this,"BombSkillTest");
            var tiles = GetTiles(GetTile(Vector2.zero),2,2);
            Debug.Log(tiles.Count);
            foreach (var tile in tiles)
            {
                tile.Target();
            }
        }
      
        
    }

    /// <summary>
    /// 타일 소환
    /// </summary>
    private void InstanceTile()
    {
        for (int j = 0; j < rowCount; j++)
        {
            Vector2 indexVec = Vector2.zero;
            float z = tileParent.position.z + (j*interval*4);
            float xInterval = j%2==0?interval:0;
            for (int i = 0; i < halfCount; i++)
            {
                var obj = Instantiate(tilePrefab, new Vector3(-(halfCount -i)-(interval*(halfCount -i))+xInterval,tileParent.position.y,z), Quaternion.identity,tileParent);
                indexVec = new Vector2(i,j);
                Tiles[indexVec] = obj;
                obj.SetIndex(indexVec);
            } 
            var mid =Instantiate(tilePrefab, new Vector3(0+xInterval,tileParent.position.y,z), Quaternion.identity,tileParent);
            indexVec = new Vector2(halfCount,j);
            Tiles[indexVec] = mid;
            mid.SetIndex(indexVec);

            for (int i = halfCount - 1; i >= 0; i--)
            {
                indexVec = new Vector2(halfCount*2-i,j);
                var obj = Instantiate(tilePrefab, new Vector3((halfCount -i)+(interval*(halfCount -i))+xInterval,tileParent.position.y,z), Quaternion.identity,tileParent);
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
    public Tile GetTile(Vector2 vec) => Tiles[vec];

    public List<Tile> GetTiles(Tile targetTile, int rowCount, int columnCount)
    {
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
}
