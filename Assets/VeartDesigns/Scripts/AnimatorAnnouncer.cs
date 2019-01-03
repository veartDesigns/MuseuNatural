using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorAnnouncer : MonoBehaviour {

    private ActionController _actionController;
    private ObjectInfo _objectInfo;

    public void SetActionController(ActionController actionController,ObjectInfo objectInfo){
        _actionController = actionController;
        _objectInfo = objectInfo;
    }
    // Use this for initialization
    public void AnnounceStart () {

        _actionController.AnimationStart(_objectInfo);
    }
    
    // Update is called once per frame
    public void AnnounceEnd() {

        _actionController.AnimationEnd(_objectInfo);
    }
    // Update is called once per frame
    public void AnnounceEgagropialaAnim()
    {
        _actionController.AnimateEgagropila();
    }
    public void AnnounceLastAnimation(){

        _actionController.AnnounceLastAnimation();
    }
    public void AnnounceEndOfStory()
    {
        _actionController.AnnounceEndOfStory();
    }

}
