using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class CharacterHead : MonoBehaviour
{
   [SerializeField]private Image image;
   private readonly string spritePath = "Assets/_Project/Art/Icons/UsingIcon/";
   private UnitSaveData curData;
   
   public async void Init(UnitSaveData unitSaveData)
   {
      curData = unitSaveData;
      string test = "Assets/_Project/Art/Icons/UsingIcon/Baldo.png";
      var sprite = await Addressables.LoadAssetAsync<Sprite>(test);
      if(image !=null)image.sprite = sprite;
   }
   
   public UnitSaveData GetUnitData() => curData;
}
