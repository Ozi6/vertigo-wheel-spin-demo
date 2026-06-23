using UnityEngine;
using UnityEngine.UI;
using Mathf = UnityEngine.Mathf;

namespace WheelOfFortune.Utility
{
    public class GridContentSizer : MonoBehaviour
    {
        public GridLayoutGroup grid;
        public RectTransform content;

    void OnTransformChildrenChanged()
    {
        UpdateSize();
    }

    void UpdateSize()
    {
        int itemCount = content.childCount;

        int columns = Mathf.FloorToInt(
            (content.rect.width + grid.spacing.x) /
            (grid.cellSize.x + grid.spacing.x)
        );

        columns = Mathf.Max(1, columns);

        int rows = Mathf.CeilToInt((float)itemCount / columns);

        float height =
            rows * grid.cellSize.y +
            (rows - 1) * grid.spacing.y +
            grid.padding.top +
            grid.padding.bottom;

            content.SetSizeWithCurrentAnchors(
                RectTransform.Axis.Vertical,
                height
            );
        }
    }
}