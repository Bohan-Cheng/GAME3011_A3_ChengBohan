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
    Bomb,
    Default
}

public class S_Jelly : MonoBehaviour
{
    [SerializeField] List<Sprite> Icons;

    public E_JellyType type = E_JellyType.Default;
    Image CurrentImage;
    Animator anim;
    S_Jelly TargetJelly;
    S_Jelly ReverseJelly;
    S_JellyTable Table;
    S_JellyManager Mana;

    Vector3 OriPosition;
    bool isDraging = false;
    bool shouldUpdate = false;
    bool good = false;

    public bool isSet = false;
    public Vector2 GridPos;

    // Start is called before the first frame update
    void Start()
    {
        Table = FindObjectOfType<S_JellyTable>();
        Mana = FindObjectOfType<S_JellyManager>();
        CurrentImage = GetComponentInChildren<Image>();
        anim = GetComponentInChildren<Animator>();

        OriPosition = transform.localPosition;

        SetDifficulty();
        StartCoroutine(Spawn());
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
            switch (Mana.difficulty)
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
        if(GridPos == Table.BombGridPos)
        {
            type = E_JellyType.Bomb;
            Table.BombGridPos = new Vector2(-1, -1);
        }
        CurrentImage.sprite = Icons[(int)type];

        switch (type)
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
                if (Mana.MaxStones == 0)
                {
                    return false;
                }
                else
                {
                    Mana.MaxStones--;
                    anim.enabled = false;
                }
                break;
            case E_JellyType.Bomb:
                break;
            case E_JellyType.Default:
                break;
            default:
                break;
        }
        return true;
    }

    IEnumerator Spawn()
    {
        yield return new WaitForSeconds(Random.Range(0.0f, 1.0f));
        anim.SetTrigger("Fall");
        yield return new WaitForSeconds(0.8f);
        FindObjectOfType<S_SoundMana>().PlayBounce();
        isSet = true;
        if (Table.Jellies.Length == 72)
        {
            Mana.MaxStones = 0;
            Table.CheckForMatch();
        }
    }

    public void Matched()
    {
        if (anim)
        {
            anim.SetTrigger("Match");
        }
        //Invoke("KillSelf", 0.5f);
    }

    public void DragJelly()
    {
        if (type != E_JellyType.Stone && !Table.isChecking && Table.canDrag)
        {
            if (shouldUpdate)
            {
                shouldUpdate = false;
            }
            transform.SetAsLastSibling();
            transform.parent.SetAsLastSibling();
            transform.position = Input.mousePosition;
            isSet = false;
        }
    }

    public void DropJelly()
    {
        if (type != E_JellyType.Stone && !Table.isChecking && Table.canDrag)
        {
            shouldUpdate = true;
            FindClosestJelly();
            SwapJellies();
            isSet = true;
            Table.CheckForMatch();
            Invoke("CheckForReverse", 0.6f);
            Table.canDrag = false;
            Table.JellyDropped();
        }
        
    }

    void CheckForReverse()
    {
        if(!Table.isMatching)
        {
            ReverseJellies();
        }
    }

    void SwapJellies()
    {
        // TODO: Swap back if no match
        if (TargetJelly && TargetJelly.type != E_JellyType.Stone)
        {
            Vector3 temp = TargetJelly.OriPosition;
            Vector2 temp2 = TargetJelly.GridPos;

            Table.Jellies[(int)GridPos.x, (int)GridPos.y] = TargetJelly.gameObject;
            Table.Jellies[(int)temp2.x, (int)temp2.y] = gameObject;
            

            TargetJelly.OriPosition = OriPosition;
            TargetJelly.GridPos = GridPos;
            TargetJelly.shouldUpdate = true;

            OriPosition = temp;
            GridPos = temp2;

            anim.SetTrigger("Bounce");
            TargetJelly.anim.SetTrigger("Bounce");
            FindObjectOfType<S_SoundMana>().PlayBounce();

            ReverseJelly = TargetJelly;
            TargetJelly = null;
        }
    }

    void ReverseJellies()
    {
        TargetJelly = ReverseJelly;
        ReverseJelly = null;
        // TODO: Swap back if no match
        if (TargetJelly && TargetJelly.type != E_JellyType.Stone)
        {
            Vector3 temp = TargetJelly.OriPosition;
            Vector2 temp2 = TargetJelly.GridPos;

            Table.Jellies[(int)GridPos.x, (int)GridPos.y] = TargetJelly.gameObject;
            Table.Jellies[(int)temp2.x, (int)temp2.y] = gameObject;


            TargetJelly.OriPosition = OriPosition;
            TargetJelly.GridPos = GridPos;
            TargetJelly.shouldUpdate = true;
            shouldUpdate = true;

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

        foreach (GameObject go in Table.Jellies)
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
