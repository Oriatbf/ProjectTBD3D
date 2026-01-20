using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BuffIcon : MonoBehaviour,IPointerMoveHandler,IPointerEnterHandler,IPointerExitHandler
{
    [SerializeField] private Image icon;
    private ActionState _actionState;
    private readonly string iconPath = "Assets/_Project/Art/Icons/UsingIcon/BuffIcons/";
    

    public virtual void Init(ActionState actionState)
    {
        this._actionState = actionState;
        if (icon != null) SetSprite(actionState.GetId()).Forget();
    }

    private async UniTask SetSprite(string spriteName)
    {
        var sprite = await Addressables.LoadAssetAsync<Sprite>(iconPath+spriteName+".png").Task;
        icon.sprite = sprite;
    }
    public ActionState GetActionState() => _actionState;
    
    public void OnPointerMove(PointerEventData eventData)
    {
        //if(skill ==null) return;
        ApplicationManager.Inst.GetModule<BuffInfoController>().InitData(_actionState,Input.mousePosition);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //if(skill ==null) return;
         ApplicationManager.Inst.GetModule<BuffInfoController>().Hide();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //if(skill ==null) return;
        ApplicationManager.Inst.GetModule<BuffInfoController>().Show();
    }
}
