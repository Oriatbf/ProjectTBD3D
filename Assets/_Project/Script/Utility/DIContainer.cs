using System;
using System.Collections.Generic;
using UnityEngine;

public static class DIContainer 
{
      public static Dictionary<Type,object> services = new Dictionary<Type, object>();

      public static void RegisterService<T>(T service) //클래스 저장
      {
            services[typeof(T)] = service;
      }

      public static T ResolveService<T>() //클래스 불러오기
      {
            return (T)services[typeof(T)];
      }

}
