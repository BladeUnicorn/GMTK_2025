using UnityEngine;

namespace Script.Tiles
{
    public class GrassTile : Tile
    {
        [SerializeField] private Color baseColor, offsetColor;

        public override void Init(int _x, int _y)
        {
            //对瓦片分组
            bool b_isOffset = (_x + _y) % 2 == 1;   //表示坐标是否是一奇一偶
            //给不同组的瓦片设置不同的颜色
            sr.color = b_isOffset ? offsetColor : baseColor;
        }
    }
}
