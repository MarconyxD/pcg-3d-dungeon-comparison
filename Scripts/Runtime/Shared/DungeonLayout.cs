using System.Collections.Generic;
using UnityEngine;

namespace Dissertation.PCG
{
    public enum DungeonCellKind
    {
        Empty = 0,
        Room = 1,
        Corridor = 2
    }

    public enum DungeonMapMarkerKind
    {
        Start = 0,
        Goal = 1,
        Prop = 2,
        Enemy = 3,
        Loot = 4,
        Trap = 5,
        StairsUp = 6,
        VerticalExit = 7
    }

    public struct DungeonGridCell
    {
        public int x;
        public int z;
        public int floorIndex;

        public DungeonGridCell(int x, int z, int floorIndex)
        {
            this.x = x;
            this.z = z;
            this.floorIndex = floorIndex;
        }

        public Vector2Int Cell2D
        {
            get { return new Vector2Int(x, z); }
        }
    }

    [System.Serializable]
    public sealed class DungeonMapMarker
    {
        public DungeonMapMarkerKind kind;
        public int x;
        public int z;
        public int floorIndex;
        public string label;

        public Vector2Int Cell2D
        {
            get { return new Vector2Int(x, z); }
        }
    }

    [System.Serializable]
    public sealed class DungeonRoom
    {
        public int id;
        public RectInt bounds;
        public int floorIndex;
        public string moduleId;

        public int AreaCells
        {
            get { return bounds.width * bounds.height; }
        }

        public Vector2Int CenterCell
        {
            get { return new Vector2Int(bounds.xMin + bounds.width / 2, bounds.yMin + bounds.height / 2); }
        }
    }

    [System.Serializable]
    public sealed class DungeonConnection
    {
        public int roomAId;
        public int roomBId;
        public Vector2Int fromCell;
        public Vector2Int toCell;
        public float gridDistance;
        public bool isVertical;
        public bool isExtraLoop;
    }

    public sealed class DungeonLayout
    {
        public readonly int width;
        public readonly int depth;
        public readonly int floorCount;
        public readonly float floorHeight;
        public readonly DungeonCellKind[,] cells;
        public readonly DungeonCellKind[,,] cellsByFloor;
        public readonly List<DungeonRoom> rooms = new List<DungeonRoom>();
        public readonly List<DungeonConnection> connections = new List<DungeonConnection>();
        public readonly List<DungeonMapMarker> markers = new List<DungeonMapMarker>();
        public readonly List<DungeonGridCell> floorOpenings = new List<DungeonGridCell>();

        private readonly HashSet<string> floorOpeningKeys = new HashSet<string>();

        public int startRoomId = -1;
        public int goalRoomId = -1;

        public DungeonLayout(int width, int depth, float floorHeight, int floorCount = 1)
        {
            this.width = width;
            this.depth = depth;
            this.floorCount = Mathf.Max(1, floorCount);
            this.floorHeight = floorHeight;
            cells = new DungeonCellKind[width, depth];
            cellsByFloor = new DungeonCellKind[width, depth, this.floorCount];
        }

        public bool InBounds(int x, int z)
        {
            return x >= 0 && z >= 0 && x < width && z < depth;
        }

        public bool InBounds(int x, int z, int floorIndex)
        {
            return InBounds(x, z) && floorIndex >= 0 && floorIndex < floorCount;
        }

        public bool InBounds(Vector2Int cell)
        {
            return InBounds(cell.x, cell.y);
        }

        public bool InBounds(Vector2Int cell, int floorIndex)
        {
            return InBounds(cell.x, cell.y, floorIndex);
        }

        public bool IsOccupied(int x, int z)
        {
            return IsOccupied(x, z, 0);
        }

        public bool IsOccupied(int x, int z, int floorIndex)
        {
            return InBounds(x, z, floorIndex) && cellsByFloor[x, z, floorIndex] != DungeonCellKind.Empty;
        }

        public bool IsOccupied(Vector2Int cell)
        {
            return IsOccupied(cell.x, cell.y);
        }

        public bool IsOccupied(Vector2Int cell, int floorIndex)
        {
            return IsOccupied(cell.x, cell.y, floorIndex);
        }

        public void MarkCell(int x, int z, DungeonCellKind kind)
        {
            MarkCell(x, z, 0, kind);
        }

        public void MarkCell(int x, int z, int floorIndex, DungeonCellKind kind)
        {
            if (!InBounds(x, z, floorIndex))
            {
                return;
            }

            if (kind == DungeonCellKind.Room || cellsByFloor[x, z, floorIndex] == DungeonCellKind.Empty)
            {
                cellsByFloor[x, z, floorIndex] = kind;
                if (floorIndex == 0)
                {
                    cells[x, z] = kind;
                }
            }
        }

        public void MarkFloorOpening(int x, int z, int floorIndex)
        {
            if (!InBounds(x, z, floorIndex))
            {
                return;
            }

            string key = CellKey(x, z, floorIndex);
            if (floorOpeningKeys.Contains(key))
            {
                return;
            }

            floorOpeningKeys.Add(key);
            floorOpenings.Add(new DungeonGridCell(x, z, floorIndex));
        }

        public bool IsFloorOpening(int x, int z, int floorIndex)
        {
            return floorOpeningKeys.Contains(CellKey(x, z, floorIndex));
        }

        public bool IsFloorOpening(Vector2Int cell, int floorIndex)
        {
            return IsFloorOpening(cell.x, cell.y, floorIndex);
        }

        public void AddMarker(DungeonMapMarkerKind kind, Vector2Int cell, int floorIndex, string label)
        {
            if (!InBounds(cell, floorIndex))
            {
                return;
            }

            DungeonMapMarker marker = new DungeonMapMarker();
            marker.kind = kind;
            marker.x = cell.x;
            marker.z = cell.y;
            marker.floorIndex = floorIndex;
            marker.label = label;
            markers.Add(marker);
        }

        public IEnumerable<DungeonMapMarker> MarkersOnFloor(int floorIndex)
        {
            for (int i = 0; i < markers.Count; i++)
            {
                if (markers[i].floorIndex == floorIndex)
                {
                    yield return markers[i];
                }
            }
        }

        public IEnumerable<DungeonGridCell> FloorOpeningsOnFloor(int floorIndex)
        {
            for (int i = 0; i < floorOpenings.Count; i++)
            {
                if (floorOpenings[i].floorIndex == floorIndex)
                {
                    yield return floorOpenings[i];
                }
            }
        }

        public int CountOccupiedCells()
        {
            int count = 0;
            for (int floor = 0; floor < floorCount; floor++)
            {
                count += CountOccupiedCells(floor);
            }

            return count;
        }

        public int CountOccupiedCells(int floorIndex)
        {
            if (floorIndex < 0 || floorIndex >= floorCount)
            {
                return 0;
            }

            int count = 0;
            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < depth; z++)
                {
                    if (cellsByFloor[x, z, floorIndex] != DungeonCellKind.Empty)
                    {
                        count++;
                    }
                }
            }

            return count;
        }

        public IEnumerable<Vector2Int> OccupiedCells()
        {
            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < depth; z++)
                {
                    if (cellsByFloor[x, z, 0] != DungeonCellKind.Empty)
                    {
                        yield return new Vector2Int(x, z);
                    }
                }
            }
        }

        public IEnumerable<DungeonGridCell> OccupiedGridCells()
        {
            for (int floor = 0; floor < floorCount; floor++)
            {
                for (int x = 0; x < width; x++)
                {
                    for (int z = 0; z < depth; z++)
                    {
                        if (cellsByFloor[x, z, floor] != DungeonCellKind.Empty)
                        {
                            yield return new DungeonGridCell(x, z, floor);
                        }
                    }
                }
            }
        }

        public DungeonRoom GetRoomById(int roomId)
        {
            for (int i = 0; i < rooms.Count; i++)
            {
                if (rooms[i].id == roomId)
                {
                    return rooms[i];
                }
            }

            return null;
        }

        public bool HasConnection(int roomAId, int roomBId)
        {
            for (int i = 0; i < connections.Count; i++)
            {
                DungeonConnection connection = connections[i];
                if ((connection.roomAId == roomAId && connection.roomBId == roomBId) ||
                    (connection.roomAId == roomBId && connection.roomBId == roomAId))
                {
                    return true;
                }
            }

            return false;
        }

        private static string CellKey(int x, int z, int floorIndex)
        {
            return floorIndex + ":" + x + "," + z;
        }
    }
}
