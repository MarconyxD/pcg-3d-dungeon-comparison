using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;

namespace Dissertation.PCG
{
    public static class DungeonMap2DExporter
    {
        private const int Margin = 16;
        private const int TitleHeight = 32;
        private const int LegendWidth = 250;
        private const int LegendRowHeight = 24;

        private static readonly Color32 BackgroundColor = new Color32(24, 27, 31, 255);
        private static readonly Color32 EmptyColor = new Color32(34, 37, 42, 255);
        private static readonly Color32 RoomColor = new Color32(214, 198, 165, 255);
        private static readonly Color32 CorridorColor = new Color32(120, 170, 190, 255);
        private static readonly Color32 OpeningColor = new Color32(8, 12, 16, 255);
        private static readonly Color32 OpeningBorderColor = new Color32(93, 224, 230, 255);
        private static readonly Color32 WallColor = new Color32(57, 63, 72, 255);
        private static readonly Color32 GridColor = new Color32(74, 78, 85, 255);
        private static readonly Color32 TextColor = new Color32(238, 238, 232, 255);
        private static readonly Color32 DarkTextColor = new Color32(24, 24, 24, 255);
        private static readonly Color32 StartColor = new Color32(79, 191, 110, 255);
        private static readonly Color32 GoalColor = new Color32(224, 76, 76, 255);
        private static readonly Color32 EnemyColor = new Color32(151, 104, 214, 255);
        private static readonly Color32 LootColor = new Color32(242, 190, 65, 255);
        private static readonly Color32 TrapColor = new Color32(229, 122, 51, 255);
        private static readonly Color32 PropColor = new Color32(57, 185, 164, 255);
        private static readonly Color32 VerticalColor = new Color32(84, 141, 232, 255);

        private static readonly Dictionary<char, string[]> Glyphs = BuildGlyphs();

        public static List<string> ExportFloorMaps(
            DungeonLayout layout,
            string folderPath,
            string filePrefix,
            string mapTitlePrefix,
            int seed,
            string exportLabel,
            int pixelsPerCell,
            bool includeGrid,
            bool includeLegend)
        {
            List<string> paths = new List<string>();
            if (layout == null)
            {
                return paths;
            }

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            int safePixelsPerCell = Mathf.Clamp(pixelsPerCell, 4, 48);
            string safePrefix = string.IsNullOrEmpty(filePrefix) ? "dungeon" : CleanFilePart(filePrefix);
            string safeLabel = string.IsNullOrEmpty(exportLabel) ? "map" : CleanFilePart(exportLabel);
            string timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture);

            for (int floor = 0; floor < layout.floorCount; floor++)
            {
                Texture2D texture = RenderFloor(layout, floor, mapTitlePrefix, seed, safePixelsPerCell, includeGrid, includeLegend);
                string fileName = safePrefix
                    + "_map_"
                    + safeLabel
                    + "_seed_"
                    + seed.ToString(CultureInfo.InvariantCulture)
                    + "_floor_"
                    + floor.ToString(CultureInfo.InvariantCulture)
                    + "_"
                    + timestamp
                    + ".png";
                string path = Path.Combine(folderPath, fileName);
                File.WriteAllBytes(path, texture.EncodeToPNG());
                paths.Add(path);

                if (Application.isPlaying)
                {
                    UnityEngine.Object.Destroy(texture);
                }
                else
                {
                    UnityEngine.Object.DestroyImmediate(texture);
                }
            }

            return paths;
        }

        private static Texture2D RenderFloor(DungeonLayout layout, int floorIndex, string mapTitlePrefix, int seed, int pixelsPerCell, bool includeGrid, bool includeLegend)
        {
            int mapWidth = layout.width * pixelsPerCell;
            int mapHeight = layout.depth * pixelsPerCell;
            int legendWidth = includeLegend ? LegendWidth : 0;
            int width = Margin + mapWidth + Margin + legendWidth + Margin;
            int height = Mathf.Max(TitleHeight + mapHeight + Margin, includeLegend ? TitleHeight + 13 * LegendRowHeight + Margin : 0);

            Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
            Fill(texture, BackgroundColor);

            string title = CleanTitlePrefix(mapTitlePrefix) + " MAP - SEED " + seed.ToString(CultureInfo.InvariantCulture) + " - FLOOR " + floorIndex.ToString(CultureInfo.InvariantCulture);
            DrawText(texture, Margin, 9, title, TextColor, 2);

            int mapX = Margin;
            int mapY = TitleHeight;
            DrawFilledRect(texture, mapX, mapY, mapWidth, mapHeight, EmptyColor);

            for (int x = 0; x < layout.width; x++)
            {
                for (int z = 0; z < layout.depth; z++)
                {
                    DungeonCellKind kind = layout.cellsByFloor[x, z, floorIndex];
                    if (kind == DungeonCellKind.Empty)
                    {
                        continue;
                    }

                    int cellX = mapX + x * pixelsPerCell;
                    int cellY = mapY + (layout.depth - 1 - z) * pixelsPerCell;
                    Color32 cellColor = kind == DungeonCellKind.Room ? RoomColor : CorridorColor;
                    DrawFilledRect(texture, cellX, cellY, pixelsPerCell, pixelsPerCell, cellColor);

                    if (includeGrid)
                    {
                        DrawRect(texture, cellX, cellY, pixelsPerCell, pixelsPerCell, GridColor, 1);
                    }
                }
            }

            foreach (DungeonGridCell opening in layout.FloorOpeningsOnFloor(floorIndex))
            {
                int cellX = mapX + opening.x * pixelsPerCell;
                int cellY = mapY + (layout.depth - 1 - opening.z) * pixelsPerCell;
                DrawFilledRect(texture, cellX, cellY, pixelsPerCell, pixelsPerCell, OpeningColor);
                DrawRect(texture, cellX, cellY, pixelsPerCell, pixelsPerCell, OpeningBorderColor, Mathf.Max(1, pixelsPerCell / 6));
            }

            DrawBoundaryWalls(texture, layout, floorIndex, mapX, mapY, pixelsPerCell);
            DrawMarkers(texture, layout, floorIndex, mapX, mapY, pixelsPerCell);

            if (includeLegend)
            {
                DrawLegend(texture, mapX + mapWidth + Margin, TitleHeight);
            }

            texture.Apply(false, false);
            return texture;
        }

        private static string CleanTitlePrefix(string mapTitlePrefix)
        {
            if (string.IsNullOrWhiteSpace(mapTitlePrefix))
            {
                return "DUNGEON";
            }

            return mapTitlePrefix.Trim().ToUpperInvariant();
        }

        private static void DrawBoundaryWalls(Texture2D texture, DungeonLayout layout, int floorIndex, int mapX, int mapY, int pixelsPerCell)
        {
            int thickness = Mathf.Max(1, pixelsPerCell / 5);
            for (int x = 0; x < layout.width; x++)
            {
                for (int z = 0; z < layout.depth; z++)
                {
                    if (!layout.IsOccupied(x, z, floorIndex))
                    {
                        continue;
                    }

                    int cellX = mapX + x * pixelsPerCell;
                    int cellY = mapY + (layout.depth - 1 - z) * pixelsPerCell;

                    if (!layout.IsOccupied(x, z + 1, floorIndex))
                    {
                        DrawFilledRect(texture, cellX, cellY, pixelsPerCell, thickness, WallColor);
                    }

                    if (!layout.IsOccupied(x + 1, z, floorIndex))
                    {
                        DrawFilledRect(texture, cellX + pixelsPerCell - thickness, cellY, thickness, pixelsPerCell, WallColor);
                    }

                    if (!layout.IsOccupied(x, z - 1, floorIndex))
                    {
                        DrawFilledRect(texture, cellX, cellY + pixelsPerCell - thickness, pixelsPerCell, thickness, WallColor);
                    }

                    if (!layout.IsOccupied(x - 1, z, floorIndex))
                    {
                        DrawFilledRect(texture, cellX, cellY, thickness, pixelsPerCell, WallColor);
                    }
                }
            }
        }

        private static void DrawMarkers(Texture2D texture, DungeonLayout layout, int floorIndex, int mapX, int mapY, int pixelsPerCell)
        {
            foreach (DungeonMapMarker marker in layout.MarkersOnFloor(floorIndex))
            {
                if (marker.kind == DungeonMapMarkerKind.Start || marker.kind == DungeonMapMarkerKind.Goal)
                {
                    continue;
                }

                DrawMarker(texture, marker, layout.depth, mapX, mapY, pixelsPerCell);
            }

            foreach (DungeonMapMarker marker in layout.MarkersOnFloor(floorIndex))
            {
                if (marker.kind != DungeonMapMarkerKind.Start && marker.kind != DungeonMapMarkerKind.Goal)
                {
                    continue;
                }

                DrawMarker(texture, marker, layout.depth, mapX, mapY, pixelsPerCell);
            }
        }

        private static void DrawMarker(Texture2D texture, DungeonMapMarker marker, int depth, int mapX, int mapY, int pixelsPerCell)
        {
            int cellX = mapX + marker.x * pixelsPerCell;
            int cellY = mapY + (depth - 1 - marker.z) * pixelsPerCell;
            int centerX = cellX + pixelsPerCell / 2;
            int centerY = cellY + pixelsPerCell / 2;
            int radius = Mathf.Max(2, pixelsPerCell / 2 - 1);
            Color32 color = MarkerColor(marker.kind);

            if (marker.kind == DungeonMapMarkerKind.Enemy)
            {
                DrawDiamond(texture, centerX, centerY, radius, color);
            }
            else if (marker.kind == DungeonMapMarkerKind.Trap)
            {
                DrawTriangle(texture, centerX, centerY, radius, color);
            }
            else if (marker.kind == DungeonMapMarkerKind.Loot || marker.kind == DungeonMapMarkerKind.Prop)
            {
                int size = Mathf.Max(4, pixelsPerCell - 3);
                DrawFilledRect(texture, centerX - size / 2, centerY - size / 2, size, size, color);
            }
            else
            {
                DrawCircle(texture, centerX, centerY, radius, color);
            }

            DrawRect(texture, cellX + 1, cellY + 1, Mathf.Max(1, pixelsPerCell - 2), Mathf.Max(1, pixelsPerCell - 2), BackgroundColor, 1);

            if (pixelsPerCell >= 8)
            {
                string letter = MarkerLetter(marker.kind);
                int scale = pixelsPerCell >= 16 ? 2 : 1;
                int textWidth = MeasureText(letter, scale);
                int textHeight = 7 * scale;
                Color32 textColor = marker.kind == DungeonMapMarkerKind.Loot ? DarkTextColor : TextColor;
                DrawText(texture, centerX - textWidth / 2, centerY - textHeight / 2, letter, textColor, scale);
            }
        }

        private static void DrawLegend(Texture2D texture, int x, int y)
        {
            DrawText(texture, x, y, "LEGENDA", TextColor, 2);
            int rowY = y + 28;

            DrawLegendSwatch(texture, x, rowY, RoomColor, "SALA"); rowY += LegendRowHeight;
            DrawLegendSwatch(texture, x, rowY, CorridorColor, "CORREDOR"); rowY += LegendRowHeight;
            DrawLegendSwatch(texture, x, rowY, OpeningColor, "ABERTURA PISO"); rowY += LegendRowHeight;
            DrawLegendMarker(texture, x, rowY, DungeonMapMarkerKind.Start, "S INICIO"); rowY += LegendRowHeight;
            DrawLegendMarker(texture, x, rowY, DungeonMapMarkerKind.Goal, "G SAIDA"); rowY += LegendRowHeight;
            DrawLegendMarker(texture, x, rowY, DungeonMapMarkerKind.StairsUp, "V ESCADA"); rowY += LegendRowHeight;
            DrawLegendMarker(texture, x, rowY, DungeonMapMarkerKind.VerticalExit, "O CHEGADA"); rowY += LegendRowHeight;
            DrawLegendMarker(texture, x, rowY, DungeonMapMarkerKind.Enemy, "E INIMIGO"); rowY += LegendRowHeight;
            DrawLegendMarker(texture, x, rowY, DungeonMapMarkerKind.Loot, "L LOOT"); rowY += LegendRowHeight;
            DrawLegendMarker(texture, x, rowY, DungeonMapMarkerKind.Trap, "T ARMADILHA"); rowY += LegendRowHeight;
            DrawLegendMarker(texture, x, rowY, DungeonMapMarkerKind.Prop, "P PROP");
        }

        private static void DrawLegendSwatch(Texture2D texture, int x, int y, Color32 color, string label)
        {
            DrawFilledRect(texture, x, y + 3, 16, 16, color);
            DrawRect(texture, x, y + 3, 16, 16, WallColor, 1);
            DrawText(texture, x + 26, y + 5, label, TextColor, 1);
        }

        private static void DrawLegendMarker(Texture2D texture, int x, int y, DungeonMapMarkerKind kind, string label)
        {
            int centerX = x + 8;
            int centerY = y + 12;
            if (kind == DungeonMapMarkerKind.Enemy)
            {
                DrawDiamond(texture, centerX, centerY, 8, MarkerColor(kind));
            }
            else if (kind == DungeonMapMarkerKind.Trap)
            {
                DrawTriangle(texture, centerX, centerY, 8, MarkerColor(kind));
            }
            else if (kind == DungeonMapMarkerKind.Loot || kind == DungeonMapMarkerKind.Prop)
            {
                DrawFilledRect(texture, x + 1, y + 5, 15, 15, MarkerColor(kind));
            }
            else
            {
                DrawCircle(texture, centerX, centerY, 8, MarkerColor(kind));
            }

            string letter = MarkerLetter(kind);
            Color32 textColor = kind == DungeonMapMarkerKind.Loot ? DarkTextColor : TextColor;
            DrawText(texture, centerX - 3, centerY - 4, letter, textColor, 1);
            DrawText(texture, x + 26, y + 6, label, TextColor, 1);
        }

        private static Color32 MarkerColor(DungeonMapMarkerKind kind)
        {
            if (kind == DungeonMapMarkerKind.Start) return StartColor;
            if (kind == DungeonMapMarkerKind.Goal) return GoalColor;
            if (kind == DungeonMapMarkerKind.Enemy) return EnemyColor;
            if (kind == DungeonMapMarkerKind.Loot) return LootColor;
            if (kind == DungeonMapMarkerKind.Trap) return TrapColor;
            if (kind == DungeonMapMarkerKind.Prop) return PropColor;
            return VerticalColor;
        }

        private static string MarkerLetter(DungeonMapMarkerKind kind)
        {
            if (kind == DungeonMapMarkerKind.Start) return "S";
            if (kind == DungeonMapMarkerKind.Goal) return "G";
            if (kind == DungeonMapMarkerKind.Enemy) return "E";
            if (kind == DungeonMapMarkerKind.Loot) return "L";
            if (kind == DungeonMapMarkerKind.Trap) return "T";
            if (kind == DungeonMapMarkerKind.Prop) return "P";
            if (kind == DungeonMapMarkerKind.VerticalExit) return "O";
            return "V";
        }

        private static void Fill(Texture2D texture, Color32 color)
        {
            Color32[] pixels = new Color32[texture.width * texture.height];
            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = color;
            }

            texture.SetPixels32(pixels);
        }

        private static void DrawFilledRect(Texture2D texture, int x, int y, int width, int height, Color32 color)
        {
            for (int px = 0; px < width; px++)
            {
                for (int py = 0; py < height; py++)
                {
                    SetPixel(texture, x + px, y + py, color);
                }
            }
        }

        private static void DrawRect(Texture2D texture, int x, int y, int width, int height, Color32 color, int thickness)
        {
            DrawFilledRect(texture, x, y, width, thickness, color);
            DrawFilledRect(texture, x, y + height - thickness, width, thickness, color);
            DrawFilledRect(texture, x, y, thickness, height, color);
            DrawFilledRect(texture, x + width - thickness, y, thickness, height, color);
        }

        private static void DrawCircle(Texture2D texture, int centerX, int centerY, int radius, Color32 color)
        {
            int radiusSquared = radius * radius;
            for (int dx = -radius; dx <= radius; dx++)
            {
                for (int dy = -radius; dy <= radius; dy++)
                {
                    if (dx * dx + dy * dy <= radiusSquared)
                    {
                        SetPixel(texture, centerX + dx, centerY + dy, color);
                    }
                }
            }
        }

        private static void DrawDiamond(Texture2D texture, int centerX, int centerY, int radius, Color32 color)
        {
            for (int dx = -radius; dx <= radius; dx++)
            {
                for (int dy = -radius; dy <= radius; dy++)
                {
                    if (Mathf.Abs(dx) + Mathf.Abs(dy) <= radius)
                    {
                        SetPixel(texture, centerX + dx, centerY + dy, color);
                    }
                }
            }
        }

        private static void DrawTriangle(Texture2D texture, int centerX, int centerY, int radius, Color32 color)
        {
            for (int dy = -radius; dy <= radius; dy++)
            {
                int rowWidth = radius - Mathf.Abs(dy / 2);
                for (int dx = -rowWidth; dx <= rowWidth; dx++)
                {
                    if (dy >= -radius / 2)
                    {
                        SetPixel(texture, centerX + dx, centerY + dy, color);
                    }
                }
            }
        }

        private static void DrawText(Texture2D texture, int x, int y, string text, Color32 color, int scale)
        {
            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            string upper = text.ToUpperInvariant();
            int cursorX = x;
            for (int i = 0; i < upper.Length; i++)
            {
                char c = upper[i];
                if (c == ' ')
                {
                    cursorX += 4 * scale;
                    continue;
                }

                string[] glyph;
                if (!Glyphs.TryGetValue(c, out glyph))
                {
                    glyph = Glyphs['?'];
                }

                for (int row = 0; row < glyph.Length; row++)
                {
                    string line = glyph[row];
                    for (int col = 0; col < line.Length; col++)
                    {
                        if (line[col] != '#')
                        {
                            continue;
                        }

                        DrawFilledRect(texture, cursorX + col * scale, y + row * scale, scale, scale, color);
                    }
                }

                cursorX += 6 * scale;
            }
        }

        private static int MeasureText(string text, int scale)
        {
            if (string.IsNullOrEmpty(text))
            {
                return 0;
            }

            int width = 0;
            for (int i = 0; i < text.Length; i++)
            {
                width += text[i] == ' ' ? 4 * scale : 6 * scale;
            }

            return Mathf.Max(0, width - scale);
        }

        private static void SetPixel(Texture2D texture, int x, int yFromTop, Color32 color)
        {
            if (x < 0 || yFromTop < 0 || x >= texture.width || yFromTop >= texture.height)
            {
                return;
            }

            texture.SetPixel(x, texture.height - 1 - yFromTop, color);
        }

        private static string CleanFilePart(string value)
        {
            char[] invalid = Path.GetInvalidFileNameChars();
            for (int i = 0; i < invalid.Length; i++)
            {
                value = value.Replace(invalid[i], '_');
            }

            return value.Replace(" ", "_");
        }

        private static Dictionary<char, string[]> BuildGlyphs()
        {
            Dictionary<char, string[]> glyphs = new Dictionary<char, string[]>();
            Add(glyphs, 'A', ".###.", "#...#", "#...#", "#####", "#...#", "#...#", "#...#");
            Add(glyphs, 'B', "####.", "#...#", "#...#", "####.", "#...#", "#...#", "####.");
            Add(glyphs, 'C', ".####", "#....", "#....", "#....", "#....", "#....", ".####");
            Add(glyphs, 'D', "####.", "#...#", "#...#", "#...#", "#...#", "#...#", "####.");
            Add(glyphs, 'E', "#####", "#....", "#....", "####.", "#....", "#....", "#####");
            Add(glyphs, 'F', "#####", "#....", "#....", "####.", "#....", "#....", "#....");
            Add(glyphs, 'G', ".####", "#....", "#....", "#.###", "#...#", "#...#", ".####");
            Add(glyphs, 'H', "#...#", "#...#", "#...#", "#####", "#...#", "#...#", "#...#");
            Add(glyphs, 'I', "#####", "..#..", "..#..", "..#..", "..#..", "..#..", "#####");
            Add(glyphs, 'J', "..###", "...#.", "...#.", "...#.", "#..#.", "#..#.", ".##..");
            Add(glyphs, 'K', "#...#", "#..#.", "#.#..", "##...", "#.#..", "#..#.", "#...#");
            Add(glyphs, 'L', "#....", "#....", "#....", "#....", "#....", "#....", "#####");
            Add(glyphs, 'M', "#...#", "##.##", "#.#.#", "#...#", "#...#", "#...#", "#...#");
            Add(glyphs, 'N', "#...#", "##..#", "#.#.#", "#..##", "#...#", "#...#", "#...#");
            Add(glyphs, 'O', ".###.", "#...#", "#...#", "#...#", "#...#", "#...#", ".###.");
            Add(glyphs, 'P', "####.", "#...#", "#...#", "####.", "#....", "#....", "#....");
            Add(glyphs, 'Q', ".###.", "#...#", "#...#", "#...#", "#.#.#", "#..#.", ".##.#");
            Add(glyphs, 'R', "####.", "#...#", "#...#", "####.", "#.#..", "#..#.", "#...#");
            Add(glyphs, 'S', ".####", "#....", "#....", ".###.", "....#", "....#", "####.");
            Add(glyphs, 'T', "#####", "..#..", "..#..", "..#..", "..#..", "..#..", "..#..");
            Add(glyphs, 'U', "#...#", "#...#", "#...#", "#...#", "#...#", "#...#", ".###.");
            Add(glyphs, 'V', "#...#", "#...#", "#...#", "#...#", ".#.#.", ".#.#.", "..#..");
            Add(glyphs, 'W', "#...#", "#...#", "#...#", "#.#.#", "#.#.#", "##.##", "#...#");
            Add(glyphs, 'X', "#...#", ".#.#.", "..#..", "..#..", "..#..", ".#.#.", "#...#");
            Add(glyphs, 'Y', "#...#", ".#.#.", "..#..", "..#..", "..#..", "..#..", "..#..");
            Add(glyphs, 'Z', "#####", "....#", "...#.", "..#..", ".#...", "#....", "#####");
            Add(glyphs, '0', ".###.", "#...#", "#..##", "#.#.#", "##..#", "#...#", ".###.");
            Add(glyphs, '1', "..#..", ".##..", "..#..", "..#..", "..#..", "..#..", ".###.");
            Add(glyphs, '2', ".###.", "#...#", "....#", "...#.", "..#..", ".#...", "#####");
            Add(glyphs, '3', "####.", "....#", "....#", ".###.", "....#", "....#", "####.");
            Add(glyphs, '4', "#...#", "#...#", "#...#", "#####", "....#", "....#", "....#");
            Add(glyphs, '5', "#####", "#....", "#....", "####.", "....#", "....#", "####.");
            Add(glyphs, '6', ".####", "#....", "#....", "####.", "#...#", "#...#", ".###.");
            Add(glyphs, '7', "#####", "....#", "...#.", "..#..", ".#...", ".#...", ".#...");
            Add(glyphs, '8', ".###.", "#...#", "#...#", ".###.", "#...#", "#...#", ".###.");
            Add(glyphs, '9', ".###.", "#...#", "#...#", ".####", "....#", "....#", "####.");
            Add(glyphs, '-', ".....", ".....", ".....", ".###.", ".....", ".....", ".....");
            Add(glyphs, '_', ".....", ".....", ".....", ".....", ".....", ".....", "#####");
            Add(glyphs, ':', ".....", "..#..", ".....", ".....", ".....", "..#..", ".....");
            Add(glyphs, '/', "....#", "...#.", "...#.", "..#..", ".#...", ".#...", "#....");
            Add(glyphs, '?', ".###.", "#...#", "....#", "...#.", "..#..", ".....", "..#..");
            return glyphs;
        }

        private static void Add(Dictionary<char, string[]> glyphs, char key, params string[] rows)
        {
            glyphs[key] = rows;
        }
    }
}
