using System;
using Cysharp.Threading.Tasks;
using UnityEngine.EventSystems;

namespace _Project.Script.FieldObject
{
    public class Shop : FieldObject
    {
        private void OnMouseDown()
        {
            if (EventSystem.current.IsPointerOverGameObject())
                return;

            ApplicationManager.Inst
                .GetModule<ShopController>()
                .Show().Forget();
        }
    }
}
