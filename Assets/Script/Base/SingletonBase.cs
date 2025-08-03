using UnityEngine;

namespace Script.Base
{
    /// <summary>
    /// 单例基类
    /// </summary>
    /// <remarks>作为父类继承即可使用，实现组件的单例模式</remarks>
    /// <typeparam name="T">组件自身</typeparam>
    public class SingletonBase<T> : MonoBehaviour where T:SingletonBase<T>
    {
        public static T instance{ get; private set; }

        protected virtual void Awake()
        {
            //如果不是单例类
            if (instance != null)
            {
                Debug.LogError(this + "不符合单例模式！");
                // Destroy(gameObject);
                // return;
            }
            
            instance = this as T;
            
            //保证跨场景时仍然是单例类
            DontDestroyOnLoad(gameObject);
        }

        public void OnDestroy()
        {
            instance = null;
        }
    }
}
