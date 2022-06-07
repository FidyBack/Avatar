using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehaviour : MonoBehaviour {
    private Transform player;
    private NavMeshAgent nav;


    public void Start() {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        nav = GetComponent<NavMeshAgent>();

    }

    public void Update() {
        nav.SetDestination(player.position);
    }

}
