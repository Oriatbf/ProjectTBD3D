using System;

namespace _Project.Script.FieldObject
{
    public class Shop : FieldObject
    {
        private void OnMouseDown()
        {
            ApplicationManager.Inst.GetModule<ShopController>().Show();
        }
    }
}
