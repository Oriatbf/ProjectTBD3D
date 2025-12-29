using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class IconBase : MonoBehaviour
{
    [SerializeField] protected Image icon;
    private readonly string iconPath = "Assets/_Project/Art/Icons/UsingIcon/";
    
    protected async void SetSprite(string spriteName)
    {
        var sprite = await Addressables.LoadAssetAsync<Sprite>(iconPath+spriteName+".png").Task;
        icon.sprite = sprite;
    }

    protected virtual void Reset()
    {
        icon.sprite = null;
    }

}


