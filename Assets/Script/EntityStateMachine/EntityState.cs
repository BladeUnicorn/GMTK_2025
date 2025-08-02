using UnityEngine;

namespace Script.StateMachine
{
    /// <summary>
    /// 状态基类
    /// </summary>
    public class EntityState
    {
        //状态机相关
        protected Machine machine;
        protected Entity entity;
        private string animBoolName;
        
        //通用计时器
        protected float stateTimer;

        public EntityState(Entity _entityBase, Machine _machine, string _animBoolName)
        {
            this.entity = _entityBase;
            this.machine = _machine;
            this.animBoolName = _animBoolName;
        }

        public virtual void OnEnter()
        {
            //播放动画
            entity.anim.SetBool(animBoolName,true);
        }

        public virtual void OnUpdate()
        {
            stateTimer -= Time.deltaTime;
        }

        public virtual void OnLateUpdate()
        {
            
        }

        public virtual void OnExit()
        {
            //停止播放动画
            entity.anim.SetBool(animBoolName,false);
        }
    }
}