using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_JellyTable : MonoBehaviour
{
    [SerializeField] GameObject JellyPrefab;
    public int SizeX = 9;
    public int SizeY = 8;
    GameObject[,] Jellies;

    Vector2 Offset = new Vector2(-320.0f, 320.0f);

    void Start()
    {
        SpawnJellies();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SpawnJellies()
    {
        Jellies = new GameObject[SizeX, SizeY];
        for (int y = 0; y < SizeY; y++)
        {
            for (int x = 0; x < SizeX; x++)
            {
                GameObject jelly = Instantiate(JellyPrefab, transform);
                Vector2 Pos = jelly.GetComponent<RectTransform>().anchoredPosition;
                Pos.x = x * 80.0f;
                Pos.y = y * -80.0f;
                jelly.GetComponent<RectTransform>().anchoredPosition = Pos + Offset;
                Jellies[x,y] = jelly;
            }
        }
    }

}
