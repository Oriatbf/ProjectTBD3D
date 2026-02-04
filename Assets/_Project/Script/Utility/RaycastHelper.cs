using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Core.Utility
{
    public static class RaycastHelper
    {
        public static bool IsPointerOverTargetUI<T>() where T : MonoBehaviour
        {
            PointerEventData eventData = new PointerEventData(EventSystem.current);
            eventData.position = Input.mousePosition;

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);

            foreach (var result in results)
            {
                if (result.gameObject.GetComponent<T>() != null)
                {
                    return true;
                }
            }
            return false;
        }
    }
}