using System;
using System.Collections.Generic;
using UnityEngine;

public enum RoomState
{
   Reachable,Current,Hovered,None
}

public class RoomTile : MonoBehaviour
{
   private Room _room;
   private RoomState _roomState;
   [SerializeField] private Transform up,down,left,right;
   [SerializeField] private Transform mapEdge;
   private List<Transform> links = new List<Transform>();
   private List<SpriteRenderer> sprList = new List<SpriteRenderer>();
   
   Color defualtColor = Color.white,
      currentColor = Color.green,reachableColor = Color.red,hoveredColor = Color.blue;

   private void Awake()
   {
      SetLink();
      SetSpr();
   }

   private void SetLink()
   {
      links.Add(up);
      links.Add(down);
      links.Add(left);
      links.Add(right);
   }

   private void SetSpr()
   {
      sprList.Add(mapEdge.GetComponent<SpriteRenderer>());
      foreach (var link in links)
      {
         sprList.Add(link.GetComponent<SpriteRenderer>());
      }
   }

   public void InitRoomData(Room room)
   {
      _room = room;
      SetLinkVisual();
   }

   public void SetRoomState(RoomState roomState)
   {
      _roomState = roomState;
      ChangeMaterial(roomState);
   }

   private void ChangeMaterial(RoomState roomState)
   {
      Color color = Color.clear;
      switch (roomState)
      {
         case RoomState.Reachable:
            color = reachableColor;
            break;
         case RoomState.Current:
            color = currentColor;
            break;
         case RoomState.Hovered:
            color = hoveredColor;
            break;
         case RoomState.None:
            color = defualtColor;
            break;
         default:
            color = defualtColor;
            break;
      }

      foreach (var spr in sprList)
      {
         spr.color = color;
      }
   }

   private void SetLinkVisual()
   {
      foreach (var link in links) link.gameObject.SetActive(true);
      
      foreach (var linkData in _room.GetLinkedDict())
      {
         switch (linkData.Key)
         {
            case Direction.Up:
               up.gameObject.SetActive(false);
               break;
            case Direction.Down:
               down.gameObject.SetActive(false);
               break;
            case Direction.Left:
               left.gameObject.SetActive(false);
               break;
            case Direction.Right:
               right.gameObject.SetActive(false);
               break;
            default:
               throw new ArgumentOutOfRangeException();
         }
      }
   }
   
   public Room GetRoom() => _room;
   public RoomState GetRoomState() => _roomState;
}
