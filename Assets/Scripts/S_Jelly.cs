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
    Yellow
}

public class S_Jelly : MonoBehaviour
{
    [SerializeField] List<Sprite> Icons;

    E_JellyType type = E_JellyType.Black;
    Image CurrentImage;
    Animator anim;
    S_Jelly TargetJelly;

    Vector3 OriPosition;
    bool isDraging = false;
    bool shouldUpdate = false;

    [HideInInspector] public Vector2 GridPos;

    // Start is called before the first frame update
    void Start()
    {
        CurrentImage = GetComponentInChildren<Image>();
        anim = GetComponentInChildren<Animator>();
        StartCoroutine(Spawn());

        OriPosition = transform.localPosition;

        SetType((E_JellyType)Random.Range(0, 6));
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

    void SetType(E_JellyType t)
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
            default:
                break;
        }
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
        if(shouldUpdate)
        {
            shouldUpdate = false;
        }
        transform.SetAsLastSibling();
        transform.parent.SetAsLastSibling();
        transform.position = Input.mousePosition;
    }

    public void DropJelly()
    {
        shouldUpdate = true;
        FindClosestJelly();
        SwapJellies();
    }

    void SwapJellies()
    {
        if (TargetJelly)
        {
            Vector3 temp = TargetJelly.OriPosition;
            Vector2 temp2 = TargetJelly.GridPos;

            TargetJelly.OriPosition = OriPosition;
            TargetJelly.GridPos = GridPos;
            TargetJelly.shouldUpdate = true;

            OriPosition = temp;
            GridPos = temp2;

            anim.SetTrigger("Bounce");
            TargetJelly.anim.SetTrigger("Bounce");
            FindObjectOfType<S_SoundMana>().PlayBounce();
        }
    }
    void FindClosestJelly()
    {
        GameObject[] gos;
        gos = GameObject.FindGameObjectsWithTag("Jelly");
        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = transform.localPosition;

        foreach (GameObject go in gos)
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
