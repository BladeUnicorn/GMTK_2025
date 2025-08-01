using System;
using UnityEngine;

namespace Script.StateMachine
{
    /// <summary>
    /// 实例脚本
    /// </summary>
    public class Entity : MonoBehaviour
    {
        //状态机和状态实例在对应子类中写
        
        #region 组件
        
        public Animator anim { get; private set; } 
        
        #endregion

        protected virtual void Awake()
        {
            
        }

        protected virtual void Start()
        {
            anim = GetComponentInChildren<Animator>();
        }

        protected virtual void Update()
        {
            
        }

        protected virtual void Exit()
        {
            
        }
    }
}