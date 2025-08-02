using UnityEngine;

namespace Script.Tiles
{
    public abstract class Tile : MonoBehaviour
    {
        [SerializeField] protected GameObject highlight;
        [SerializeField] protected SpriteRenderer sr;
        
        public virtual void Init(int _x,int _y)
        {

        }

        private void OnMouseEnter()
        {
            highlight.SetActive(true);
        }

        private void OnMouseExit()
        {
            highlight.SetActive(false);
        }
    }
}
