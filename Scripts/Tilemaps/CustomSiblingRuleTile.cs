using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class CustomSiblingRuleTile : RuleTile {

    public enum SiblingGroup {
        Detail,
        Terrain,
    }
    public SiblingGroup siblingGroup;

    public string siblingTag = "SomeTag";

    public Tile borderTile;

    public override bool RuleMatch(int neighbor, TileBase other) {
        if (other is RuleOverrideTile)
            other = (other as RuleOverrideTile).m_InstanceTile;

        switch (neighbor) {
            case TilingRule.Neighbor.This:
                return other is CustomSiblingRuleTile && (other as CustomSiblingRuleTile).siblingGroup == this.siblingGroup;
            case TilingRule.Neighbor.NotThis:
                return !(other is CustomSiblingRuleTile && (other as CustomSiblingRuleTile).siblingGroup == this.siblingGroup);
        }

        return base.RuleMatch(neighbor, other);
    }

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData) {
        base.GetTileData(position, tilemap, ref tileData);

        var currentTile = tilemap.GetTile(position) as CustomSiblingRuleTile;

        bool hasTopTile = tilemap.GetTile(position + Vector3Int.up) != null;
        bool hasBottomTile = tilemap.GetTile(position + Vector3Int.down) != null;
        bool hasLeftTile = tilemap.GetTile(position + Vector3Int.left) != null;
        bool hasRightTile = tilemap.GetTile(position + Vector3Int.right) != null;

        if (hasTopTile && hasBottomTile && hasLeftTile && hasRightTile) {
            for (int x = -1; x <= 1; x += 2) {
                for (int y = -1; y <= 1; y += 2) {
                    var neighborPosition = position + new Vector3Int(x, y);
                    var neighborTile = tilemap.GetTile(neighborPosition) as CustomSiblingRuleTile;

                    if (neighborTile != null && currentTile != null && neighborTile.siblingTag != currentTile.siblingTag) {
                        tileData.sprite = borderTile.sprite;
                        tileData.colliderType = borderTile.colliderType;
                        tileData.flags = borderTile.flags;
                        return;
                    }
                }
            }
        }
    }
}
