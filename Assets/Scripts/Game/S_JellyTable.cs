using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_JellyTable : MonoBehaviour
{
    [SerializeField] GameObject JellyPrefab;
    public GameObject[,] Jellies;
    public int SizeX = 9;
    public int SizeY = 8;
    List<GameObject> matchObj;
    List<GameObject> ToKillObj;
    public bool isChecking = false;
    public bool isMatching = false;
    public bool canDrag = true;

    Vector2 Offset = new Vector2(-320.0f, 320.0f);
    public Vector2 BombGridPos = new Vector2(-1, -1);

    void Start()
    {
        matchObj = new List<GameObject>();
        ToKillObj = new List<GameObject>();
        Jellies = new GameObject[SizeX, SizeY];
        Invoke("SpawnJellies", 1.0f);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void CheckForMatch()
    {
        if (IsTableSet())
        {
            isChecking = true;
            StartCoroutine(DelayStart());
        }
        
    }

    public void JellyDropped()
    {
        canDrag = false;
        CancelInvoke("ResetCanDrag");
        Invoke("ResetCanDrag", 0.7f);
    }

    void ResetCanDrag()
    {
        canDrag = true;
    }

    void SpawnJellies()
    {
        for (int y = 0; y < SizeY; y++)
        {
            for (int x = 0; x < SizeX; x++)
            {
                SpawnAt(x, y);
            }
        }
    }

    void SpawnAt(int x, int y)
    {
        GameObject jelly = Instantiate(JellyPrefab, transform);
        Vector2 Pos = jelly.GetComponent<RectTransform>().anchoredPosition;
        Pos.x = x * 80.0f;
        Pos.y = y * -80.0f;
        jelly.GetComponent<RectTransform>().anchoredPosition = Pos + Offset;
        jelly.GetComponent<S_Jelly>().GridPos = new Vector2(x, y);
        Jellies[x, y] = jelly;
    }

    public void ResetTable()
    {
        foreach (GameObject j in Jellies)
        {
            Destroy(j);
        }
        SpawnJellies();
    }

    IEnumerator DelayStart()
    {
        yield return new WaitForSeconds(0.5f);
        CheckMatch();
    }

    public void CheckMatch()
    {
        MatchJelliesX();
        MatchJelliesY();
        if(ToKillObj.Count != 0)
        {
            FindObjectOfType<S_JellyManager>().AddScore(ToKillObj.Count * 25);
            foreach (GameObject j in ToKillObj)
            {
                j.GetComponent<S_Jelly>().Matched();
                StartCoroutine(RespawnJelly(j));
            }
            ToKillObj.Clear();
            FindObjectOfType<S_SoundMana>().PlayPop();
            isMatching = true;
        }
        else
        {
            isChecking = false;
            isMatching = false;
        }
        
    }

    IEnumerator RespawnJelly(GameObject j)
    {
        yield return new WaitForSeconds(0.5f);
        if (j)
        {
            Vector2 Pos = j.GetComponent<S_Jelly>().GridPos;
            Destroy(j);
            SpawnAt((int)Pos.x, (int)Pos.y);
        }
    }

    bool ValidKill(GameObject obj)
    {
        foreach (GameObject j in ToKillObj)
        {
            if(j == obj)
            {
                return false;
            }
        }
        return true;
    }

    public bool IsTableSet()
    {
        foreach (GameObject j in Jellies)
        {
            if(j && !j.GetComponent<S_Jelly>().isSet)
            {
                return false;
            }
        }
        return true;
    }

    void KillMatch()
    {
        if (matchObj[0] && matchObj[0].GetComponent<S_Jelly>().type != E_JellyType.Stone)
        {
            if (matchObj.Count >= 5)
            {
                Debug.Log("More than 5 matched!");
                bool BombSpawned = false;
                foreach (GameObject j in matchObj)
                {
                    if (ValidKill(j))
                    {
                        if(!BombSpawned)
                        {
                            BombGridPos = matchObj[Mathf.RoundToInt(matchObj.Count/2)].GetComponent<S_Jelly>().GridPos;
                            BombSpawned = true;
                        }
                        ToKillObj.Add(j);
                    }
                }
            }
            else if (matchObj.Count >= 3)
            {
                Debug.Log("More than 3 matched!");
                foreach (GameObject j in matchObj)
                {
                    if (ValidKill(j))
                    {
                        ToKillObj.Add(j);
                    }
                }
            }
        }
        matchObj.Clear();
    }

    void MatchJelliesX()
    {
        for (int y = 0; y < SizeY; y++)
        {
            matchObj = new List<GameObject>();
            for (int x = 0; x < SizeX; x++)
            {
                if (x == 0) { matchObj.Add(Jellies[x, y]); }
                else
                {
                    if (Jellies[x, y] && Jellies[x - 1, y])
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
                    }
                    if (x + 1 == SizeX) { KillMatch(); }
                }
            }
        }
    }

    void MatchJelliesY()
    {
        for (int x = 0; x < SizeX; x++)
        {
            matchObj = new List<GameObject>();
            for (int y = 0; y < SizeY; y++)
            {
                if (y == 0) { matchObj.Add(Jellies[x, y]); }
                else
                {
                    if (Jellies[x, y] && Jellies[x, y - 1])
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
                    }
                    if (y + 1 == SizeY) { KillMatch(); }
                }
            }
        }
    }

}
