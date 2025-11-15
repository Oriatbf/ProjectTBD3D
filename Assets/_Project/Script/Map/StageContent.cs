using Map;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StageContent : MonoBehaviour,IPoolable
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI nameTxt;
    private Camera _camera;
    private readonly float posY = -150;
    public void Init(MapState mapState)
    {
        nameTxt.text = mapState.ToString();
    }
    
    public void SetPos(Vector3 targetPos)
    {
        Vector3 screenPos = _camera.WorldToScreenPoint(targetPos);
        screenPos += new Vector3(0, posY);
        transform.position = screenPos;
    }

    public void OnSpawnFromPool()
    {
       _camera = Camera.main;
    }

    public void OnReturnToPool()
    {
        
    }

    public void OnDestroyFromPool()
    {
        
    }
}
