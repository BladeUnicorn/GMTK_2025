using System.Collections.Generic;
using System.Linq;
using Script.Base;
using UnityEngine;

namespace Script.Manager
{
    public class UnitManager : SingletonBase<UnitManager>
    {
        public List<ScriptableUnit> units;

        protected override void Awake()
        {
            base.Awake();

            units = Resources.LoadAll<ScriptableUnit>("Unitprefab").ToList();
        }

        public void SpawnPlayer()
        {
            var playerUnit = units.FirstOrDefault(u => u.type == EntityType.Player);
            var spawnedUnit = Instantiate(playerUnit);
        }
    }
}