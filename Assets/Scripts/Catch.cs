﻿using UnityEngine;
using UnityEngine.UI;

public class Catch : MonoBehaviour {

    [Header("Times")]
    public float timeToCatch = 5;
    public float timeToBring = 10;

    [Header("Prefabs")]
    public GameObject timeToBringPrefab;
    public GameObject timeToCatchPrefab;

    [Header("Materials")]
    public Material playerMat;

    [Header("Scripts")]
    public Timer timeToCatchTimer;
    public Timer timeToBringTimer;

    private bool collided = false;
    private GameObject toCatchUI;
    private GameObject toBringUI;
    private GameObject canvas;

    // Use this for initialization
    void Start () {
        canvas = GameObject.Find("Canvas");
        startCatchTimer();
    }
	
	// Update is called once per frame
	void Update () {
        if (collided) {
            UI.moveUIWithObject(toBringUI, transform.position, 100, 100);
            updateTimerUI(toBringUI.transform.GetChild(0).gameObject, timeToBringTimer);
            destroyOnFinish();
            CatchSpawn.catchCubeName = null;
        } else {
            updateTimerUI(toCatchUI, timeToCatchTimer);
        }
    }

    /* OnColissionEnter
	 * collision -> returns the object which it has collided with
	 * */
    void OnCollisionEnter(Collision collision) {
		/* If its first collision
		 * */
		if (!collided && collision.transform.tag == "Player") {
            //deactivates the Dashed border on the catch position
            HighlightController.deactivateAllDashedBorders();

            //disable and destroy catch timer
            timeToCatchTimer.stopTimer();
            Destroy(toCatchUI);

            attachCatchToPlayer(collision.transform.parent);

			collided = true; //no more collisions

            disableCollisionsAndAnimations(collision);

            startBringTimer();

            /* get a reference to the last cube with the catch
             * make it a normal catch cube if the catch is cought
             * */
            setLastAsNormal();

        }
	}

    void isTimeLeft(float time) {
        if (time <= 0) destroyAll(true);
    }

    void updateTimerUI(GameObject timerUI, Timer timer) {
        float timeRemaining = timer.TimeRemaining();
        isTimeLeft(timeRemaining);
        timerUI.GetComponent<Text>().text = Timer.secondsToMinutes(timeRemaining);
    }

    void startCatchTimer () {
        toCatchUI = Instantiate(timeToCatchPrefab, canvas.transform);
        toCatchUI.transform.localScale = new Vector3(1f, 1f, 1f);
        UI.moveUIWithObject(toCatchUI, transform.position, 0, 75);
        timeToCatchTimer.startTimer(timeToCatch);
    }

    void startBringTimer() {
        toBringUI = Instantiate(timeToBringPrefab, canvas.transform);
        timeToBringTimer.startTimer(timeToBring);
    }

    void setLastAsNormal() {
        GameObject makeNormalCube = GameObject.Find(CatchSpawn.catchCubeName);
        makeNormalCube.transform.tag = "CatchCube";
    }

    void attachCatchToPlayer(Transform player) {
        transform.parent = player; //make it a child of object it has collided with
        transform.SetAsLastSibling(); //sets the catch as last sibling in p1 gameobject
        transform.localPosition = new Vector3(0f, 0.6f, 0f); //sets its local position to center
    }

    void disableCollisionsAndAnimations(Collision collision) {
        GetComponent<SphereCollider>().enabled = false; //disable sphere colider for clicks.
        collision.transform.GetComponent<Animator>().SetBool("catch", false); //disables the animation of the cube
    }

    /*
     * Checks if Player has a child ( catch )
     * And is on Destination ( coordinates on Vector3 destination )
     * and destroys All if both true 
     * */

    void destroyOnFinish () {
        RaycastHit hit;

        if (Physics.Raycast(HighlightController.catchDestination, Vector3.up, out hit)) {
            //if that is player and has a child in it 
            if (hit.transform.name.Contains("Target") && hit.transform.childCount > 0) {

                //getting the real cube reference
                Transform player = transform.parent.GetChild(0);

                //length of the player GameObject name
                int nameLength = transform.parent.GetChild(0).name.Length;
                
                //removing the last 6 simbols (TARGET)
                player.name = player.name.Substring(0, nameLength - 6);

                player.GetComponent<MoveController>().setNewPosition(Players.randomFreePosition());

                //destroying the catch
                destroyAll(false);

                //Adds Score and refreshes material
                Score.addScore();
                Materials.set(player.gameObject, playerMat);
            }
        }
    }

    /*
     * Destroys every gameObject related to the catch
     * Including:
     * -> self GameObject
     * -> image GameObject
     * -> text GameObject
     * sets counterActive = false
     * */
    void destroyAll (bool gameOver) {
        Destroy(toCatchUI);
        Destroy(toBringUI);

        Destroy(gameObject);
        if (gameOver) {
            GameManager.gameOver();
        }
    }

}
