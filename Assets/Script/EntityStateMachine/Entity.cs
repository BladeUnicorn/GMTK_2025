using System;
using System.Collections;
using System.Collections.Generic;
using Script.Manager;
using Unity.VisualScripting;
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
        
        //新输入系统
        public InputSystem_Actions inputSystem { get; private set; }
        public Vector2 inputMoveVec2 {get; private set;}
        public float inputMoveVec2_X { get; private set; }
        public float inputMoveVec2_Y { get; private set; }
        
        public int facingDir { get; private set; } = 1;
        public bool b_facingRight { get; private set; } = true;

        public float moveDuration;
        
        protected virtual void Awake()
        {
            inputSystem = new InputSystem_Actions();
        }

        protected virtual void Start()
        {
            anim = GetComponentInChildren<Animator>();
            rb = GetComponent<Rigidbody2D>();
        }

        protected virtual void Update()
        {
            //读取移动输入
            inputMoveVec2 = inputSystem.GamePlay.Move.ReadValue<Vector2>().normalized;
            inputMoveVec2_X = inputMoveVec2.x;
            inputMoveVec2_Y = inputMoveVec2.y;
        }

        protected virtual void Exit()
        {
            
        }

        private void OnEnable()
        {
            inputSystem.Enable();
        }

        private void OnDisable()
        {
            inputSystem.Disable();
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

        /// <summary>
        /// 按格移动
        /// </summary>
        public virtual void DiscreteMovement()
        {
            if (Mathf.Abs(inputMoveVec2_X) == 1)
            {
                Vector3 target = transform.position + new Vector3(inputMoveVec2_X, 0, 0);
                StartCoroutine(MoveToPosition(target));
            }
            else if (Mathf.Abs(inputMoveVec2_Y) == 1)
            {
                Vector3 target = transform.position + new Vector3(0, inputMoveVec2_Y, 0);
                StartCoroutine(MoveToPosition(target));
            }
        }

        /// <summary>
        /// 插值移动到某处
        /// </summary>
        /// <param name="_target">目标位置</param>
        /// <returns></returns>
        private IEnumerator MoveToPosition(Vector3 _target)
        {
            Vector3 start = transform.position;
            float elapsed = 0f;

            while (elapsed < moveDuration)
            {
                transform.position = Vector3.Lerp(start, _target, elapsed / moveDuration);
                elapsed += Time.deltaTime;
                yield return null;
            }
            transform.position = _target;
        }

        #endregion

    }
}