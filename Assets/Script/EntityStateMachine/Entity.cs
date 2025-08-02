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
        public Rigidbody2D rb { get; private set; }
        
        #endregion

        public int facingDir { get; private set; } = 1;
        public bool b_facingRight { get; private set; } = true;

        protected virtual void Awake()
        {
            
        }

        protected virtual void Start()
        {
            anim = GetComponentInChildren<Animator>();
            rb = GetComponent<Rigidbody2D>();
        }

        protected virtual void Update()
        {
            
        }

        protected virtual void Exit()
        {
            
        }


        #region Flip API

        /// <summary>
        /// 翻转逻辑
        /// </summary>
        public virtual void Flip()
        {
            facingDir *= -1;
            b_facingRight = !b_facingRight;
            
            transform.Rotate(0,180,0);
        }

        /// <summary>
        /// 翻转控制
        /// </summary>
        /// <param name="_x"></param>
        /// <param name="_y"></param>
        public virtual void FlipController(float _x,float _y)
        {
            if(_x > 0 && !b_facingRight)
                Flip();
            else if(_x <0 && b_facingRight)
                Flip();
        }

        #endregion
        
        
        #region Velocity API

        /// <summary>
        /// 设置实体速度
        /// </summary>
        /// <param name="_xVelocity"></param>
        /// <param name="_yVelocity"></param>
        public virtual void SetVelocity(float _xVelocity, float _yVelocity)
        {
            rb.velocity = new Vector2(_xVelocity, _yVelocity);
            FlipController(_xVelocity, _yVelocity);
        }

        /// <summary>
        /// 设置实体速度为0
        /// </summary>
        public virtual void SetZeroVelocity()
        {
            rb.velocity = new Vector2(0f, 0f);
        }

        #endregion

    }
}