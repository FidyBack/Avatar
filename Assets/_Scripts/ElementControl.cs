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
    private Vector3[] pos, velocity;


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
                if (velocity[0].y < -4.5f && velocity[1].y < -4.5f && !objectAction) {
                    objectAction = true;
                    CreateWall();
                }
                else if (velocity[0].z > 3f && velocity[1].z > 3f && !objectAction) {
                    objectAction = true;
                    PushWall();
                }
                else if (velocity[i].y > 4.5f && !objectAction) {
                    SpawnStone(i);
                }
                else if (velocity[i].z > 4f && !objectAction) {
                    PushStone(i);
                }
            }
        }
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


    // Wall
    void CreateWall() {
        Debug.Log("Creating wall");
        if (numWalls < 1) {
            wallIntantiated = Instantiate(wall, new Vector3(pos[0].x, 0, pos[0].z)+(Vector3.forward*1.5f), Quaternion.identity);
            StartCoroutine(WallUp(wallIntantiated, 2.0f));
            numWalls++;
        }
    }

    IEnumerator WallUp(GameObject wall, float time = 1f) {
        float t = 0;
        float x = wall.transform.position.x;
        float z = wall.transform.position.z;
        while (t < time) {
            t += Time.deltaTime;
            wall.transform.position = Vector3.Lerp(wall.transform.position, new Vector3(x, 1f, z) + (Vector3.forward * 1.5f), t / time);
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
}
