using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class ElementControl : MonoBehaviour {
    public GameObject[] Controllers;
    public GameObject stone, wall;
    private GameObject[] stoneIntantiated;
    private GameObject wallIntantiated;
    
    private float valueToLerp;
    private int numWalls = 0;
    private bool[] stoneIsSpawned;
    private bool objectAction = false;

    private GameManager gm;
    private Vector3[] pos, rot, velocity;


    void Start() {
        gm = GameManager.GetInstance();
        gm.gameState = GameManager.GameState.START;

        stoneIsSpawned = new bool[Controllers.Length];
        pos = new Vector3[Controllers.Length];
        velocity = new Vector3[Controllers.Length];
        stoneIntantiated = new GameObject[Controllers.Length];

        for (int i = 0; i < Controllers.Length; i++) {
            stoneIsSpawned[i] = false;
            pos[i] = Controllers[i].transform.position;
            // rot[i] = Controllers[i].transform.rotation.eulerAngles;
            velocity[i] = Vector3.zero;
        }

        Invoke(nameof(Begin), 1f);
    }

    void Begin() {
        gm.gameState = GameManager.GameState.BEGIN;
    }

    void Update() {
        Debug.Log(objectAction);
        for (int i = 0; i < Controllers.Length; i++) {
            velocity[i] = (Controllers[i].transform.position - pos[i]) / Time.deltaTime;
            pos[i] = Controllers[i].transform.position;

            if (gm.gameState == GameManager.GameState.BEGIN) {
                velocity[i] = velocity[i].normalized;
                horizontalVel = Mathf.Sqrt(Mathf.Pow(velocity[i].z, 2) + Mathf.Pow(velocity[i].x, 2));

                Debug.Log(velocity[i] + " " + horizontalVel);

                if ((velocity[0].y < -0.5f && velocity[1].y < -0.5f && !objectAction)) {
                    objectAction = true;
                    CreateWall();
                }
                else if (horizontalVel > 0.5f && !objectAction) {
                    objectAction = true;
                    PushWall();
                }
                else if (velocity[i].y > 0.5f && !objectAction) {
                    SpawnStone(i);
                }
                else if (horizontalVel > 0.5f && !objectAction) {
                    objectAction = true;
                    CreateWall();
                }
            }
        }
    }


    // Wall
    void CreateWall() {
        Debug.Log("Creating wall");
        if (numWalls < 1) {
            wallIntantiated = Instantiate(wall, new Vector3(pos[0].x, -1, pos[0].z)+(Vector3.forward*2f), Quaternion.identity);
            StartCoroutine(WallUp(wallIntantiated, 1.0f));
            numWalls++;
        }
    }

    IEnumerator WallUp(GameObject wall, float time = 1f) {
        float t = 0;
        float x = wall.transform.position.x;
        float z = wall.transform.position.z;
        while (t < time) {
            t += Time.deltaTime;
            wall.transform.position = Vector3.Lerp(wall.transform.position, new Vector3(x, 0.8f, z), t / time);
            yield return null;
        }
        objectAction = false;
    }

    void PushWall() {
        Debug.Log("Pushing wall");
        if (wallIntantiated != null) {
            // mean of the velocity
            Vector3 mean = (velocity[0] + velocity[1]) / 2;
            wallIntantiated.GetComponent<Rigidbody>().AddForce(new Vector3(mean.x, 0, mean.z) * 5f, ForceMode.Impulse);
            wallIntantiated = null;
            numWalls--;
        }
        objectAction = false;
    }


    // Stone
    void SpawnStone(int i) {
        Debug.Log("Spawning stone");
        if (stoneIntantiated[i] == null) {
            objectAction = true;
            stoneIntantiated[i] = Instantiate(stone, new Vector3(pos[i].x, 0, pos[i].z), Quaternion.identity);
            StartCoroutine(StoneUp(stoneIntantiated[i], i, 0.8f));
        }
    }

    IEnumerator StoneUp(GameObject stone, int i, float time = 1f) {
        float t = 0;
        float x = stone.transform.position.x;
        float z = stone.transform.position.z;
        while (t < time) {
            t += Time.deltaTime;
            stone.transform.position = Vector3.Lerp(stone.transform.position, new Vector3(x, pos[i].y, z), t / time);
            yield return null;
        }
        objectAction = false;
    }

    void PushStone(int i) {
        Debug.Log("Pushing stone");
        if (stoneIntantiated[i] != null) {
            objectAction = true;
            stoneIntantiated[i].GetComponent<Rigidbody>().AddForce(velocity[i]*5f, ForceMode.Impulse);
            stoneIntantiated[i].GetComponent<Rigidbody>().useGravity = true;
            stoneIntantiated[i] = null;
        }
        objectAction = false;
    }
}
