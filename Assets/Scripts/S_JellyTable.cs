using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_JellyTable : MonoBehaviour
{
    [SerializeField] GameObject JellyPrefab;
   // public List<GameObject> Jellies;
    public GameObject[,] Jellies;
    public int SizeX = 9;
    public int SizeY = 8;
    List<GameObject> matchObj;

    Vector2 Offset = new Vector2(-320.0f, 320.0f);

    void Start()
    {
        matchObj = new List<GameObject>();
        Jellies = new GameObject[SizeX, SizeY];
        SpawnJellies();
        StartCoroutine(DelayStart());
    }

    // Update is called once per frame
    void Update()
    {
    }

    void SpawnJellies()
    {
        for (int y = 0; y < SizeY; y++)
        {
            for (int x = 0; x < SizeX; x++)
            {
                GameObject jelly = Instantiate(JellyPrefab, transform);
                Vector2 Pos = jelly.GetComponent<RectTransform>().anchoredPosition;
                Pos.x = x * 80.0f;
                Pos.y = y * -80.0f;
                jelly.GetComponent<RectTransform>().anchoredPosition = Pos + Offset;
                jelly.GetComponent<S_Jelly>().GridPos = new Vector2(x, y);
                Jellies[x, y] = jelly;
            }
        }
    }

    public void ResetTable()
    {
        foreach (GameObject j in Jellies)
        {
            Destroy(j);
        }
        SpawnJellies();
        StartCoroutine(DelayStart());
    }

    IEnumerator DelayStart()
    {
        yield return new WaitForSeconds(3.0f);
        MatchJelliesX();
        MatchJelliesY();
    }

    void KillMatch()
    {
        if (matchObj[0].GetComponent<S_Jelly>().type != E_JellyType.Stone)
        {
            if (matchObj.Count >= 5)
            {
                Debug.Log("More than 5 matched!");
                foreach (GameObject j in matchObj)
                {
                    //Destroy(j);
                    j.SetActive(false);
                }
            }
            else if (matchObj.Count >= 3)
            {
                Debug.Log("More than 3 matched!");
                foreach (GameObject j in matchObj)
                {
                    //Destroy(j);
                    j.SetActive(false);
                }
            }
        }
        matchObj.Clear();
    }

    public void MatchJelliesX()
    {
        for (int y = 0; y < SizeY; y++)
        {
            matchObj = new List<GameObject>();
            for (int x = 0; x < SizeX; x++)
            {
                if (x == 0) { matchObj.Add(Jellies[x, y]); }
                else
                {
                    if (Jellies[x, y].GetComponent<S_Jelly>().type == Jellies[x - 1, y].GetComponent<S_Jelly>().type)
                    {
                        matchObj.Add(Jellies[x, y]);
                    }
                    else
                    {
                        KillMatch();
                        matchObj.Clear();
                        matchObj.Add(Jellies[x, y]);
                    }
                    if (x + 1 == SizeX) { KillMatch(); }
                }
            }
        }
    }

    public void MatchJelliesY()
    {
        for (int x = 0; x < SizeX; x++)
        {
            matchObj = new List<GameObject>();
            for (int y = 0; y < SizeY; y++)
            {
                if (y == 0) { matchObj.Add(Jellies[x, y]); }
                else
                {
                    if (Jellies[x, y].GetComponent<S_Jelly>().type == Jellies[x, y - 1].GetComponent<S_Jelly>().type)
                    {
                        matchObj.Add(Jellies[x, y]);
                    }
                    else
                    {
                        KillMatch();
                        matchObj.Clear();
                        matchObj.Add(Jellies[x, y]);
                    }
                    if (y + 1 == SizeY) { KillMatch(); }
                }
            }
        }
    }

}
