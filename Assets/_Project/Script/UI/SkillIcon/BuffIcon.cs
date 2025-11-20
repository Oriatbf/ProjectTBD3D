using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BuffIcon : MonoBehaviour,IPointerMoveHandler,IPointerEnterHandler,IPointerExitHandler
{
    [SerializeField] private Image icon;
    protected BuffDebuff buffDebuff;
    private readonly string iconPath = "Assets/_Project/Art/Icons/UsingIcon/BuffIcons/";
    

    public virtual void Init(BuffDebuff buffDebuff)
    {
        this.buffDebuff = buffDebuff;
        if (icon != null) SetSprite(buffDebuff.id);
    }

    private async void SetSprite(string spriteName)
    {
        var sprite = await Addressables.LoadAssetAsync<Sprite>(iconPath+spriteName+".png").Task;
        icon.sprite = sprite;
    }
    public BuffDebuff GetBuffDebuff() => buffDebuff;
    
    public void OnPointerMove(PointerEventData eventData)
    {
        //if(skill ==null) return;
        ApplicationManager.Inst.GetModule<BuffInfoController>().InitData(buffDebuff,Input.mousePosition);
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
