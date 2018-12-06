using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorAnnouncer : MonoBehaviour {

    private ActionController _actionController;

    public void SetActionController(ActionController actionController){
        _actionController = actionController;
    }
    // Use this for initialization
    public void AnnounceStart () {

        _actionController.AnimationStart();
    }
    
    // Update is called once per frame
    public void AnnounceEnd() {

        _actionController.AnimationEnd();
    }
}
