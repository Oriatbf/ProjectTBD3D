using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class IconBase : MonoBehaviour
{
    [SerializeField] protected Image icon;
    private readonly string iconPath = "Assets/_Project/Art/Icons/UsingIcon/";
    
    protected async UniTask SetSprite(string spriteName)
    {
        var sprite = await Addressables.LoadAssetAsync<Sprite>(iconPath+spriteName+".png").Task;
        icon.sprite = sprite;
        AlphaIcon(1);
    }

    protected void AlphaIcon(float alpha,bool isDotween = false,float duration = 0.25f)
    {
        if (isDotween) icon.DOFade(alpha, duration);
        else icon.DOFade(alpha, 0);
    }

    protected virtual void Reset()
    {
        icon.sprite = null;
    }

}


