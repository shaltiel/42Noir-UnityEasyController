using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//simple example of using easycontroller.

public class ControllingCube : MonoBehaviour
{
    bool initial = true; //

    void Update()
    {

        if (this.initial) // we do it once at the first update to initialise values in easycontroller.
        {
            EasyController.escon.set(1, 0);
            EasyController.escon.set(2, 0);
            EasyController.escon.set(3, 0);
            EasyController.esconsend.Send_data("1 rot_x"); //change name in easycontroller to s1a.

            this.initial = false;
        }

        float s1a = EasyController.escon.get("s1a", 0.0f, 5.0f); //receive value from slider 1a but remap range to 0-127 to 0..5.0f.
        float s2a = EasyController.escon.get("s2a", 0.0f, 5.0f); //receive value from slider 2a.
        float s3a = EasyController.escon.get("s3a"); //receive value from slider 3a.

        // this will rotate our cube by moving controllers knobs 1-3 of easycontroller.
        this.transform.Rotate(s1a, s2a, s3a);



        EasyController.esconsend.Send_data(111+(int)s1a, 1); //sending push button back to easycontrooler, we can use number or keys.
      

        List<int[]> notes_events = EasyController.escon.get_new_notes(); // receive all new notes that were pressed within the last frame rendering.

        IDictionary<int,int> active_notes = EasyController.escon.get_active_notes(); // get all notes are still pressed and not released in the last frame rendering.

        // let's make our keyboard notes scaling our cube's faces in its 3 dimensions.
        Vector3 pos = new Vector3(1.0f, 1.0f, 1.0f);
        int i = 0;
        foreach (int note in active_notes.Keys)
        {
            if (i<3)
            {
                pos[i] = 1.0f + (float)active_notes[note] / 128.0f;
            }
            i += 1;
            //UnityEngine.Debug.Log(notes_events[0][0]);
        }

        i = 0;

        // check all notes on or all notes are off send indicator to push button.
        foreach (int[] note in notes_events)
        {
            i += 1;
            EasyController.esconsend.Send_data("p1a", note[1]); 
        }

        this.transform.localScale = pos;


    }
}
