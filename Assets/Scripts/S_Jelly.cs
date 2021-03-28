using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum E_JellyType
{
    Black,
    Blue,
    Green,
    Purple,
    Red,
    Yellow,
    Stone,
    Bomb
}

public class S_Jelly : MonoBehaviour
{
    [SerializeField] List<Sprite> Icons;

    public E_JellyType type = E_JellyType.Black;
    Image CurrentImage;
    Animator anim;
    S_Jelly TargetJelly;

    Vector3 OriPosition;
    bool isDraging = false;
    bool shouldUpdate = false;
    bool good = false;

    public Vector2 GridPos;

    // Start is called before the first frame update
    void Start()
    {
        CurrentImage = GetComponentInChildren<Image>();
        anim = GetComponentInChildren<Animator>();
        StartCoroutine(Spawn());

        OriPosition = transform.localPosition;

        SetDifficulty();
    }

    // Update is called once per frame
    void Update()
    {
        if (shouldUpdate)
        {
            if (!isDraging && Vector3.Distance(OriPosition, transform.localPosition) >= 0.1f)
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, OriPosition, 8.0f * Time.deltaTime);
                if(Vector3.Distance(OriPosition, transform.localPosition) <= 0.1f)
                {
                    transform.localPosition = OriPosition;
                    shouldUpdate = false;
                }
            }
        }

    }

    void SetDifficulty()
    {
        while (!good)
        {
            switch (FindObjectOfType<S_JellyManager>().difficulty)
            {
                case E_Difficulty.Easy:
                    good = SetType((E_JellyType)Random.Range(0, 3));
                    break;
                case E_Difficulty.Medium:
                    good = SetType((E_JellyType)Random.Range(0, 4));
                    break;
                case E_Difficulty.Hard:
                    good = SetType((E_JellyType)Random.Range(0, 7));
                    break;
                default:
                    break;
            }
        }
    }

    bool SetType(E_JellyType t)
    {
        type = t;
        CurrentImage.sprite = Icons[(int)t];

        switch (t)
        {
            case E_JellyType.Black:
                break;
            case E_JellyType.Blue:
                break;
            case E_JellyType.Green:
                break;
            case E_JellyType.Purple:
                break;
            case E_JellyType.Red:
                break;
            case E_JellyType.Yellow:
                break;
            case E_JellyType.Stone:
                if (FindObjectOfType<S_JellyManager>().MaxStones == 0)
                {
                    return false;
                }
                else
                {
                    FindObjectOfType<S_JellyManager>().MaxStones--;
                    anim.enabled = false;
                }
                break;
            case E_JellyType.Bomb:
                break;
            default:
                break;
        }
        return true;
    }

    IEnumerator Spawn()
    {
        yield return new WaitForSeconds(Random.Range(0.5f, 2.0f));
        anim.SetTrigger("Fall");
        yield return new WaitForSeconds(0.8f);
        FindObjectOfType<S_SoundMana>().PlayBounce();
    }

    public void DragJelly()
    {
        if (type != E_JellyType.Stone)
        {
            if (shouldUpdate)
            {
                shouldUpdate = false;
            }
            transform.SetAsLastSibling();
            transform.parent.SetAsLastSibling();
            transform.position = Input.mousePosition;
        }
    }

    public void DropJelly()
    {
        if (type != E_JellyType.Stone)
        {
            shouldUpdate = true;
            FindClosestJelly();
            SwapJellies();
            FindObjectOfType<S_JellyTable>().MatchJelliesX();
            FindObjectOfType<S_JellyTable>().MatchJelliesY();
        }
    }

    void SwapJellies()
    {
        if (TargetJelly && TargetJelly.type != E_JellyType.Stone)
        {
            Vector3 temp = TargetJelly.OriPosition;
            Vector2 temp2 = TargetJelly.GridPos;

            FindObjectOfType<S_JellyTable>().Jellies[(int)GridPos.x, (int)GridPos.y] = TargetJelly.gameObject;
            FindObjectOfType<S_JellyTable>().Jellies[(int)temp2.x, (int)temp2.y] = gameObject;
            

            TargetJelly.OriPosition = OriPosition;
            TargetJelly.GridPos = GridPos;
            TargetJelly.shouldUpdate = true;

            OriPosition = temp;
            GridPos = temp2;

            anim.SetTrigger("Bounce");
            TargetJelly.anim.SetTrigger("Bounce");
            FindObjectOfType<S_SoundMana>().PlayBounce();

            TargetJelly = null;
        }
    }
    void FindClosestJelly()
    {
        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = transform.localPosition;

        foreach (GameObject go in FindObjectOfType<S_JellyTable>().Jellies)
        {
            if (go != transform.gameObject)
            {
                Vector3 diff = go.transform.localPosition - position;
                float curDistance = diff.sqrMagnitude;
                if (curDistance < distance)
                {
                    closest = go;
                    distance = curDistance;
                }
            }
        }
        if(closest)
        {
            if (IsInSwapRange(closest.GetComponent<S_Jelly>().GridPos))
            {
                TargetJelly = closest.GetComponent<S_Jelly>();
            }
        }
    }

    bool IsInSwapRange(Vector2 targetPos)
    {
        float x = Mathf.Abs(GridPos.x - targetPos.x);
        float y = Mathf.Abs(GridPos.y - targetPos.y);
        float dist = x + y;
        return dist == 1.0f;
    }
}
