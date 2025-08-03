using Script.Unit;
using UnityEngine;

namespace Script.Tiles
{
    public abstract class Tile : MonoBehaviour
    {
        [SerializeField] protected GameObject highlight;
        [SerializeField] protected SpriteRenderer sr;
        [SerializeField] private bool b_isTileBlocked;  //瓦片是否是阻挡性瓦片BlockTile

        public BaseUnit occupiedUnit { get; private set; }
        //瓦片是否可走
        public bool b_isWalkable => b_isTileBlocked && occupiedUnit is null;
        
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
