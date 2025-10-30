using System.Collections.Generic;
using UnityEngine;
using VInspector;

public class TileManager : MonoBehaviour
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

    /// <summary>
    /// 타일 소환
    /// </summary>
    private void InstanceTile()
    {
        for (int j = 0; j < rowCount; j++)
        {
            float z = tileParent.position.z + (j*interval*4);
            float xInterval = j%2==0?interval:0;
            for (int i = 0; i < halfCount; i++)
            {
                var obj = Instantiate(tilePrefab, new Vector3(-(halfCount -i)-(interval*(halfCount -i))+xInterval,tileParent.position.y,z), Quaternion.identity,tileParent);
                Tiles[new Vector2(i,j)] = obj;
            } 
            var mid =Instantiate(tilePrefab, new Vector3(0+xInterval,tileParent.position.y,z), Quaternion.identity,tileParent);
            Tiles[new Vector2(halfCount,j)] = mid;

            for (int i = halfCount - 1; i >= 0; i--)
            {
                var obj = Instantiate(tilePrefab, new Vector3((halfCount -i)+(interval*(halfCount -i))+xInterval,tileParent.position.y,z), Quaternion.identity,tileParent);
                Tiles[new Vector2(halfCount*2-i,j)] = obj;
            }
        }
       
    }
    
    /// <summary>
    /// 타일 가져오기
    /// </summary>
    public Tile GetPlayerTile(Vector2 vec)=> Tiles[vec];
    public Tile GetEnemyTile(Vector2 vec) => Tiles[new Vector2(halfCount+1,0) + vec];
    public Tile GetTile(Vector2 vec) => Tiles[vec];
}
