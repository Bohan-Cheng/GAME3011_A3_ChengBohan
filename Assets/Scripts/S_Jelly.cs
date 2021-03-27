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

    // Start is called before the first frame update
    void Start()
    {
        CurrentImage = GetComponent<Image>();
        anim = GetComponent<Animator>();
        StartCoroutine(Spawn());

        OriPosition = transform.parent.localPosition;

        SetType((E_JellyType)Random.Range(0, 6));
    }

    // Update is called once per frame
    void Update()
    {
        if (shouldUpdate)
        {
            if (!isDraging && Vector3.Distance(OriPosition, transform.parent.localPosition) >= 0.1f)
            {
                transform.parent.localPosition = Vector3.Lerp(transform.parent.localPosition, OriPosition, 8.0f * Time.deltaTime);
                if(Vector3.Distance(OriPosition, transform.parent.localPosition) <= 0.1f)
                {
                    transform.parent.localPosition = OriPosition;
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
        anim.SetTrigger("Play");
        StartCoroutine(PlaySound());
    }

    IEnumerator PlaySound()
    {
        yield return new WaitForSeconds(0.8f);
        FindObjectOfType<S_SoundMana>().PlayBounce();
    }

    public void DragJelly()
    {
        transform.parent.SetAsLastSibling();
        transform.parent.parent.SetAsLastSibling();
        transform.parent.position = Input.mousePosition;
    }

    public void DropJelly()
    {
        Debug.Log("DROP");
        shouldUpdate = true;
        FindClosestJelly();
        SwapJellies();
    }

    void SwapJellies()
    {
        if (TargetJelly)
        {
            Vector3 temp = TargetJelly.OriPosition;
            TargetJelly.OriPosition = OriPosition;
            OriPosition = temp;
            TargetJelly.shouldUpdate = true;
        }
    }
    void FindClosestJelly()
    {
        GameObject[] gos;
        gos = GameObject.FindGameObjectsWithTag("Jelly");
        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = transform.parent.localPosition;

        foreach (GameObject go in gos)
        {
            if (go != transform.parent.gameObject)
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
            TargetJelly = closest.GetComponentInChildren<S_Jelly>();
        }
    }
}
