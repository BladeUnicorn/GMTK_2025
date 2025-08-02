using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Script
{
    public class GridManager : SingletonBase<GridManager>
    {
        [SerializeField] private int width, height;
        [SerializeField] private Tile grassTile;
        [SerializeField] private Tile blockTile;
        private Dictionary<Vector3,Tile> tiles = new Dictionary<Vector3, Tile>(); 
        
        [SerializeField] private Transform cam;

        private void Start()
        {
            // cam.transform.position = new Vector3((float)(width-1) / 2, (float)(height-1) / 2, -10);
            InitCamera();
            GenerateGrid();
        }

        /// <summary>
        /// 生成瓦片
        /// </summary>
        public void GenerateGrid()
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var ramdomTile = Random.Range(0, 6) == 3 ? blockTile : grassTile;
                    var spawnedTile = Instantiate(ramdomTile,new Vector3(x,y),Quaternion.identity);
                    spawnedTile.name = $"Tile {x} {y}";
                    
                    //执行瓦片的初始化函数
                    spawnedTile.Init(x,y);
                    
                    //存储瓦片
                    tiles[new Vector3(x, y, 0)] = spawnedTile;
                }
            }
        }

        /// <summary>
        /// 初始化相机
        /// </summary>
        public void InitCamera()
        {
            Vector3 mapCenter = new Vector3((float)(width - 1) / 2, (float)(height - 1) / 2, 0);
            
            cam.transform.position = mapCenter + new Vector3(0, -4.5f, -6f);
            cam.transform.rotation = Quaternion.Euler(-30, 0, 0);
        }

        #region Tile API

        /// <summary>
        /// 获取某位置上的瓦片
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public Tile GetTileAtPosition(Vector3 pos)
        {
            if (tiles.TryGetValue(pos, out var tile))
            {
                return tile;
            }

            return null;
        }
        
        public Tile GetTileAtPosition(Vector2 pos)
        {
            if (tiles.TryGetValue(new Vector3(pos.x, pos.y, 0), out var tile))
            {
                return tile;
            }

            return null;
        }

        #endregion

        
    }
}
