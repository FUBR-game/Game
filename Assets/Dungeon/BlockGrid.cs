using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets
{
    class BlockGrid
    {
        int gridWidth;
        int gridHeight;
        Vector2 minReal;
        Vector2 maxReal;

        float realWidth;
        float realHeight;

        public BlockGrid(int gridWidth, int gridHeight, Vector2 minReal, Vector2 maxReal)
        {
            this.gridWidth = gridWidth;
            this.gridHeight = gridHeight;
            this.minReal = minReal;
            this.maxReal = maxReal;
            realWidth = Mathf.Abs(minReal.x - maxReal.x);
            realHeight = Mathf.Abs(minReal.y - maxReal.y);
        }

        public BlockGrid(int gridWidth, int gridHeight, Vector2 bounds) : this(gridWidth, gridHeight, -bounds/2, bounds/2)
        {
        }

        public Vector2 GridPosToRealPos(Vector2Int gridPos)
        {
            return new Vector2(Mathf.Lerp(minReal.x, maxReal.x, gridPos.x / (float)gridWidth), Mathf.Lerp(minReal.y, maxReal.y, gridPos.y / (float)gridWidth));
        }

        public Vector2 GetRealLength(Vector2Int gridLength)
        {
            return new Vector2(Mathf.Lerp(0.0f, realWidth, (float)gridLength.x/gridWidth), Mathf.Lerp(0.0f, realHeight, (float)gridLength.y/gridWidth));
        }
    }
}
