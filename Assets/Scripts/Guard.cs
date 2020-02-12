using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

public class Guard : MonoBehaviour
{
    public static event Action GardSpotPlayer;
    
    public Transform pathHolder;
    
    public float speed = 5f;
    public float waitTime = 0.3f;
    public float turnSpeed = 90f;
    public float timeToSpot = .5f;

    public Light spotLight;
    public float viewDistance;
    public LayerMask viewMask;
    private float viewAngle;

    private Transform player;
    private Color originalSpotlight;
    private float playerVisibleTimer;
    
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        originalSpotlight = spotLight.color;
        
        viewAngle = spotLight.spotAngle;
        
        Vector3[] waypoints = new Vector3[pathHolder.transform.childCount];
        for (int i = 0; i < waypoints.Length; i++)
        {
            waypoints[i] = pathHolder.GetChild(i).position;
            waypoints[i] = new Vector3(waypoints[i].x, transform.position.y, waypoints[i].z);
        }

        StartCoroutine(FollowPath(waypoints));
    }

    private void Update()
    {
        if (CanSeePlayer())
        {
           //spotLight.color = Color.red;
            playerVisibleTimer += Time.deltaTime;
        }
        else
        {
            //spotLight.color = originalSpotlight;
            playerVisibleTimer -= Time.deltaTime;
        }
        playerVisibleTimer = Mathf.Clamp(playerVisibleTimer, 0, timeToSpot);
        spotLight.color = Color.Lerp(originalSpotlight, Color.red, playerVisibleTimer/timeToSpot);

        if (playerVisibleTimer >= timeToSpot)
        {
            if (GardSpotPlayer != null)
            {
                GardSpotPlayer();
            }
        }
    }

    bool CanSeePlayer()
    {
        if (Vector3.Distance(transform.position, player.position) < viewDistance)
        {
            Vector3 dirToPlayer = (player.position - transform.position).normalized;
            float angleBetweenBoth = Vector3.Angle(transform.forward, dirToPlayer);

            if (angleBetweenBoth < viewAngle / 2)
            {
                if (!Physics.Linecast(transform.position, player.position, viewMask))
                {
                    return true;
                }
            }
        }
        return false;
    }

    IEnumerator FollowPath(Vector3[] waypoints)
    {
        transform.position = waypoints[0];
        int targetWaypointIndex = 1;
        Vector3 targetWaypoint = waypoints[targetWaypointIndex];
        transform.LookAt(targetWaypoint);
        
        while (true)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetWaypoint, speed * Time.deltaTime);
            if (transform.position == targetWaypoint)
            {
                targetWaypointIndex = (targetWaypointIndex + 1) % waypoints.Length;
                targetWaypoint = waypoints[targetWaypointIndex];
                yield return new WaitForSeconds(waitTime);
                yield return StartCoroutine(TurnToFace(targetWaypoint));
            }
            yield return null;
        }
    }

    IEnumerator TurnToFace(Vector3 looktarget)
    {
        Vector3 dirToLookTarget = (looktarget - transform.position).normalized;
        float targetAngle = 90 - Mathf.Atan2(dirToLookTarget.z, dirToLookTarget.x) * Mathf.Rad2Deg;

        while (Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y, targetAngle)) > 0.05f)
        {
            float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetAngle, turnSpeed * Time.deltaTime);
            transform.eulerAngles = Vector3.up * angle;
            yield return null;
        }
    }
    
    private void OnDrawGizmos()
    {
        Vector3 startPosition = pathHolder.GetChild(0).position;
        Vector3 previousPosition = startPosition;
        
        foreach (Transform waypoint in pathHolder)
        {
            Gizmos.DrawSphere(waypoint.position, 0.3f);
            Gizmos.DrawLine(previousPosition, waypoint.position);
            previousPosition = waypoint.position;
        }
        Gizmos.DrawLine(previousPosition, startPosition);
        
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * viewDistance);
    }
}
