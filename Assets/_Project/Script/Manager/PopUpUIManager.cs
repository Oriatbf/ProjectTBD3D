using DamageNumbersPro;
using UnityEngine;
using UnityEngine.Serialization;
using VInspector;

public class PopUpUIManager : MonoBehaviour
{
    [SerializeField] private DamageNumber damagePopUp,healthPopUp;
    
    public void SpawnDamagePopUp(float value,Transform target)
    {
        if(value>=0) damagePopUp.Spawn(target.position+new Vector3(0,2f), value);
        else  healthPopUp.Spawn(target.position+new Vector3(0,2f), -value);
    }
}
