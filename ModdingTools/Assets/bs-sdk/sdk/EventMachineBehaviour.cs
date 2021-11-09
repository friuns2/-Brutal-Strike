using UnityEngine;


public class EventMachineBehaviour : StateMachineBehaviour
{
    public EventType2 eventType;
    // public float eTime;
    public Object Object;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.GetComponent<ForwardEvents>().instantiateEffect?.Invoke(eventType != EventType2.None ? (object) eventType : Object);
    }
    // public override void OnStateUpdate(Animator animator, AnimatorStateInfo state, int layerIndex)
    // {
    //     var time = state.normalizedTime * state.length * state.speed * state.speedMultiplier;
    //
    //     if (time > eTime && time - TimeCached.deltaTime * state.speed * state.speedMultiplier < eTime)
    //         animator.GetComponent<ForwardEvents>().instantiateEffect?.Invoke(this);
    //
    //     base.OnStateUpdate(animator, state, layerIndex);
    // }
    
}
