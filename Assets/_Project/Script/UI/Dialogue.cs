using _Project.Pooling;
using TMPro;
using UnityEngine;

public class Dialogue : MonoBehaviour,IPoolable
{
    [SerializeField] private TextMeshProUGUI dialogueTxt;

    public void SetTxt(string text)
    {
        dialogueTxt.text = text;
    }

    public void OnSpawnFromPool()
    {
        
    }

    public void OnReturnToPool()
    {
        
    }

    public void OnDestroyFromPool()
    {
        
    }
}
