using _Project.Pooling;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthContent : MonoBehaviour,IPoolable
{
    [SerializeField] private TextMeshProUGUI hpTxt,barrierTxt;
    [SerializeField] private Image charmBar_value;
    private Camera _camera;
    [SerializeField]bool isShow = false;
    const float posY = 300f;

    public void Init(StatContainer statContainer)
    {
        statContainer.hp.OnValueChanged += SetHpTxt;
        statContainer.barrier.OnValueChanged += SetBarrierTxt;
        statContainer.charmResist.OnValueChanged += SetCharmBar;
        SetHpTxt(statContainer.hp._baseValue);
        SetBarrierTxt(0);
        SetCharmBar(0);
    }
    
    private void SetHpTxt(float hp)
    {
        hpTxt.text = hp.ToString();
    }

    private void SetBarrierTxt(float value)
    {
        barrierTxt.text = value.ToString();
    }

    private void SetCharmBar(float value)
    {
       // Debug.Log(value);
        charmBar_value.fillAmount = value;
    }

    public void SetPos(Vector3 targetPos)
    {
        if(_camera == null)Debug.Log("HealthContent camera is null");
        Vector3 screenPos = _camera.WorldToScreenPoint(targetPos);
        screenPos += new Vector3(0, posY);
        transform.position = screenPos;
        Show();
    }

    private void Show()
    {
        if (isShow) return;
        isShow = true;
    }

    private void Hide()
    {
        if (!isShow) return;
        isShow = false;
    }

    
    
    public void OnSpawnFromPool()
    {
        Debug.Log("OnSpawnFromPool");
        _camera = Camera.main;
    }

    public void OnReturnToPool()
    {
        
    }

    public void OnDestroyFromPool()
    {
        
    }
}
