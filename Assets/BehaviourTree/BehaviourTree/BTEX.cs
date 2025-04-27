using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using BehaviourTree;
using Sirenix.Serialization;
using UnityEngine;

namespace BehaviourTree
{
    public static class BTEX
    {
        /// <summary>
        /// 获取所有继承了该类型的类型，包含子类的子类
        /// </summary>
        public static List<Type> GetDerivedClasses(this Type type)
        {
            List<Type> derivedClasses = new List<Type>();
            
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type t in assembly.GetTypes())
                {
                    if (t.IsClass && !t.IsAbstract && type.IsAssignableFrom(t))
                    {
                        derivedClasses.Add(t);
                    }
                }
            }
            return derivedClasses;
        }

        public static string GetPath(this string path,string FlieName)
        {
            int i = 0;
            while (File.Exists($"{path}/{FlieName}{i}.asset"))
            {
                i++;
            }
            return $"{path}/{FlieName}{i}.asset";
        }

        public static NodeType GetNodeType(this Type type)
        {
            if (type.IsSubclassOf(typeof(Entry))) return NodeType.root;
            if (type.IsSubclassOf(typeof(BtComposite))) return NodeType.composite;
            if (type.IsSubclassOf(typeof(BtPrecondition))) return NodeType.pecondition;
            if (type.IsSubclassOf(typeof(BtActionNode))) return NodeType.action;

            return NodeType.none;
        }
        
    } 
}
