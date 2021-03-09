using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class controls : MonoBehaviour
{
    public GameObject cursor;
    public Collider2D[] cl;
    public tileManager tm;
    Vector2 startpos;
    Vector2 endpos;
    bool canswipe = true;

    public GameObject maskobj;


    void swipedelay()
    {
        canswipe = true;
    }
    void Update()
    {
        maskobj.transform.position = cursor.transform.position;
        Vector2 pointt;
        if (Input.touchCount > 0)
        {
            Touch t = Input.GetTouch(0);
            Vector2 direction;
            if (t.phase == TouchPhase.Began)
            {
                startpos = new Vector2(t.position.x / (float)Screen.width, t.position.y / (float)Screen.width);
            }
            if (t.phase == TouchPhase.Ended && canswipe == true)
            {

                endpos = new Vector2(t.position.x / (float)Screen.width, t.position.y / (float)Screen.width);

                direction = new Vector2(endpos.x - startpos.x, endpos.y - startpos.y);
                Invoke("swipedelay", 0.35f);
                if (direction.x > 0 && direction.magnitude > 0.15f)
                {
                    StartCoroutine(rotetehexagons(-120, 2));

                }
                else if (direction.x < 0 && direction.magnitude > 0.15f)
                {
                    StartCoroutine(rotetehexagons(120, 2));
                }
                else
                {
                    pointt = Camera.main.ScreenToWorldPoint(new Vector2(Input.GetTouch(0).position.x,
               Input.GetTouch(0).position.y));
                    docontrols(pointt);
                }

                canswipe = false;
            }
        }

    }

    void docontrols(Vector2 pointt)
    {
        Collider2D[] temp = cl;

        //cl = null;
        cl = Physics2D.OverlapCircleAll(pointt, 0.4f);
        if (cl.Length > 3 || cl.Length <= 2)
        {
            // cl = null;
            cl = temp;
            return;
        }
        tm.refresh();
        // check distance if its on border 
        if (Vector3.Distance(cl[0].transform.position, cl[1].transform.position) > 1f ||
            Vector3.Distance(cl[0].transform.position, cl[2].transform.position) > 1f ||
            Vector3.Distance(cl[1].transform.position, cl[2].transform.position) > 1f)
        {
            return;
        }
        Vector3 sum = new Vector3(0, 0, 0);
        //cursor.transform.position = new Vector3(3, -3, 0);



        //highlights the selected 
        for (int i = 0; i < 3; i++)
        {
            cl[i].GetComponent<tile>().sprite.sortingOrder += 2;
            cl[i].GetComponent<tile>().isselectedsprite.color = Color.white;
            cl[i].GetComponent<tile>().isselectedsprite.sortingOrder += 1;
            //Destroy(c.gameObject);
            sum += cl[i].transform.position;
        }
        cursor.transform.position = sum / 3;

        foreach (Collider2D g in cl)
        {
            g.transform.parent = cursor.transform;
        }

    }



    IEnumerator rotetehexagons(float a, int rotatecount)
    {
        maskobj.transform.rotation = cursor.transform.rotation;
        maskobj.transform.Rotate(0,0,a);
        //cursor.transform.Rotate(0, 0, a);

        StartCoroutine( rotateObject(cursor.transform.rotation,
            maskobj.transform.rotation,0.1f));

        yield return new WaitForSeconds(0.15f);
        //detach the childrens
        
        GameObject[] tiles = new GameObject[3];

        int index = 0;
        foreach (Transform g in cursor.transform)
        {
            tiles[index] = g.gameObject;
            index++;
        }
        
       cursor.transform.DetachChildren();
       foreach (GameObject gg in tiles)
       {
            if (gg==null)
            {
                continue;
            }
           float xpos = gg.transform.position.x;
           float ypos = gg.transform.position.y;

           gg.transform.localScale = new Vector3(1, 1, 1);
           gg.transform.rotation = Quaternion.Euler(0, 0, 0);
           xpos = Mathf.Round(xpos * 10.0f) * 0.1f;
           // ypos = Mathf.Round(ypos * 10.0f) * 0.1f;

           gg.transform.position = new Vector2(xpos, ypos);
       }

         
        bool stopturning;

        stopturning = tm.checkforgrouped();
        //stopturning = false;     
        




        if (stopturning == false)
        {
            newturn();
            foreach (GameObject t in tiles)
            {
                if (t == null)
                {
                    continue;
                }
                t.transform.parent = cursor.transform;
            }

            if (rotatecount > 0)
            {
                rotatecount -= 1;
                StartCoroutine(rotetehexagons(a, rotatecount));
            }
            else
            {

                tm.refresh();
                //StartCoroutine("newturn", 0.5f);
                newturn();
                

            }
        }
        else
        {
            tm.refresh();
            //StartCoroutine("newturn", 1f);
            newturn();
            tm.turnpassed();

        }

    }


    void newturn()
    {
        docontrols(new Vector2(cursor.transform.position.x,
                    cursor.transform.position.y));
    }

    IEnumerator MoveObject(Vector3 source, Vector3 target, float overTime)
    {
        float startTime = Time.time;
        while (Time.time < startTime + overTime)
        {
            transform.position = Vector3.Lerp(source, target, (Time.time - startTime) / overTime);
            yield return null;
        }
        transform.position = target;
    }
    IEnumerator rotateObject(Quaternion source, Quaternion target, float overTime)
    {
        float startTime = Time.time;
        while (Time.time < startTime + overTime)
        {
            //transform.position = Vector3.Lerp(source, target, (Time.time - startTime) / overTime);
            cursor.transform.rotation = Quaternion.Lerp(source,target, (Time.time - startTime) / overTime);
            yield return null;
        }
        cursor.transform.rotation = target;
        maskobj.transform.rotation = target;
    }
}







