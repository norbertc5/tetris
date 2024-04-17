using UnityEngine;

namespace norbertcUtilities.Grid
{
    public class GridGenerator : MonoBehaviour
    {
        [SerializeField] private GameObject cellPrefab;
        [SerializeField] private int width;
        [SerializeField] private int height;
        [SerializeField] private Color color1;
        [SerializeField] private Color color2;

        void Awake()
        {
            // generates grid

            // set position of the board in middle
            Vector2 position = -(new Vector3(width * cellPrefab.transform.localScale.x, height * cellPrefab.transform.localScale.y)
                - cellPrefab.transform.localScale) / 2;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    GameObject newCell = Instantiate(cellPrefab, position, Quaternion.identity, transform);
                    newCell.name = $"Cell ({x}, {y})";
                    position.x += cellPrefab.transform.localScale.x;

                    // set colors like in chessboard
                    if ((y + x) % 2 == 0)
                        newCell.transform.GetChild(0).GetComponent<SpriteRenderer>().color = color1;
                    else
                        newCell.transform.GetChild(0).GetComponent<SpriteRenderer>().color = color2;
                }
                position.y += cellPrefab.transform.localScale.y;
                position.x = -(width * cellPrefab.transform.localScale.x - cellPrefab.transform.localScale.x) / 2;
            }
        }
    }
}
