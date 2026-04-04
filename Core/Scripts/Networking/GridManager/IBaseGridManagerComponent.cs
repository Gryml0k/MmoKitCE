using UnityEngine;

namespace MultiplayerARPG
{
    public interface IBaseGridManagerComponenent
    {
        bool IsDisabled { get; }
        ushort CellSize { get; }
        int GridSize { get; }
        void SetupDynamicGrid();

        byte GetCellId(Vector3 pos);

        void GetCell(byte id, out GridCell gridCell);

        Vector3 GetCellLocalPosition(byte cellId, Vector3 position);

        Vector3 GetWorldPosition(byte cellId, Vector3 position);

        int GetCompressionMode(float distSq, int previousMode);
    }
}

