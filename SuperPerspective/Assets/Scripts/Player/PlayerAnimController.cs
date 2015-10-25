using UnityEngine;
using System.Collections;

public class PlayerAnimController : MonoBehaviour {

	public static PlayerAnimController instance;

	private PlayerController player;

	private Animator anim;
	private GameObject model;
	
	private bool animationPaused = false;
	private float prePauseAnimSpeed = 0;
	
	private bool playerWasJumping = false;
	
	private float orientation = 0;//TODO store in enum
	
	private const float epsilon = .1f;
	
	void Awake(){
		setupSingleton();
	}
	
	private void setupSingleton(){
		if(instance == null)
			instance = this;
		else if(instance != this)
			Destroy(this);
	}
	
	void Start () {
		initVariables();
	}
	
	private void initVariables(){
		player = PlayerController.instance;
		anim = GetComponentInChildren<Animator>();
		model = anim.gameObject;
	}
	
	void Update () {
		checkForJumpTrigger();
		updateAnimationStates();
		updateOrientation();
	}
	
	private void checkForJumpTrigger(){
		bool playerIsJumping = (player.isJumping() || player.isLaunched());
		if(!playerWasJumping && playerIsJumping){
			anim.SetTrigger("Jump");
			anim.SetInteger("EdgeState", 6);
			playerWasJumping = true;
		}
		
		if(player.isGrounded()) playerWasJumping = false;
	}
	
	private void updateAnimationStates(){
		updateMovementStates();
		updateCrateStates();
	}
	
	private void updateMovementStates(){
		anim.SetBool("Walking", player.isWalking() && !player.GrabbedCrate());
		anim.SetBool("Running", player.isRunning() && !player.GrabbedCrate());
		anim.SetBool("Kick", player.isKicking());
	}
	
	private void updateCrateStates(){
		if(player.GrabbedCrate()){
			float crateVel = Vector3.Dot(player.getVelocity(), player.getGrabAxis());
			anim.SetBool("Pushing", crateVel > epsilon);
			anim.SetBool("Pulling", crateVel < -epsilon);
			anim.SetBool("CrateIdle", -epsilon <= crateVel && crateVel <= epsilon);
		}else{
			anim.SetBool("Pulling", false);
			anim.SetBool("CrateIdle", false);
			anim.SetBool("Pushing", player.isPassivelyPushing());
		}
	}
	
	private void updateOrientation(){
		bool playerIsOnEdge = player.getEdgeState() == EdgeState.HANGING;
		bool playerIsMoving = player.getVelocity().magnitude > epsilon;
		
		bool playerCanRotateFreely = !player.isClimbing() && playerIsMoving && !player.GrabbedCrate() 
			&& !playerIsOnEdge;
		
		if (playerCanRotateFreely){
			orientation = Mathf.Rad2Deg * Mathf.Atan2(-player.getVelocity().z, player.getVelocity().x) + 90;
		}else if(playerIsOnEdge){
			orientation = (-1 - player.getEdgeOrientation()) * 90;
		}
		model.transform.rotation = Quaternion.AngleAxis(orientation, Vector3.up);
	}
	
	void FixedUpdate(){
		updateKick();
		updateVerticalVariables();
		updateShimmy();
		updateEdgeStates();
	}
	
	private void updateKick(){
		if(player.isFalling())
			anim.SetBool("Kick", false);
		else
			anim.SetBool("Kick", player.isKicking());
	}
	
	private void updateShimmy(){
		if(player.isShimmying() && anim.GetInteger("EdgeState") < 3)
			anim.SetInteger("EdgeState", 3);
	}
	
	private void updateVerticalVariables(){
		anim.SetBool("Falling", player.isFalling());
		anim.SetBool("Grounded", player.isGrounded());
	}
	
	private void updateEdgeStates(){
		//TODO store animation edge states into series of const variables
		int animEdgeState = anim.GetInteger("EdgeState");
		if(animEdgeState < 3 || (animEdgeState == 5 && !player.isClimbing()))	
			anim.SetInteger("EdgeState", (int)player.getEdgeState());
		else if(animEdgeState == 3){
			if(player.getVelocity().magnitude < epsilon)
				anim.SetInteger("EdgeState", (int)(int)player.getEdgeState());
		}else if(!anim.GetCurrentAnimatorStateInfo(0).IsName("HangBegin") && 
				!anim.GetCurrentAnimatorStateInfo(0).IsName("HangShimmying")){
			anim.SetInteger("EdgeState", (int)player.getEdgeState());
		}
	}

	#region public interface
	
	public void updateEdgeState(int animEdgeState){
		anim.SetInteger("EdgeState", animEdgeState);
	}
	
	public void pauseAnimation(){
		if(!animationPaused){
			prePauseAnimSpeed = anim.speed;
			anim.speed = 0;
			animationPaused = true;
		}
	}
	
	public void resumeAnimation(){
		if(animationPaused){
			anim.speed = prePauseAnimSpeed;
			animationPaused = false;
		}
	}
	
	public float getOrientation(){ return orientation; }
	
	#endregion public interface
}
