using UnityEngine;
using System.Collections;

public class Footsteps : StateMachineBehaviour {

	public StepManager step;
	public AnimationClip anim;
	AnimatorStateInfo currentState;
	float playbackTime;
	bool play1, play2;

	 // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		step = Object.FindObjectOfType<StepManager> ();
		currentState = animator.GetCurrentAnimatorStateInfo(0);
		play1 = false;
		play2 = false;
	}

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		currentState = animator.GetCurrentAnimatorStateInfo(0);
		playbackTime = currentState.normalizedTime % 1;

		if (playbackTime < 0.3f && !play1){
			play1 = true;
			step.GrassStep();
			Debug.Log ("1");
		}

		else if (playbackTime > 0.5f && playbackTime < 0.8f && !play2){
			play2 = true;
			step.GrassStep();
			Debug.Log ("2");
		}

		else if (playbackTime > 0.81f){
			play1 = false;
			play2 = false;
			Debug.Log ("3");
		}
	}

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
