using UnityEngine;
using System.Collections;

public class JumpLand : StateMachineBehaviour {

	#pragma warning disable 414

	public StepManager step;
	AnimatorStateInfo currentState;
	float playbackTime;
	bool play;
	public ParticleSystem dustLanding;
	public bool hasPoofed = false;

	public void Start(){

	}

	 // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		dustLanding = Object.FindObjectOfType<ParticleSystem>();
		dustLanding.enableEmission = false;
		dustLanding.Emit(20);
		step = Object.FindObjectOfType<StepManager> ();
		if(step == null)
			return;
		step.GrassStep ();
		currentState = animator.GetCurrentAnimatorStateInfo(0);
	}

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	/*override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {

		currentState = animator.GetCurrentAnimatorStateInfo(0);
		playbackTime = currentState.normalizedTime % 1;

		if (playbackTime > 0.25f && playbackTime < 0.3f && !play) {
			play = true;
			step.GrassStep ();
			Debug.Log ("Land2");
		} 

		else if (playbackTime > 0.3f) {
			play = false;
		}
	}*/

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	//override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}

	// OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
	//override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}

	// OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
	//override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}
}
