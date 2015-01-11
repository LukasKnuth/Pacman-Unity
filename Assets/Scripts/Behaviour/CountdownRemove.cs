using UnityEngine;
using System.Collections;

public class CountdownRemove : MonoBehaviour {

    public float SecondsToWait;
    private float _timer;

	// Update is called once per frame
	void Update () {
	    this._timer += Time.deltaTime;
	    if (this._timer >= SecondsToWait) {
	        Destroy(this.gameObject);
	    }
	}
}
