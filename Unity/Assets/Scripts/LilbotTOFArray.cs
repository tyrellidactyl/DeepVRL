using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Valve.VR;
using System.Linq;
using UnityEngine.UI;

/* Creates an in-game UI to display readings from a single 
distance sensor attached to a controller GameObject */

public class LilbotTOFArray : MonoBehaviour
{
    public Transform lilbotPose;
    public Transform lilbotPoseLeft;
    public Transform lilbotPoseRight;

    public GameObject laserPrefab; // 1
    private GameObject laser; // 2 stores a reference to an instance of the laser.
    private GameObject laserLeft;
    private GameObject laserRight;
    private Transform laserTransform; // 3
    private Transform laserLeftTransform; // 3
    private Transform laserRightTransform; // 3
    private Vector3 hitPoint; // 4 This is the position where the laser hits.
    private Vector3 hitPointLeft;
    private Vector3 hitPointRight;

    private static float hitPointCenterDist = 3f;
    private static float hitPointLeftDist = 3f;
    private static float hitPointRightDist = 3f;
    public short CenterDist = System.Convert.ToInt16(hitPointCenterDist);
    public short LeftDist = System.Convert.ToInt16(hitPointLeftDist);
    public short RightDist = System.Convert.ToInt16(hitPointRightDist);
    public ArrayList TOFreadings = new ArrayList { hitPointCenterDist, hitPointLeftDist, hitPointRightDist };

    public GameObject respawn;
    public GameObject left;
    public GameObject right;

    private static List<string> logs = new List<string>();
    private static List<string> logsLeft = new List<string>();
    private static List<string> logsRight = new List<string>();
    private static Text text;
    private static Text textLeft;
    private static Text textRight;
    public static int nLogs = 1; // max number of lines to display. Can be changed

    void Start()
    {
        // 1 Spawn a new laser and save a reference to it in laser.
        laser = Instantiate(laserPrefab, respawn.transform.position, respawn.transform.rotation);
        laserLeft = Instantiate(laserPrefab, left.transform.position, left.transform.rotation);
        laserRight = Instantiate(laserPrefab, right.transform.position, right.transform.rotation);
        // 2 Store the laser’s transform component.
        laserTransform = laser.transform;
        laserLeftTransform = laserLeft.transform;
        laserRightTransform = laserRight.transform;

        // create a canvas and a text element to display the logs
        GameObject goCanvas = new GameObject("Canvas");
        GameObject goCanvasLeft = new GameObject("CanvasLeft");
        GameObject goCanvasRight = new GameObject("CanvasRight");
        Canvas canvas = goCanvas.AddComponent<Canvas>();
        Canvas canvasLeft = goCanvasLeft.AddComponent<Canvas>();
        Canvas canvasRight = goCanvasRight.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvasLeft.renderMode = RenderMode.WorldSpace;
        canvasRight.renderMode = RenderMode.WorldSpace;

        GameObject goText = new GameObject("Text");
        GameObject goTextLeft = new GameObject("TextLeft");
        GameObject goTextRight = new GameObject("TextRight");
        goText.transform.parent = goCanvas.transform;
        goTextLeft.transform.parent = goCanvasLeft.transform;
        goTextRight.transform.parent = goCanvasRight.transform;
        goText.transform.position = Vector3.zero;
        goTextLeft.transform.position = Vector3.zero;
        goTextRight.transform.position = Vector3.zero;
        goText.transform.position = new Vector3(0, .75f, 0);
        goTextLeft.transform.position = new Vector3(0, .75f, -0.5f);
        goTextRight.transform.position = new Vector3(0, .75f, 0.5f);
        goText.transform.rotation *= Quaternion.Euler(0, 270f, 0);
        goTextLeft.transform.rotation *= Quaternion.Euler(0, 270f, 0);
        goTextRight.transform.rotation *= Quaternion.Euler(0, 270f, 0);

        text = goText.AddComponent<Text>();
        textLeft = goTextLeft.AddComponent<Text>();
        textRight = goTextRight.AddComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        textLeft.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        textRight.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        text.fontSize = 20;
        textLeft.fontSize = 20;
        textRight.fontSize = 20;
        text.color = Color.black;
        textLeft.color = Color.black;
        textRight.color = Color.black;
        RectTransform tr = goText.GetComponent<RectTransform>();
        RectTransform trLeft = goTextLeft.GetComponent<RectTransform>();
        RectTransform trRight = goTextRight.GetComponent<RectTransform>();
        tr.localScale = new Vector3(0.005f, 0.005f, 0.01f);
        trLeft.localScale = new Vector3(0.005f, 0.005f, 0.01f);
        trRight.localScale = new Vector3(0.005f, 0.005f, 0.01f);
        tr.sizeDelta = new Vector2(75, 100);
        trLeft.sizeDelta = new Vector2(75, 100);
        trRight.sizeDelta = new Vector2(75, 100);
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;

        // 2 Shoot a ray from the controller. 
        // If it hits something, make it store the point where it hit and show the laser.
        if (Physics.Raycast(lilbotPose.transform.position, lilbotPose.transform.forward, out hit, 10))
        {
            hitPoint = hit.point;
            hitPointCenterDist = hit.distance * 1000;
            CenterDist = System.Convert.ToInt16(hitPointCenterDist);
            TOFreadings[1] = CenterDist;
            ShowLaser(hit);
            //Debug.Log("Distance Reading: " + hit.distance);
            Log("Center: " + CenterDist);
        }

        if (Physics.Raycast(lilbotPoseLeft.transform.position, lilbotPoseLeft.transform.forward, out hit, 100))
        {
            hitPointLeft = hit.point;
            hitPointLeftDist = hit.distance * 1000;
            LeftDist = System.Convert.ToInt16(hitPointLeftDist);
            TOFreadings[0] = LeftDist;
            ShowLaserLeft(hit);
            LogLeft("Left: " + LeftDist);
        }

        if (Physics.Raycast(lilbotPoseRight.transform.position, lilbotPoseRight.transform.forward, out hit, 100))
        {
            hitPointRight = hit.point;
            hitPointRightDist = hit.distance * 1000;
            RightDist = System.Convert.ToInt16(hitPointRightDist);
            TOFreadings[2] = RightDist;
            ShowLaserRight(hit);
            LogRight("Right: " + RightDist);
        }

    }

    // This method takes a RaycastHit 
    // as a parameter because it contains the position of the hit and the distance it traveled.
    private void ShowLaser(RaycastHit hit)
    {
        // 1 Show the laser.
        laser.SetActive(true);
        // 2 Position the laser between the controller and the point where the raycast hits.
        // You use Lerp because you can give it two positions and the percent it should travel. 
        // If you pass it 0.5f, which is 50%, it returns the precise middle point.
        laserTransform.position = Vector3.Lerp(respawn.transform.position, hitPoint, .5f);
        // 3 Point the laser at the position where the raycast hit.
        laserTransform.LookAt(hitPoint);
        // 4 Scale the laser so it fits perfectly between the two positions.
        laserTransform.localScale = new Vector3(laserTransform.localScale.x,
                                                laserTransform.localScale.y,
                                                hit.distance);
    }

    private void ShowLaserLeft(RaycastHit hit)
    {
        laserLeft.SetActive(true);
        laserLeftTransform.position = Vector3.Lerp(left.transform.position, hitPointLeft, .5f);
        laserLeftTransform.LookAt(hitPointLeft);
        laserLeftTransform.localScale = new Vector3(laserLeftTransform.localScale.x,
                                                laserLeftTransform.localScale.y,
                                                hit.distance);
    }

    private void ShowLaserRight(RaycastHit hit)
    {
        laserRight.SetActive(true);
        laserRightTransform.position = Vector3.Lerp(right.transform.position, hitPointRight, .5f);
        laserRightTransform.LookAt(hitPointRight);
        laserRightTransform.localScale = new Vector3(laserRightTransform.localScale.x,
                                                laserRightTransform.localScale.y,
                                                hit.distance);
    }

    public static void Log(object log)
    {
        if (text == null)
        {
            Debug.Log(log);
            return;
        }
        // add the log to the queue
        string s = log.ToString();
        logs.Add(s);
        // make sure we don't keep too many logs
        if (logs.Count > nLogs)
        {
            logs.RemoveAt(0);
            PrintLogs();
        }
    }

    public static void LogLeft(object log)
    {
        if (text == null)
        {
            Debug.Log(log);
            return;
        }
        // add the log to the queue
        string s = log.ToString();
        logsLeft.Add(s);
        // make sure we don't keep too many logs
        if (logsLeft.Count > nLogs)
        {
            logsLeft.RemoveAt(0);
            PrintLogsLeft();
        }
    }

    public static void LogRight(object log)
    {
        if (text == null)
        {
            Debug.Log(log);
            return;
        }
        // add the log to the queue
        string s = log.ToString();
        logsRight.Add(s);
        // make sure we don't keep too many logs
        if (logsRight.Count > nLogs)
        {
            logsRight.RemoveAt(0);
            PrintLogsRight();
        }
    }

    private static void PrintLogs()
    {
        string s = "";
        foreach (string k in logs)
        {
            s += k + "\n";
        }
        text.text = s;
    }

    private static void PrintLogsLeft()
    {
        string s = "";
        foreach (string k in logsLeft)
        {
            s += k + "\n";
        }
        textLeft.text = s;
    }

    private static void PrintLogsRight()
    {
        string s = "";
        foreach (string k in logsRight)
        {
            s += k + "\n";
        }
        textRight.text = s;
    }
}
