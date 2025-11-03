using DamageNumbersPro;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class PopUpUIController : BaseController
{
    private DamageNumber damagePopUp,healPopUp;
    private string damageUIPath = "Assets/_Project/Prefab/UI/PopUp/DamagePopUp.prefab";
    private string healUIPath = "Assets/_Project/Prefab/UI/PopUp/HealPopUp.prefab";
    

    public override void OnInitialize()
    {
        base.OnInitialize();
       setting();
    }

    /// <summary>
    /// DamageNumber 에셋 Prefab 할당
    /// </summary>
    private async void setting()
    {
        var damageUI = await Addressables.LoadAssetAsync<GameObject>(damageUIPath).Task;
        if(damageUI.TryGetComponent<DamageNumber>(out damagePopUp)) damagePopUp = damageUI.GetComponent<DamageNumber>();
        var healUI = await Addressables.LoadAssetAsync<GameObject>(healUIPath).Task;
        if(healUI.TryGetComponent<DamageNumber>(out healPopUp)) healPopUp = healUI.GetComponent<DamageNumber>();
         
    }

    /// <summary>
    /// 데미지/힐을 받았을 때 UI띄우기
    /// </summary>
    public void SpawnDamagePopUp(float value,Transform target)
    {
        if(value>=0) damagePopUp.Spawn(target.position+new Vector3(0,2f), value);
        else  healPopUp.Spawn(target.position+new Vector3(0,2f), -value);
    }
}
