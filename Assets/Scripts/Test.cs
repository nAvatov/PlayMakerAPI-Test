using UnityEngine;
using HutongGames.PlayMaker;

public class Test : MonoBehaviour {
    [SerializeField] private UnityEngine.UI.Button _button;
    private PlayMakerFSM _pmFsm;

    private void Start() {
        _pmFsm = _button.GetComponent<PlayMakerFSM>();

        Setup();
    }

    private FsmTransition CreateTransition(FsmState destionationState, FsmEvent transitionEvent) {
        FsmTransition transition = new FsmTransition();
        transition.ToFsmState = destionationState;
        transition.FsmEvent = transitionEvent;

        return transition;
    }

    private FsmState CreateState(string name) {
        FsmState state = new FsmState(_pmFsm.Fsm);
        state.Name = name;
        
        return state;
    }

    private void Setup() {
        FsmState[] states = new FsmState[] { 
            CreateState("idle"),
            CreateState("wating 3 seconds"),
            CreateState("print in console")
        };

        _pmFsm.Fsm.Events = new FsmEvent[] {
            new FsmEvent("UI CLICK"),
            new FsmEvent("FINISHED")
        };
        
        HutongGames.PlayMaker.Actions.UiButtonOnClickEvent clickAction = new HutongGames.PlayMaker.Actions.UiButtonOnClickEvent();
        HutongGames.PlayMaker.Actions.Wait waitAction = new HutongGames.PlayMaker.Actions.Wait();
        HutongGames.PlayMaker.Actions.DebugLog logAction = new HutongGames.PlayMaker.Actions.DebugLog();

        CreateActions(clickAction, waitAction, logAction);

        SetupStates(states, clickAction, waitAction, logAction);

        FillStatesList(states);
        ReplaceStates();
    }


    /// <summary>
    /// Reinitializing states array of Fsm component array and setting starting state
    /// </summary>
    /// <param name="states"></param>
    private void FillStatesList(FsmState[] states) {
        _pmFsm.Fsm.States = null;

        _pmFsm.Fsm.States = states;

        _pmFsm.Fsm.StartState = states[0].Name;
        _pmFsm.SetState(_pmFsm.Fsm.StartState);
    }

    /// <summary>
    /// Replacing spawned states in FSM canvas field
    /// </summary>
    private void ReplaceStates() {
        int placeCoord = 0;
        foreach(FsmState state in _pmFsm.Fsm.States) {
            state.Position = new Rect(placeCoord, placeCoord, 120, 50);
            placeCoord += 100;
        }
    }

    /// <summary>
    /// Setup satets actions and transitions
    /// </summary>
    /// <param name="states"></param>
    /// <param name="clickAction"></param>
    /// <param name="waitAction"></param>
    /// <param name="logAction"></param>
    private void SetupStates(FsmState[] states, HutongGames.PlayMaker.Actions.UiButtonOnClickEvent clickAction, HutongGames.PlayMaker.Actions.Wait waitAction, HutongGames.PlayMaker.Actions.DebugLog logAction) {
        states[0].Actions = new FsmStateAction[] { clickAction };
        states[0].Transitions = new FsmTransition[] { CreateTransition(states[1], _pmFsm.Fsm.Events[0]) };
        
        states[1].Actions = new FsmStateAction[] { waitAction };
        states[1].Transitions = new FsmTransition[] { CreateTransition(states[2], _pmFsm.Fsm.Events[1]) };

        states[2].Actions = new FsmStateAction[] { logAction };
    }

    /// <summary>
    /// Creates custom actions for current task
    /// </summary>
    /// <param name="clickAction"></param>
    /// <param name="waitAction"></param>
    /// <param name="logAction"></param>
    private void CreateActions(HutongGames.PlayMaker.Actions.UiButtonOnClickEvent clickAction, HutongGames.PlayMaker.Actions.Wait waitAction, HutongGames.PlayMaker.Actions.DebugLog logAction) {
        clickAction.sendEvent = _pmFsm.Fsm.Events[0];
        clickAction.eventTarget = FsmEventTarget.Self;
        FsmOwnerDefault newDefaultOwner = new FsmOwnerDefault();
        newDefaultOwner.GameObject = _button.gameObject;
        clickAction.gameObject = newDefaultOwner;

        waitAction.time = 3f;
        waitAction.finishEvent = _pmFsm.Fsm.Events[1];

        logAction.text = "Button Clicked!";
        logAction.sendToUnityLog = true;
    }
}
