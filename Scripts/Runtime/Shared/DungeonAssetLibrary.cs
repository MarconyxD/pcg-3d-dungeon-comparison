using System.Collections.Generic;
using UnityEngine;

namespace Dissertation.PCG
{
    [CreateAssetMenu(menuName = "Dissertation PCG/Dungeon Asset Library", fileName = "DungeonAssetLibrary")]
    public sealed class DungeonAssetLibrary : ScriptableObject
    {
        [Header("KayKit structural prefabs")]
        [Tooltip("Prefab do tile de chão usado para preencher salas e corredores.")]
        public GameObject floorTilePrefab;
        [Tooltip("Prefab de parede modular usado nas bordas de salas e corredores.")]
        public GameObject wallPrefab;
        [Tooltip("Prefab de porta. Reservado para substituir aberturas simples por portas visuais em versões futuras.")]
        public GameObject doorPrefab;
        [Tooltip("Prefab de escada de subida usado pelos conectores verticais do BSP multiandar.")]
        public GameObject stairsUpPrefab;
        [Tooltip("Prefab de escada de descida usado pelos conectores verticais do BSP multiandar.")]
        public GameObject stairsDownPrefab;

        [Header("Markers")]
        [Tooltip("Prefab opcional usado para marcar visualmente a sala inicial.")]
        public GameObject startMarkerPrefab;
        [Tooltip("Prefab opcional usado para marcar visualmente a sala final ou objetivo.")]
        public GameObject goalMarkerPrefab;

        [Header("Semantic spawn prefabs")]
        [Tooltip("Lista de objetos decorativos que podem ser espalhados pelas salas.")]
        public List<GameObject> propPrefabs = new List<GameObject>();
        [Tooltip("Lista de prefabs de inimigos usados pelo orçamento de spawn.")]
        public List<GameObject> enemyPrefabs = new List<GameObject>();
        [Tooltip("Lista de prefabs de itens, baús ou recompensas usados pelo orçamento de loot.")]
        public List<GameObject> lootPrefabs = new List<GameObject>();
        [Tooltip("Lista de prefabs de armadilhas usados pelo orçamento de traps.")]
        public List<GameObject> trapPrefabs = new List<GameObject>();

        public int StructuralPrefabCount
        {
            get
            {
                int count = 0;
                if (floorTilePrefab != null) count++;
                if (wallPrefab != null) count++;
                if (doorPrefab != null) count++;
                if (stairsUpPrefab != null) count++;
                if (stairsDownPrefab != null) count++;
                return count;
            }
        }

        public GameObject GetRandomProp(System.Random rng)
        {
            return GetRandomFromList(propPrefabs, rng);
        }

        public GameObject GetRandomEnemy(System.Random rng)
        {
            return GetRandomFromList(enemyPrefabs, rng);
        }

        public GameObject GetRandomLoot(System.Random rng)
        {
            return GetRandomFromList(lootPrefabs, rng);
        }

        public GameObject GetRandomTrap(System.Random rng)
        {
            return GetRandomFromList(trapPrefabs, rng);
        }

        private static GameObject GetRandomFromList(List<GameObject> prefabs, System.Random rng)
        {
            if (prefabs == null || prefabs.Count == 0)
            {
                return null;
            }

            return prefabs[rng.Next(0, prefabs.Count)];
        }
    }
}
